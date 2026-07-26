<script setup lang="ts">
import axios from 'axios'
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import BulkMeter from '@/components/BulkMeter.vue'
import CountdownChip from '@/components/CountdownChip.vue'
import ItemListRow from '@/components/ItemListRow.vue'
import MoneyText from '@/components/MoneyText.vue'
import OperationStatusChip from '@/components/OperationStatusChip.vue'
import { getCampaigns, type Campaign } from '@/features/campaigns/api'
import { getWallet, type Wallet } from '@/features/commerce/api'
import {
  getCharacterInventory,
  getPartyExchanges,
  getPartyGifts,
  type CharacterInventory,
  type CharacterInventoryItem,
  type ItemCategory,
  type PartyExchange,
  type PartyGift,
} from '@/features/inventory/api'
import { formatBulk } from '@/features/inventory/bulk'
import {
  getCampaignCharacter,
  type Character,
  type CampaignCharacter,
} from '@/features/characters/api'

interface ItemCategoryGroup {
  category: ItemCategory
  items: CharacterInventoryItem[]
}

const route = useRoute()
const { d, t, te } = useI18n()
const campaignId = Number(route.params.campaignId)
const characterId = Number(route.params.characterId)
const campaign = ref<Campaign | null>(null)
const character = ref<Character | null>(null)
const inventory = ref<CharacterInventory | null>(null)
const wallet = ref<Wallet | null>(null)
const incomingGifts = ref<PartyGift[]>([])
const pendingExchanges = ref<PartyExchange[]>([])
const selectedItemKey = ref<string | null>(null)
const contextErrors = ref<string[]>([])
const inventoryErrors = ref<string[]>([])
const walletErrors = ref<string[]>([])
const pendingErrors = ref<string[]>([])
const isContextLoading = ref(true)
const isInventoryLoading = ref(true)
const isWalletLoading = ref(false)
const isPendingLoading = ref(false)
const inventoryNotMigrated = ref(false)
const walletDialog = ref(false)
const incomingExpanded = ref(false)

const selectedItem = computed(
  () =>
    inventory.value?.items.find((item) => item.itemInstanceKey === selectedItemKey.value) ?? null,
)
const equippedGroups = computed(() =>
  groupItems(inventory.value?.items.filter((item) => item.isEquipped) ?? []),
)
const backpackGroups = computed(() =>
  groupItems(inventory.value?.items.filter((item) => !item.isEquipped) ?? []),
)
const incomingCount = computed(() => incomingGifts.value.length + pendingExchanges.value.length)
const characterSubtitle = computed(() => {
  const parts = [
    character.value?.classPackage?.name,
    character.value ? t('inventoryUi.level', { level: 1 }) : null,
    campaign.value?.parties.find((party) =>
      party.characters.some((item) => item.characterId === characterId),
    )?.name,
  ]
  return parts.filter(Boolean).join(' · ')
})

function groupItems(items: CharacterInventoryItem[]): ItemCategoryGroup[] {
  const groups = new Map<ItemCategory, CharacterInventoryItem[]>()
  for (const item of items) {
    const group = groups.get(item.revision.primaryCategory) ?? []
    group.push(item)
    groups.set(item.revision.primaryCategory, group)
  }

  return Array.from(groups, ([category, groupItems]) => ({
    category,
    items: groupItems.sort((left, right) => left.revision.name.localeCompare(right.revision.name)),
  }))
}

function selectItem(item: CharacterInventoryItem): void {
  selectedItemKey.value =
    selectedItemKey.value === item.itemInstanceKey ? null : item.itemInstanceKey
}

function localizedValue(namespace: string, value: string): string {
  const key = `${namespace}.${value}`
  return te(key) ? t(key) : value
}

async function loadContext(): Promise<void> {
  isContextLoading.value = true
  contextErrors.value = []
  try {
    const [campaigns, campaignCharacter] = await Promise.all([
      getCampaigns(),
      getCampaignCharacter(campaignId, characterId),
    ])
    campaign.value = campaigns.find((item) => item.id === campaignId) ?? null
    character.value = (campaignCharacter as CampaignCharacter).character
  } catch (error) {
    contextErrors.value = getApiErrorMessages(error)
  } finally {
    isContextLoading.value = false
  }
}

