<script setup lang="ts">
import axios from 'axios'
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { getApiErrorMessages } from '@/api/errors'
import ItemListRow from '@/components/ItemListRow.vue'
import { useSnackbar } from '@/composables/useSnackbar'
import type { Campaign, CampaignCharacterReference } from '@/features/campaigns/api'
import {
  depositPartyStorage,
  getCharacterInventory,
  getPartyStorage,
  withdrawPartyStorage,
  type CharacterInventory,
  type PartyStorage,
  type PartyStorageItem,
} from './api'
import { isItemVersionConflict } from './versionConflict'
import { canWithdrawFromStorage } from './storagePolicy'

interface ControlledCharacter {
  id: number
  name: string
}

const props = defineProps<{
  campaign: Campaign
  characters: CampaignCharacterReference[]
}>()
const { d, t } = useI18n()
const snackbar = useSnackbar()
const storage = ref<PartyStorage | null>(null)
const errors = ref<string[]>([])
const isLoading = ref(true)
const depositDialog = ref(false)
const withdrawDialog = ref(false)
const selectedCharacterId = ref<number | null>(null)
const selectedInventoryItemKey = ref<string | null>(null)
const selectedStorageItem = ref<PartyStorageItem | null>(null)
const characterInventory = ref<CharacterInventory | null>(null)
const dialogErrors = ref<string[]>([])
const versionConflict = ref(false)
const isInventoryLoading = ref(false)
const isTransferring = ref(false)
const operationId = ref('')

const activeParty = computed(() =>
  props.campaign.parties.find((party) => party.status === 'Active'),
)
const isGameMaster = computed(() => props.campaign.roles.includes('GameMaster'))
const controlledCharacters = computed<ControlledCharacter[]>(() => {
  const names = new Map(props.characters.map((character) => [character.id, character.name]))
  return (
    activeParty.value?.characters
      .filter((item) => item.controlledByUserId === props.campaign.currentUserId)
      .map((item) => ({
        id: item.characterId,
        name: names.get(item.characterId) ?? `#${item.characterId}`,
      })) ?? []
  )
})
const selectedInventoryItem = computed(
  () =>
    characterInventory.value?.items.find(
      (item) => item.itemInstanceKey === selectedInventoryItemKey.value,
    ) ?? null,
)
const depositableItems = computed(
  () => characterInventory.value?.items.filter((item) => !item.isEquipped) ?? [],
)
const canWithdraw = computed(() =>
  storage.value ? canWithdrawFromStorage(storage.value.accessPolicy, isGameMaster.value) : false,
)

function newOperation(): void {
  operationId.value = globalThis.crypto.randomUUID()
  dialogErrors.value = []
  versionConflict.value = false
}

async function load(): Promise<void> {
  isLoading.value = true
  errors.value = []
  try {
    storage.value = await getPartyStorage(props.campaign.id)
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    isLoading.value = false
  }
}

async function loadCharacterInventory(): Promise<void> {
  if (!selectedCharacterId.value) {
    characterInventory.value = null
    return
  }

  isInventoryLoading.value = true
  dialogErrors.value = []
  try {
    characterInventory.value = await getCharacterInventory(
      props.campaign.id,
      selectedCharacterId.value,
    )
    if (selectedInventoryItemKey.value && !selectedInventoryItem.value) {
      selectedInventoryItemKey.value = null
    }
  } catch (error) {
    dialogErrors.value = getApiErrorMessages(error)
  } finally {
    isInventoryLoading.value = false
  }
}

function openDeposit(): void {
  newOperation()
  selectedCharacterId.value =
    controlledCharacters.value.length === 1 ? controlledCharacters.value[0]!.id : null
  selectedInventoryItemKey.value = null
  characterInventory.value = null
  depositDialog.value = true
  if (selectedCharacterId.value) void loadCharacterInventory()
}

function openWithdraw(item: PartyStorageItem): void {
  newOperation()
  selectedStorageItem.value = item
  selectedCharacterId.value =
    controlledCharacters.value.length === 1 ? controlledCharacters.value[0]!.id : null
  withdrawDialog.value = true
}

function handleTransferError(error: unknown): void {
  if (isItemVersionConflict(error)) {
    versionConflict.value = true
    return
  }

  dialogErrors.value = getApiErrorMessages(error).map((message) =>
    message.includes('Party storage policy does not permit')
      ? t('inventoryUi.storage.policyDenied')
      : message,
  )
  if (axios.isAxiosError(error) && error.response) {
    operationId.value = globalThis.crypto.randomUUID()
  }
}

