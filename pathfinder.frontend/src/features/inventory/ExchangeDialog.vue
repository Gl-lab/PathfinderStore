<script setup lang="ts">
import axios from 'axios'
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { getApiErrorMessages } from '@/api/errors'
import ItemListRow from '@/components/ItemListRow.vue'
import { createPartyExchange, getExchangeInventory, type InventoryOperationItem } from './api'
import { reconcileExchangeLines, toCreateExchangeLines, type ExchangeDraftLine } from './exchange'
import type { GiftRecipient } from './gift'

const props = defineProps<{
  campaignId: number
  sourceCharacterId: number
  sourceCharacterName: string
  sourceItems: InventoryOperationItem[]
  initialItem: InventoryOperationItem | null
  recipients: GiftRecipient[]
}>()
const emit = defineEmits<{
  sent: [exchangeKey: string]
}>()
const model = defineModel<boolean>({ required: true })
const { t } = useI18n()
const exchangeKey = ref('')
const counterpartyId = ref<number | null>(null)
const counterpartyItems = ref<InventoryOperationItem[]>([])
const lines = ref<ExchangeDraftLine[]>([])
const errors = ref<string[]>([])
const removedNames = ref<string[]>([])
const isLoadingCounterparty = ref(false)
const isSending = ref(false)

const sourceLines = computed(() =>
  lines.value.filter((line) => line.fromCharacterId === props.sourceCharacterId),
)
const counterpartyLines = computed(() =>
  lines.value.filter((line) => line.fromCharacterId === counterpartyId.value),
)
const counterparty = computed(
  () => props.recipients.find((item) => item.characterId === counterpartyId.value) ?? null,
)

watch(model, (isOpen) => {
  if (!isOpen) return
  exchangeKey.value = globalThis.crypto.randomUUID()
  counterpartyId.value = null
  counterpartyItems.value = []
  lines.value = props.initialItem ? [createLine(props.sourceCharacterId, props.initialItem)] : []
  errors.value = []
  removedNames.value = []
})

watch(counterpartyId, async (newId, oldId) => {
  if (!model.value || newId === oldId) return
  lines.value = lines.value.filter((line) => line.fromCharacterId === props.sourceCharacterId)
  counterpartyItems.value = []
  errors.value = []
  if (!newId) return
  await loadCounterparty(newId)
})

function createLine(fromCharacterId: number, item: InventoryOperationItem): ExchangeDraftLine {
  return {
    fromCharacterId,
    item,
    reservationOperationId: globalThis.crypto.randomUUID(),
  }
}

function isSelected(fromCharacterId: number, itemKey: string): boolean {
  return lines.value.some(
    (line) => line.fromCharacterId === fromCharacterId && line.item.itemInstanceKey === itemKey,
  )
}

function toggleLine(fromCharacterId: number, item: InventoryOperationItem): void {
  const existing = lines.value.findIndex(
    (line) =>
      line.fromCharacterId === fromCharacterId &&
      line.item.itemInstanceKey === item.itemInstanceKey,
  )
  if (existing >= 0) {
    lines.value.splice(existing, 1)
    return
  }

  lines.value.push(createLine(fromCharacterId, item))
}

async function loadCounterparty(characterId: number): Promise<void> {
  isLoadingCounterparty.value = true
  try {
    const result = await getExchangeInventory(
      props.campaignId,
      props.sourceCharacterId,
      characterId,
    )
    counterpartyItems.value = result.items
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    isLoadingCounterparty.value = false
  }
}

async function reconcileAfterConflict(): Promise<void> {
  if (!counterpartyId.value) return
  const [source, target] = await Promise.all([
    getExchangeInventory(props.campaignId, props.sourceCharacterId, props.sourceCharacterId),
    getExchangeInventory(props.campaignId, props.sourceCharacterId, counterpartyId.value),
  ])
  counterpartyItems.value = target.items
  const result = reconcileExchangeLines(
    lines.value,
    new Map([
      [props.sourceCharacterId, source.items],
      [counterpartyId.value, target.items],
    ]),
  )
  lines.value = result.lines
  removedNames.value = result.removedNames
}

async function send(): Promise<void> {
  if (!counterpartyId.value || lines.value.length === 0 || isSending.value) return
  isSending.value = true
  errors.value = []
  removedNames.value = []
  try {
    await createPartyExchange(props.campaignId, {
      exchangeKey: exchangeKey.value,
      initiatorCharacterId: props.sourceCharacterId,
      counterpartyCharacterId: counterpartyId.value,
      lines: toCreateExchangeLines(lines.value),
    })
    emit('sent', exchangeKey.value)
    model.value = false
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 400) {
      try {
        await reconcileAfterConflict()
      } catch {
        errors.value = getApiErrorMessages(error)
      }
    }
    if (removedNames.value.length === 0) {
      errors.value = getApiErrorMessages(error)
      if (axios.isAxiosError(error) && error.response && error.response.status < 500) {
        exchangeKey.value = globalThis.crypto.randomUUID()
      }
    }
  } finally {
    isSending.value = false
  }
}
</script>