async function loadInventory(): Promise<void> {
  isInventoryLoading.value = true
  inventoryErrors.value = []
  inventoryNotMigrated.value = false
  try {
    inventory.value = await getCharacterInventory(campaignId, characterId)
    if (selectedItemKey.value && !selectedItem.value) selectedItemKey.value = null

    if (!inventory.value.isReadOnly) {
      void Promise.all([loadWallet(), loadPending()])
    }
  } catch (error) {
    inventory.value = null
    if (axios.isAxiosError(error) && error.response?.status === 404) {
      inventoryNotMigrated.value = true
    } else {
      inventoryErrors.value = getApiErrorMessages(error)
    }
  } finally {
    isInventoryLoading.value = false
  }
}

async function loadWallet(): Promise<void> {
  isWalletLoading.value = true
  walletErrors.value = []
  try {
    wallet.value = await getWallet(campaignId, characterId)
  } catch (error) {
    walletErrors.value = getApiErrorMessages(error)
  } finally {
    isWalletLoading.value = false
  }
}

async function loadPending(): Promise<void> {
  isPendingLoading.value = true
  pendingErrors.value = []
  try {
    const [gifts, exchanges] = await Promise.all([
      getPartyGifts(campaignId, characterId, 'Incoming'),
      getPartyExchanges(campaignId, characterId),
    ])
    incomingGifts.value = gifts
    pendingExchanges.value = exchanges
  } catch (error) {
    pendingErrors.value = getApiErrorMessages(error)
  } finally {
    isPendingLoading.value = false
  }
}

onMounted(() => {
  void Promise.all([loadContext(), loadInventory()])
})
</script>