async function deposit(): Promise<void> {
  if (!selectedCharacterId.value || !selectedInventoryItem.value || isTransferring.value) return

  isTransferring.value = true
  dialogErrors.value = []
  try {
    await depositPartyStorage(props.campaign.id, {
      characterId: selectedCharacterId.value,
      itemInstanceKey: selectedInventoryItem.value.itemInstanceKey,
      expectedItemVersion: selectedInventoryItem.value.version,
      operationId: operationId.value,
    })
    snackbar.success(
      t('inventoryUi.storage.deposited', { name: selectedInventoryItem.value.revision.name }),
    )
    depositDialog.value = false
    await Promise.all([load(), loadCharacterInventory()])
  } catch (error) {
    handleTransferError(error)
    if (versionConflict.value) {
      const itemKey = selectedInventoryItemKey.value
      await Promise.all([load(), loadCharacterInventory()])
      if (itemKey && selectedInventoryItem.value) {
        versionConflict.value = false
      } else {
        depositDialog.value = false
        snackbar.error(t('inventoryUi.storage.itemUnavailable'))
      }
    }
  } finally {
    isTransferring.value = false
  }
}

async function withdraw(): Promise<void> {
  if (!selectedCharacterId.value || !selectedStorageItem.value || isTransferring.value) return

  isTransferring.value = true
  dialogErrors.value = []
  try {
    await withdrawPartyStorage(props.campaign.id, {
      characterId: selectedCharacterId.value,
      itemInstanceKey: selectedStorageItem.value.item.itemInstanceKey,
      expectedItemVersion: selectedStorageItem.value.item.version,
      operationId: operationId.value,
    })
    const character = controlledCharacters.value.find(
      (item) => item.id === selectedCharacterId.value,
    )
    snackbar.success(
      t('inventoryUi.storage.withdrawn', {
        item: selectedStorageItem.value.item.name,
        character: character?.name ?? `#${selectedCharacterId.value}`,
      }),
    )
    withdrawDialog.value = false
    await Promise.all([load(), getCharacterInventory(props.campaign.id, selectedCharacterId.value)])
  } catch (error) {
    handleTransferError(error)
    if (versionConflict.value) {
      await load()
      const refreshed = storage.value?.items.find(
        (item) => item.item.itemInstanceKey === selectedStorageItem.value?.item.itemInstanceKey,
      )
      if (refreshed) {
        selectedStorageItem.value = refreshed
        versionConflict.value = false
      } else {
        withdrawDialog.value = false
        snackbar.error(t('inventoryUi.storage.itemUnavailable'))
      }
    }
  } finally {
    isTransferring.value = false
  }
}

watch(selectedCharacterId, () => {
  selectedInventoryItemKey.value = null
  if (depositDialog.value) void loadCharacterInventory()
})
onMounted(load)
</script>