<template>
  <v-dialog v-model="model" fullscreen persistent transition="dialog-bottom-transition">
    <v-card>
      <v-toolbar color="surface">
        <v-btn icon="mdi-close" :disabled="isSending" @click="model = false" />
        <v-toolbar-title>{{ t('tradeUi.exchange.title') }}</v-toolbar-title>
        <v-spacer />
        <v-btn
          color="primary"
          :disabled="!counterpartyId || lines.length === 0 || isSending"
          :loading="isSending"
          variant="flat"
          @click="send"
        >
          {{ t('tradeUi.exchange.send') }}
        </v-btn>
      </v-toolbar>

      <v-card-text class="exchange-dialog">
        <div class="exchange-heading">
          <strong>{{ sourceCharacterName }}</strong>
          <v-icon icon="mdi-swap-horizontal" />
          <v-select
            v-model="counterpartyId"
            class="counterparty-select"
            hide-details
            item-title="name"
            item-value="characterId"
            :items="recipients"
            :label="t('tradeUi.exchange.counterparty')"
          />
        </div>

        <v-alert v-for="name in removedNames" :key="name" type="warning" variant="tonal">
          {{ t('tradeUi.exchange.lineChanged', { name }) }}
        </v-alert>
        <v-alert v-for="message in errors" :key="message" type="error" variant="tonal">
          {{ message }}
        </v-alert>

        <div class="exchange-columns">
          <v-card variant="outlined">
            <v-card-title>{{ t('tradeUi.exchange.youGive') }}</v-card-title>
            <v-card-subtitle>{{ sourceCharacterName }}</v-card-subtitle>
            <v-card-text class="item-stack">
              <ItemListRow
                v-for="item in sourceItems"
                :key="item.itemInstanceKey"
                :bulk-tenths="item.bulkTenths * item.quantity"
                :category="item.primaryCategory"
                :name="item.name"
                :quantity="item.quantity"
                :selected="isSelected(sourceCharacterId, item.itemInstanceKey)"
                @click="toggleLine(sourceCharacterId, item)"
              >
                <template #append>
                  <v-checkbox-btn
                    :model-value="isSelected(sourceCharacterId, item.itemInstanceKey)"
                    @click.stop="toggleLine(sourceCharacterId, item)"
                  />
                </template>
              </ItemListRow>
              <v-empty-state
                v-if="sourceItems.length === 0"
                :title="t('tradeUi.exchange.noAvailableItems')"
              />
            </v-card-text>
          </v-card>

          <v-card variant="outlined">
            <v-card-title>{{ t('tradeUi.exchange.youReceive') }}</v-card-title>
            <v-card-subtitle>
              {{ counterparty?.name ?? t('tradeUi.exchange.selectCounterparty') }}
            </v-card-subtitle>
            <v-progress-linear v-if="isLoadingCounterparty" indeterminate />
            <v-card-text class="item-stack">
              <ItemListRow
                v-for="item in counterpartyItems"
                :key="item.itemInstanceKey"
                :bulk-tenths="item.bulkTenths * item.quantity"
                :category="item.primaryCategory"
                :name="item.name"
                :quantity="item.quantity"
                :selected="isSelected(counterpartyId ?? 0, item.itemInstanceKey)"
                @click="counterpartyId && toggleLine(counterpartyId, item)"
              >
                <template #append>
                  <v-checkbox-btn
                    :model-value="isSelected(counterpartyId ?? 0, item.itemInstanceKey)"
                    @click.stop="counterpartyId && toggleLine(counterpartyId, item)"
                  />
                </template>
              </ItemListRow>
              <v-empty-state
                v-if="counterpartyId && !isLoadingCounterparty && counterpartyItems.length === 0"
                :title="t('tradeUi.exchange.noAvailableItems')"
              />
            </v-card-text>
          </v-card>
        </div>

        <v-alert
          v-if="counterpartyId && sourceLines.length > 0 && counterpartyLines.length === 0"
          type="info"
          variant="tonal"
        >
          {{ t('tradeUi.exchange.oneSidedHint') }}
        </v-alert>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<style scoped>
.exchange-dialog,
.item-stack {
  display: grid;
  gap: 16px;
}

.exchange-heading {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
}

.counterparty-select {
  max-width: 360px;
}

.exchange-columns {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

@media (max-width: 799px) {
  .exchange-heading {
    align-items: stretch;
    flex-direction: column;
  }

  .exchange-columns {
    grid-template-columns: 1fr;
  }
}
</style>