<template>
  <section class="inventory-view">
    <v-btn
      prepend-icon="mdi-arrow-left"
      variant="text"
      :to="{ name: 'campaign-details', params: { campaignId }, query: { tab: 'party' } }"
    >
      {{ t('inventoryUi.backToCampaign') }}
    </v-btn>

    <v-progress-linear v-if="isContextLoading" color="accent" indeterminate rounded />
    <v-alert v-for="message in contextErrors" :key="message" type="error" variant="tonal">
      {{ message }}
      <template #append>
        <v-btn variant="text" @click="loadContext">{{ t('common.retry') }}</v-btn>
      </template>
    </v-alert>

    <header v-if="character" class="inventory-header">
      <div>
        <p class="eyebrow">
          {{ t('inventoryUi.campaignEyebrow', { name: campaign?.name ?? `#${campaignId}` }) }}
        </p>
        <h1>{{ t('inventoryUi.title', { name: character.name }) }}</h1>
        <p class="lead">{{ characterSubtitle }}</p>
      </div>
      <v-btn
        v-if="inventory && !inventory.isReadOnly"
        :loading="isPendingLoading"
        prepend-icon="mdi-inbox-arrow-down"
        variant="tonal"
        @click="incomingExpanded = !incomingExpanded"
      >
        {{ t('inventoryUi.incoming', { count: incomingCount }) }}
      </v-btn>
    </header>

    <v-alert v-if="inventory?.isReadOnly" type="info" variant="tonal">
      {{ t('inventoryUi.gameMasterReadOnly') }}
    </v-alert>

    <v-expand-transition>
      <v-card v-if="incomingExpanded && !inventory?.isReadOnly" class="panel" elevation="0">
        <v-card-title class="panel-title">
          {{ t('inventoryUi.incomingTitle') }}
          <v-btn
            icon="mdi-refresh"
            :loading="isPendingLoading"
            size="small"
            variant="text"
            @click="loadPending"
          />
        </v-card-title>
        <v-card-text class="stack">
          <v-alert v-for="message in pendingErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <div v-for="gift in incomingGifts" :key="gift.gift.giftKey" class="operation-row">
            <div>
              <strong>{{ gift.item.name }}</strong>
              <p>
                {{
                  t('inventoryUi.giftFrom', {
                    name: gift.sourceCharacter.name,
                    quantity: gift.item.quantity,
                  })
                }}
              </p>
            </div>
            <OperationStatusChip status="Pending" />
            <CountdownChip :expires-at-utc="gift.gift.expiresAtUtc" @expired="loadPending" />
          </div>
          <div
            v-for="exchange in pendingExchanges"
            :key="exchange.exchange.exchangeKey"
            class="operation-row"
          >
            <div>
              <strong>{{ t('inventoryUi.exchange') }}</strong>
              <p>{{ exchange.initiatorCharacter.name }}</p>
            </div>
            <OperationStatusChip status="Pending" />
            <CountdownChip
              :expires-at-utc="exchange.exchange.expiresAtUtc"
              @expired="loadPending"
            />
          </div>
          <p v-if="!isPendingLoading && incomingCount === 0">
            {{ t('inventoryUi.noIncoming') }}
          </p>
        </v-card-text>
      </v-card>
    </v-expand-transition>

    <v-progress-linear v-if="isInventoryLoading" color="accent" indeterminate rounded />
    <v-alert v-if="inventoryNotMigrated" type="info" variant="tonal">
      {{ t('inventoryUi.notMigrated') }}
    </v-alert>
    <v-alert v-for="message in inventoryErrors" :key="message" type="error" variant="tonal">
      {{ message }}
      <template #append>
        <v-btn variant="text" @click="loadInventory">{{ t('common.retry') }}</v-btn>
      </template>
    </v-alert>

    <div v-if="inventory" class="inventory-grid">
      <aside class="summary-column">
        <v-card v-if="!inventory.isReadOnly" class="panel" elevation="0">
          <v-card-title>{{ t('inventoryUi.wallet.title') }}</v-card-title>
          <v-card-text>
            <v-progress-linear v-if="isWalletLoading" color="accent" indeterminate />
            <v-alert
              v-for="message in walletErrors"
              :key="message"
              density="compact"
              type="error"
              variant="tonal"
            >
              {{ message }}
              <template #append>
                <v-btn size="small" variant="text" @click="loadWallet">
                  {{ t('common.retry') }}
                </v-btn>
              </template>
            </v-alert>
            <template v-if="wallet">
              <MoneyText class="wallet-amount" :copper="wallet.availableCopper" />
              <dl class="wallet-breakdown">
                <div>
                  <dt>{{ t('inventoryUi.wallet.balance') }}</dt>
                  <dd><MoneyText :copper="wallet.balanceCopper" compact /></dd>
                </div>
                <div>
                  <dt>{{ t('inventoryUi.wallet.reserved') }}</dt>
                  <dd><MoneyText :copper="wallet.reservedCopper" compact /></dd>
                </div>
              </dl>
              <v-btn block variant="text" @click="walletDialog = true">
                {{ t('inventoryUi.wallet.ledger') }}
              </v-btn>
            </template>
          </v-card-text>
        </v-card>

        <v-card class="panel" elevation="0">
          <v-card-title>{{ t('inventoryUi.bulk.title') }}</v-card-title>
          <v-card-text>
            <BulkMeter
              :total-tenths="inventory.bulk.totalTenths"
              :encumbered-tenths="inventory.bulk.encumberedAtTenths"
              :maximum-tenths="inventory.bulk.maximumTenths"
            />
          </v-card-text>
        </v-card>
      </aside>

      <v-card class="panel items-panel" elevation="0">
        <v-card-title>{{ t('inventoryUi.items.title') }}</v-card-title>
        <v-card-text>
          <template v-if="inventory.items.length">
            <section v-if="equippedGroups.length">
              <h2>{{ t('inventoryUi.items.equipped') }}</h2>
              <div v-for="group in equippedGroups" :key="group.category">
                <h3>{{ t(`inventoryUi.categories.${group.category}`) }}</h3>
                <v-list>
                  <ItemListRow
                    v-for="item in group.items"
                    :key="item.itemInstanceKey"
                    :bulk-tenths="item.revision.bulkTenths * item.quantity"
                    :category="item.revision.primaryCategory"
                    :name="item.revision.name"
                    :quantity="item.quantity"
                    :selected="selectedItemKey === item.itemInstanceKey"
                    @click="selectItem(item)"
                  />
                </v-list>
              </div>
            </section>
            <section v-if="backpackGroups.length">
              <h2>{{ t('inventoryUi.items.backpack') }}</h2>
              <div v-for="group in backpackGroups" :key="group.category">
                <h3>{{ t(`inventoryUi.categories.${group.category}`) }}</h3>
                <v-list>
                  <ItemListRow
                    v-for="item in group.items"
                    :key="item.itemInstanceKey"
                    :bulk-tenths="item.revision.bulkTenths * item.quantity"
                    :category="item.revision.primaryCategory"
                    :name="item.revision.name"
                    :quantity="item.quantity"
                    :selected="selectedItemKey === item.itemInstanceKey"
                    @click="selectItem(item)"
                  />
                </v-list>
              </div>
            </section>
          </template>
          <v-empty-state
            v-else
            icon="mdi-bag-personal-outline"
            :text="t('inventoryUi.items.emptyText')"
            :title="t('inventoryUi.items.emptyTitle')"
          />
        </v-card-text>
      </v-card>

      <v-card class="panel details-panel" elevation="0">
        <v-card-title>{{ t('inventoryUi.details.title') }}</v-card-title>
        <v-card-text v-if="selectedItem" class="stack">
          <h2>{{ selectedItem.revision.name }}</h2>
          <div class="chips">
            <v-chip size="small">{{
              t('inventoryUi.level', { level: selectedItem.revision.level })
            }}</v-chip>
            <v-chip size="small">{{
              t(`inventoryUi.categories.${selectedItem.revision.primaryCategory}`)
            }}</v-chip>
            <v-chip size="small">
              {{
                t('inventoryUi.details.revision', { number: selectedItem.revision.revisionNumber })
              }}
            </v-chip>
          </div>
          <p>{{ selectedItem.revision.description || t('inventoryUi.details.noDescription') }}</p>
          <dl class="item-facts">
            <div>
              <dt>{{ t('inventoryUi.details.catalogPrice') }}</dt>
              <dd><MoneyText :copper="selectedItem.revision.priceInCopperPieces" /></dd>
            </div>
            <div>
              <dt>{{ t('inventoryUi.bulk.title') }}</dt>
              <dd>{{ formatBulk(selectedItem.revision.bulkTenths) }} Bulk</dd>
            </div>
            <div v-if="selectedItem.provenance">
              <dt>{{ t('inventoryUi.details.provenance') }}</dt>
              <dd>
                {{ localizedValue('inventoryUi.provenance', selectedItem.provenance.kind) }} ·
                {{ d(new Date(selectedItem.provenance.occurredAtUtc), { dateStyle: 'short' }) }}
              </dd>
            </div>
          </dl>
        </v-card-text>
        <v-card-text v-else>{{ t('inventoryUi.details.empty') }}</v-card-text>
      </v-card>
    </div>

    <v-dialog v-model="walletDialog" max-width="720">
      <v-card>
        <v-card-title>{{ t('inventoryUi.wallet.ledgerTitle') }}</v-card-title>
        <v-card-text>
          <v-list v-if="wallet?.entries.length">
            <v-list-item v-for="entry in wallet.entries" :key="entry.operationId">
              <v-list-item-title>{{ entry.description }}</v-list-item-title>
              <v-list-item-subtitle>
                {{
                  d(new Date(entry.occurredAtUtc), {
                    dateStyle: 'medium',
                    timeStyle: 'short',
                  })
                }}
                · {{ localizedValue('commerceUi.walletTransactionKinds', entry.kind) }} ·
                {{ t('inventoryUi.wallet.balanceAfter') }}
                <MoneyText :copper="entry.balanceAfterCopper" compact />
              </v-list-item-subtitle>
              <template #append>
                <span>{{ entry.amountCopper >= 0 ? '+' : '−' }}</span>
                <MoneyText :copper="Math.abs(entry.amountCopper)" />
              </template>
            </v-list-item>
          </v-list>
          <v-empty-state v-else :title="t('inventoryUi.wallet.emptyLedger')" />
        </v-card-text>
      </v-card>
    </v-dialog>
  </section>