<template>
  <section class="storage-tab">
    <header class="storage-header">
      <div>
        <h2>{{ t('inventoryUi.storage.title') }}</h2>
        <p>{{ t('inventoryUi.storage.lead') }}</p>
      </div>
      <v-tooltip v-if="storage?.accessPolicy === 'Unconfigured'">
        <template #activator="{ props: tooltipProps }">
          <v-chip v-bind="tooltipProps" color="error" variant="tonal">
            {{ t('inventoryUi.storage.policies.Unconfigured') }}
          </v-chip>
        </template>
        {{ t('inventoryUi.storage.unconfiguredHint') }}
      </v-tooltip>
      <v-chip v-else-if="storage" variant="tonal">
        {{ t(`inventoryUi.storage.policies.${storage.accessPolicy}`) }}
      </v-chip>
    </header>

    <v-progress-linear v-if="isLoading" color="accent" indeterminate rounded />
    <v-alert v-for="message in errors" :key="message" type="error" variant="tonal">
      {{ message }}
      <template #append>
        <v-btn variant="text" @click="load">{{ t('common.retry') }}</v-btn>
      </template>
    </v-alert>
    <v-alert v-if="!controlledCharacters.length && storage" type="info" variant="tonal">
      {{ t('inventoryUi.storage.noCharacter') }}
    </v-alert>

    <template v-if="storage">
      <v-btn
        v-if="controlledCharacters.length"
        color="primary"
        prepend-icon="mdi-tray-arrow-down"
        @click="openDeposit"
      >
        {{ t('inventoryUi.storage.depositAction') }}
      </v-btn>

      <v-card class="storage-panel" elevation="0">
        <v-list v-if="storage.items.length">
          <ItemListRow
            v-for="item in storage.items"
            :key="item.item.itemInstanceKey"
            :bulk-tenths="item.item.bulkTenths * item.item.quantity"
            :category="item.item.primaryCategory"
            :name="item.item.name"
            :quantity="item.item.quantity"
            :subtitle="
              item.depositedBy
                ? t('inventoryUi.storage.depositedBy', {
                    name: item.depositedBy.name,
                    date: item.depositedAtUtc
                      ? d(new Date(item.depositedAtUtc), { dateStyle: 'short' })
                      : '—',
                  })
                : undefined
            "
          >
            <template #append>
              <v-btn
                v-if="controlledCharacters.length && canWithdraw"
                size="small"
                variant="tonal"
                @click.stop="openWithdraw(item)"
              >
                {{ t('inventoryUi.storage.withdrawAction') }}
              </v-btn>
            </template>
          </ItemListRow>
        </v-list>
        <v-empty-state
          v-else
          icon="mdi-treasure-chest-outline"
          :text="t('inventoryUi.storage.emptyText')"
          :title="t('inventoryUi.storage.emptyTitle')"
        />
      </v-card>

      <v-expansion-panels v-if="storage.recentOperations.length" variant="accordion">
        <v-expansion-panel :title="t('inventoryUi.storage.journal')">
          <v-expansion-panel-text>
            <v-list>
              <v-list-item
                v-for="operation in storage.recentOperations"
                :key="`${operation.item.itemInstanceKey}:${operation.occurredAtUtc}`"
                :subtitle="
                  d(new Date(operation.occurredAtUtc), { dateStyle: 'medium', timeStyle: 'short' })
                "
                :title="
                  t(`inventoryUi.storage.operations.${operation.kind}`, {
                    character: operation.character?.name ?? '—',
                    item: operation.item.name,
                    quantity: operation.item.quantity,
                  })
                "
              />
            </v-list>
          </v-expansion-panel-text>
        </v-expansion-panel>
      </v-expansion-panels>
    </template>

    <v-dialog v-model="depositDialog" max-width="620" persistent>
      <v-card>
        <v-card-title>{{ t('inventoryUi.storage.depositTitle') }}</v-card-title>
        <v-card-text class="dialog-stack">
          <v-select
            v-if="controlledCharacters.length > 1"
            v-model="selectedCharacterId"
            item-title="name"
            item-value="id"
            :items="controlledCharacters"
            :label="t('inventoryUi.storage.character')"
          />
          <v-progress-linear v-if="isInventoryLoading" color="accent" indeterminate />
          <v-alert v-if="versionConflict" type="warning" variant="tonal">
            {{ t('tradeUi.gift.versionConflict') }}
          </v-alert>
          <v-alert v-for="message in dialogErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <v-radio-group v-if="depositableItems.length" v-model="selectedInventoryItemKey">
            <v-radio
              v-for="item in depositableItems"
              :key="item.itemInstanceKey"
              :label="`${item.revision.name} ×${item.quantity}`"
              :value="item.itemInstanceKey"
            />
          </v-radio-group>
          <v-alert
            v-else-if="selectedCharacterId && !isInventoryLoading && !dialogErrors.length"
            type="info"
            variant="tonal"
          >
            {{ t('inventoryUi.storage.noItems') }}
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isTransferring" variant="text" @click="depositDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            color="primary"
            :disabled="!selectedInventoryItem || isTransferring || versionConflict"
            :loading="isTransferring"
            @click="deposit"
          >
            {{ t('inventoryUi.storage.deposit') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="withdrawDialog" max-width="500" persistent>
      <v-card>
        <v-card-title>{{ t('inventoryUi.storage.withdrawTitle') }}</v-card-title>
        <v-card-text class="dialog-stack">
          <p v-if="selectedStorageItem">
            {{ selectedStorageItem.item.name }} ×{{ selectedStorageItem.item.quantity }}
          </p>
          <v-select
            v-if="controlledCharacters.length > 1"
            v-model="selectedCharacterId"
            item-title="name"
            item-value="id"
            :items="controlledCharacters"
            :label="t('inventoryUi.storage.character')"
          />
          <v-alert v-if="versionConflict" type="warning" variant="tonal">
            {{ t('tradeUi.gift.versionConflict') }}
          </v-alert>
          <v-alert v-for="message in dialogErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <v-alert type="info" variant="tonal">
            {{ t('inventoryUi.storage.withdrawConfirm') }}
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isTransferring" variant="text" @click="withdrawDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            color="primary"
            :disabled="!selectedCharacterId || isTransferring || versionConflict"
            :loading="isTransferring"
            @click="withdraw"
          >
            {{ t('inventoryUi.storage.withdraw') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </section>
</template>

<style scoped>
.storage-tab,
.dialog-stack {
  display: grid;
  gap: 16px;
}

.storage-header {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.storage-header h2,
.storage-header p {
  margin: 0;
}

.storage-panel {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}
</style>