</template>

<style scoped>
.inventory-view,
.summary-column,
.stack {
  display: grid;
  gap: 16px;
}

.inventory-header,
.panel-title,
.operation-row,
.chips {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px;
}

.inventory-header,
.panel-title,
.operation-row {
  justify-content: space-between;
}

.eyebrow {
  margin: 0 0 8px;
  color: rgb(var(--v-theme-secondary));
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

h1 {
  margin: 0;
  color: rgb(var(--v-theme-primary));
  font-family: Georgia, 'Times New Roman', serif;
  font-size: clamp(2rem, 5vw, 3rem);
}

.lead,
.operation-row p {
  margin: 4px 0 0;
  color: rgb(var(--v-theme-on-surface-variant));
}

.panel {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}

.inventory-grid {
  display: grid;
  grid-template-columns: 230px minmax(0, 1fr) 260px;
  gap: 16px;
  align-items: start;
}

.wallet-amount {
  margin-bottom: 16px;
  font-size: 1.5rem;
  font-weight: 700;
}

.wallet-breakdown,
.item-facts {
  display: grid;
  gap: 8px;
  margin: 0;
}

.wallet-breakdown div,
.item-facts div {
  display: flex;
  justify-content: space-between;
  gap: 12px;
}

.wallet-breakdown dd,
.item-facts dd {
  margin: 0;
  text-align: right;
}

.items-panel h2,
.details-panel h2 {
  margin: 8px 0;
  font-family: Georgia, 'Times New Roman', serif;
}

.items-panel h3 {
  margin: 16px 16px 4px;
  color: rgb(var(--v-theme-on-surface-variant));
  font-size: 0.75rem;
  letter-spacing: 0.06em;
  text-transform: uppercase;
}

@media (max-width: 959px) {
  .inventory-grid {
    grid-template-columns: 1fr;
  }
}
</style>
