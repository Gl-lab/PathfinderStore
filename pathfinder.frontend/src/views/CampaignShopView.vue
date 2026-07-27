<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import CountdownChip from '@/components/CountdownChip.vue'
import ItemListRow from '@/components/ItemListRow.vue'
import MoneyText from '@/components/MoneyText.vue'
import OperationStatusChip from '@/components/OperationStatusChip.vue'
import { useSnackbar } from '@/composables/useSnackbar'
import {
  cancelPurchaseReservation,
  completePurchase,
  getPurchaseReservations,
  getSellQuote,
  getSettlements,
  getShopOffers,
  getWallet,
  reservePurchase,
  sellItem,
  type PurchaseReservation,
  type SellQuote,
  type Settlement,
  type Shop,
  type ShopOffer,
  type Wallet,
} from '@/features/commerce/api'
import {
  availableOfferQuantity,
  maxAffordableQuantity,
  purchaseShortfall,
} from '@/features/commerce/shop'
import {
  getCampaignCharacters,
  getCampaigns,
  type Campaign,
  type CampaignCharacterReference,
} from '@/features/campaigns/api'
import {
  getCharacterInventory,
  type CharacterInventory,
  type CharacterInventoryItem,
  type OperationStatus,
} from '@/features/inventory/api'

type ShopTab = 'offers' | 'sell' | 'reservations'

const allowedTabs: ShopTab[] = ['offers', 'sell', 'reservations']
const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const snackbar = useSnackbar()
const campaignId = computed(() => Number(route.params.campaignId))
const shopId = computed(() => Number(route.params.shopId))
const campaign = ref<Campaign | null>(null)
const settlement = ref<Settlement | null>(null)
const shop = ref<Shop | null>(null)
const characters = ref<CampaignCharacterReference[]>([])
const selectedCharacterId = ref<number | null>(null)
const offers = ref<ShopOffer[]>([])
const wallet = ref<Wallet | null>(null)
const reservations = ref<PurchaseReservation[]>([])
const inventory = ref<CharacterInventory | null>(null)
const quantities = ref<Record<string, number>>({})
const reservationOperationIds = ref<Record<string, string>>({})
const finalOperationIds = ref<Record<string, string>>({})
const quotes = ref<Record<string, SellQuote>>({})
const quoteErrors = ref<Record<string, string[]>>({})
const sellCandidate = ref<CharacterInventoryItem | null>(null)
const errors = ref<string[]>([])
const actionErrors = ref<string[]>([])
const isLoading = ref(true)
const actionKey = ref<string | null>(null)
const activeTab = computed<ShopTab>({
  get: () => {
    const requested = String(route.query.tab ?? 'offers') as ShopTab
    return allowedTabs.includes(requested) ? requested : 'offers'
  },
  set: (value) => {
    void router.replace({ query: { ...route.query, tab: value } })
  },
})
const controlledCharacters = computed(() => {
  const partyCharacters =
    campaign.value?.parties
      .find((party) => party.status === 'Active')
      ?.characters.filter(
        (character) => character.controlledByUserId === campaign.value?.currentUserId,
      ) ?? []
  return partyCharacters.map((character) => ({
    id: character.characterId,
    name:
      characters.value.find((item) => item.id === character.characterId)?.name ??
      `#${character.characterId}`,
  }))
})
const activeReservation = computed(
  () => reservations.value.find((reservation) => reservation.status === 'Active') ?? null,
)

function quantityFor(offer: ShopOffer): number {
  return quantities.value[offer.offerKey] ?? 1
}

function setQuantity(offerKey: string, value: unknown): void {
  quantities.value[offerKey] = Math.max(1, Number(value) || 1)
}

function operationId(
  store: { value: Record<string, string> },
  key: string,
): string {
  const existing = store.value[key]
  if (existing) return existing

  const created = globalThis.crypto.randomUUID()
  store.value[key] = created
  return created
}

function reservationStatus(status: PurchaseReservation['status']): OperationStatus {
  return status === 'Active' ? 'Reserved' : status
}

async function loadBase(): Promise<void> {
  isLoading.value = true
  errors.value = []
  try {
    const [campaignItems, characterItems, settlementItems] = await Promise.all([
      getCampaigns(),
      getCampaignCharacters(),
      getSettlements(campaignId.value),
    ])
    campaign.value = campaignItems.find((item) => item.id === campaignId.value) ?? null
    characters.value = characterItems
    settlement.value =
      settlementItems.find((item) => item.shops.some((candidate) => candidate.id === shopId.value)) ??
      null
    shop.value =
      settlement.value?.shops.find((candidate) => candidate.id === shopId.value) ?? null
    if (!campaign.value || !shop.value) {
      await router.replace({
        name: 'campaign-details',
        params: { campaignId: campaignId.value },
        query: { tab: 'commerce' },
      })
      return
    }

    const requestedCharacterId = Number(route.query.characterId)
    const defaultCharacter =
      controlledCharacters.value.find((item) => item.id === requestedCharacterId) ??
      controlledCharacters.value[0]
    selectedCharacterId.value = defaultCharacter?.id ?? null
    await loadCharacterData()
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    isLoading.value = false
  }
}

async function loadCharacterData(): Promise<void> {
  actionErrors.value = []
  const characterId = selectedCharacterId.value
  if (!characterId) {
    offers.value = await getShopOffers(campaignId.value, shopId.value)
    wallet.value = null
    reservations.value = []
    inventory.value = null
    return
  }

  const [offerItems, walletItem, reservationItems, inventoryItem] = await Promise.all([
    getShopOffers(campaignId.value, shopId.value),
    getWallet(campaignId.value, characterId),
    getPurchaseReservations(campaignId.value, characterId),
    getCharacterInventory(campaignId.value, characterId),
  ])
  offers.value = offerItems
  wallet.value = walletItem
  reservations.value = reservationItems
  inventory.value = inventoryItem
  offers.value.forEach((offer) => {
    quantities.value[offer.offerKey] = Math.max(
      1,
      Math.min(quantityFor(offer), Math.max(1, availableOfferQuantity(offer))),
    )
  })
}

async function refreshAfterPurchase(): Promise<void> {
  const characterId = selectedCharacterId.value
  if (!characterId) return

  const [offerItems, walletItem, reservationItems, inventoryItem] = await Promise.all([
    getShopOffers(campaignId.value, shopId.value),
    getWallet(campaignId.value, characterId),
    getPurchaseReservations(campaignId.value, characterId),
    getCharacterInventory(campaignId.value, characterId),
  ])
  offers.value = offerItems
  wallet.value = walletItem
  reservations.value = reservationItems
  inventory.value = inventoryItem
}

async function selectCharacter(characterId: number | null): Promise<void> {
  selectedCharacterId.value = characterId
  quotes.value = {}
  quoteErrors.value = {}
  await router.replace({
    query: { ...route.query, characterId: characterId?.toString() },
  })
  try {
    await loadCharacterData()
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
  }
}

async function reserve(offer: ShopOffer): Promise<void> {
  if (!selectedCharacterId.value) return
  actionKey.value = `reserve:${offer.offerKey}`
  actionErrors.value = []
  try {
    await reservePurchase(campaignId.value, {
      operationId: operationId(reservationOperationIds, offer.offerKey),
      offerKey: offer.offerKey,
      buyerCharacterId: selectedCharacterId.value,
      quantity: quantityFor(offer),
    })
    delete reservationOperationIds.value[offer.offerKey]
    await refreshAfterPurchase()
    snackbar.success(t('commerceUi.shop.reserved', { item: offer.itemName }))
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
    await refreshAfterPurchase()
  } finally {
    actionKey.value = null
  }
}

async function repeatReservation(reservation: PurchaseReservation): Promise<void> {
  const offer = offers.value.find((item) => item.offerKey === reservation.offerKey)
  if (!offer) return

  quantities.value[offer.offerKey] = reservation.quantity
  delete reservationOperationIds.value[offer.offerKey]
  await reserve(offer)
}

async function complete(reservation: PurchaseReservation): Promise<void> {
  actionKey.value = `complete:${reservation.reservationKey}`
  actionErrors.value = []
  try {
    await completePurchase(
      campaignId.value,
      reservation.reservationKey,
      operationId(finalOperationIds, `complete:${reservation.reservationKey}`),
    )
    delete finalOperationIds.value[`complete:${reservation.reservationKey}`]
    await refreshAfterPurchase()
    snackbar.success(t('commerceUi.shop.purchased', { item: reservation.itemName }))
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
    await refreshAfterPurchase()
  } finally {
    actionKey.value = null
  }
}

async function cancel(reservation: PurchaseReservation): Promise<void> {
  actionKey.value = `cancel:${reservation.reservationKey}`
  actionErrors.value = []
  try {
    await cancelPurchaseReservation(
      campaignId.value,
      reservation.reservationKey,
      operationId(finalOperationIds, `cancel:${reservation.reservationKey}`),
    )
    delete finalOperationIds.value[`cancel:${reservation.reservationKey}`]
    await refreshAfterPurchase()
    snackbar.info(t('commerceUi.shop.cancelled'))
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
    await refreshAfterPurchase()
  } finally {
    actionKey.value = null
  }
}

async function quote(item: CharacterInventoryItem): Promise<void> {
  if (!selectedCharacterId.value) return
  actionKey.value = `quote:${item.itemInstanceKey}`
  quoteErrors.value[item.itemInstanceKey] = []
  try {
    quotes.value[item.itemInstanceKey] = await getSellQuote(
      campaignId.value,
      shopId.value,
      selectedCharacterId.value,
      item.itemInstanceKey,
    )
  } catch (error) {
    quoteErrors.value[item.itemInstanceKey] = getApiErrorMessages(error)
  } finally {
    actionKey.value = null
  }
}

async function confirmSale(): Promise<void> {
  const item = sellCandidate.value
  const characterId = selectedCharacterId.value
  if (!item || !characterId) return

  actionKey.value = `sell:${item.itemInstanceKey}`
  actionErrors.value = []
  try {
    await sellItem(campaignId.value, shopId.value, {
      operationId: operationId(finalOperationIds, `sell:${item.itemInstanceKey}`),
      sellerCharacterId: characterId,
      itemInstanceKey: item.itemInstanceKey,
    })
    delete finalOperationIds.value[`sell:${item.itemInstanceKey}`]
    sellCandidate.value = null
    await refreshAfterPurchase()
    snackbar.success(t('commerceUi.shop.sold'))
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
  } finally {
    actionKey.value = null
  }
}

watch([campaignId, shopId], loadBase)
onMounted(loadBase)
</script>

<template>
  <section class="shop-page">
    <v-progress-linear v-if="isLoading" color="accent" indeterminate rounded />
    <v-alert v-for="message in errors" :key="message" type="error" variant="tonal">
      {{ message }}
      <template #append>
        <v-btn variant="text" @click="loadBase">{{ t('common.retry') }}</v-btn>
      </template>
    </v-alert>

    <template v-if="campaign && shop">
      <v-btn
        :to="{
          name: 'campaign-details',
          params: { campaignId },
          query: { tab: 'commerce' },
        }"
        prepend-icon="mdi-arrow-left"
        variant="text"
      >
        {{ t('commerceUi.shop.back') }}
      </v-btn>

      <header class="shop-header">
        <div>
          <p class="eyebrow">
            {{ t('commerceUi.shop.eyebrow', { settlement: settlement?.name }) }}
          </p>
          <h1>{{ shop.name }}</h1>
          <v-chip size="small" variant="tonal">
            {{
              t('commerceUi.shop.policy', {
                buy: shop.catalogPricePercent,
                sell: shop.buybackPricePercent,
              })
            }}
          </v-chip>
        </div>
        <v-select
          :model-value="selectedCharacterId"
          class="character-select"
          density="compact"
          hide-details
          item-title="name"
          item-value="id"
          :items="controlledCharacters"
          :label="t('commerceUi.shop.character')"
          @update:model-value="selectCharacter"
        />
      </header>

      <v-alert v-if="!controlledCharacters.length" type="warning" variant="tonal">
        {{ t('commerceUi.shop.noCharacter') }}
      </v-alert>
      <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
        {{ message }}
      </v-alert>

      <div v-if="selectedCharacterId" class="shop-layout">
        <main>
          <v-tabs v-model="activeTab" color="primary" show-arrows>
            <v-tab value="offers">{{ t('commerceUi.shop.tabs.offers') }}</v-tab>
            <v-tab value="sell">{{ t('commerceUi.shop.tabs.sell') }}</v-tab>
            <v-tab value="reservations">
              {{ t('commerceUi.shop.tabs.reservations') }}
              <v-badge
                v-if="reservations.filter((item) => item.status === 'Active').length"
                color="secondary"
                inline
                :content="reservations.filter((item) => item.status === 'Active').length"
              />
            </v-tab>
          </v-tabs>

          <v-window v-model="activeTab">
            <v-window-item value="offers">
              <v-alert v-if="!offers.length" class="tab-content" type="info" variant="tonal">
                {{ t('commerceUi.shop.emptyOffers') }}
              </v-alert>
              <v-list v-else class="tab-content offer-list" bg-color="transparent">
                <v-list-item
                  v-for="offer in offers"
                  :key="offer.offerKey"
                  class="offer-row"
                  prepend-icon="mdi-package-variant"
                  rounded="lg"
                >
                  <v-list-item-title>{{ offer.itemName }}</v-list-item-title>
                  <v-list-item-subtitle>
                    {{ t('commerceUi.shop.level', { level: offer.itemLevel }) }} ·
                    {{ t(`commerceUi.shop.offerKinds.${offer.kind}`) }} ·
                    {{
                      t('commerceUi.shop.available', {
                        count: availableOfferQuantity(offer),
                      })
                    }}
                  </v-list-item-subtitle>
                  <template #append>
                    <div class="offer-actions">
                      <MoneyText :copper="offer.unitPriceCopper" />
                      <v-number-input
                        v-if="offer.kind === 'Catalog'"
                        control-variant="split"
                        density="compact"
                        hide-details
                        :max="Math.max(1, availableOfferQuantity(offer))"
                        :min="1"
                        :model-value="quantityFor(offer)"
                        @update:model-value="setQuantity(offer.offerKey, $event)"
                      />
                      <v-btn
                        color="primary"
                        :disabled="
                          quantityFor(offer) > maxAffordableQuantity(offer, wallet) ||
                          availableOfferQuantity(offer) === 0
                        "
                        :loading="actionKey === `reserve:${offer.offerKey}`"
                        @click="reserve(offer)"
                      >
                        {{ t('commerceUi.shop.buy') }}
                      </v-btn>
                    </div>
                  </template>
                  <v-alert
                    v-if="purchaseShortfall(offer, quantityFor(offer), wallet) > 0"
                    class="shortfall"
                    type="warning"
                    variant="tonal"
                  >
                    {{ t('commerceUi.shop.shortfall') }}
                    <MoneyText
                      :copper="purchaseShortfall(offer, quantityFor(offer), wallet)"
                    />
                    · {{ t('commerceUi.shop.availableMoney') }}
                    <MoneyText :copper="wallet?.availableCopper ?? 0" />
                    · {{ t('commerceUi.shop.reservedMoney') }}
                    <MoneyText :copper="wallet?.reservedCopper ?? 0" />
                  </v-alert>
                </v-list-item>
              </v-list>
            </v-window-item>

            <v-window-item value="sell">
              <v-alert
                v-if="!inventory?.items.length"
                class="tab-content"
                type="info"
                variant="tonal"
              >
                {{ t('commerceUi.shop.emptyInventory') }}
              </v-alert>
              <v-list v-else class="tab-content" bg-color="transparent">
                <ItemListRow
                  v-for="item in inventory.items"
                  :key="item.itemInstanceKey"
                  :bulk-tenths="item.revision.bulkTenths"
                  :category="item.revision.primaryCategory"
                  :name="item.revision.name"
                  :quantity="item.quantity"
                  :subtitle="t('commerceUi.shop.level', { level: item.revision.level })"
                >
                  <template #append>
                    <div class="sell-actions">
                      <template v-if="quotes[item.itemInstanceKey]">
                        <MoneyText :copper="quotes[item.itemInstanceKey].totalPriceCopper" />
                        <v-btn color="primary" variant="tonal" @click="sellCandidate = item">
                          {{ t('commerceUi.shop.sell') }}
                        </v-btn>
                      </template>
                      <v-btn
                        v-else
                        :loading="actionKey === `quote:${item.itemInstanceKey}`"
                        variant="outlined"
                        @click="quote(item)"
                      >
                        {{ t('commerceUi.shop.quote') }}
                      </v-btn>
                    </div>
                    <v-alert
                      v-for="message in quoteErrors[item.itemInstanceKey]"
                      :key="message"
                      class="quote-error"
                      type="warning"
                      variant="tonal"
                    >
                      {{ message }}
                    </v-alert>
                  </template>
                </ItemListRow>
              </v-list>
            </v-window-item>

            <v-window-item value="reservations">
              <v-alert
                v-if="!reservations.length"
                class="tab-content"
                type="info"
                variant="tonal"
              >
                {{ t('commerceUi.shop.emptyReservations') }}
              </v-alert>
              <v-list v-else class="tab-content" bg-color="transparent">
                <v-list-item
                  v-for="reservation in reservations"
                  :key="reservation.reservationKey"
                  class="reservation-row"
                  prepend-icon="mdi-timer-sand"
                  rounded="lg"
                >
                  <v-list-item-title>
                    {{ reservation.itemName }} ×{{ reservation.quantity }}
                  </v-list-item-title>
                  <v-list-item-subtitle>
                    <MoneyText :copper="reservation.totalPriceCopper" />
                  </v-list-item-subtitle>
                  <template #append>
                    <div class="reservation-actions">
                      <OperationStatusChip :status="reservationStatus(reservation.status)" />
                      <CountdownChip
                        v-if="reservation.status === 'Active'"
                        :expires-at-utc="reservation.expiresAtUtc"
                        @expired="refreshAfterPurchase"
                      />
                      <v-btn
                        v-if="reservation.status === 'Active'"
                        color="primary"
                        size="small"
                        :loading="actionKey === `complete:${reservation.reservationKey}`"
                        @click="complete(reservation)"
                      >
                        {{ t('commerceUi.shop.pay') }}
                      </v-btn>
                      <v-btn
                        v-if="reservation.status === 'Active'"
                        size="small"
                        variant="outlined"
                        :loading="actionKey === `cancel:${reservation.reservationKey}`"
                        @click="cancel(reservation)"
                      >
                        {{ t('commerceUi.shop.cancelReservation') }}
                      </v-btn>
                      <v-btn
                        v-if="
                          reservation.status === 'Expired' &&
                          offers.some((offer) => offer.offerKey === reservation.offerKey)
                        "
                        size="small"
                        variant="outlined"
                        @click="repeatReservation(reservation)"
                      >
                        {{ t('commerceUi.shop.repeat') }}
                      </v-btn>
                    </div>
                  </template>
                </v-list-item>
              </v-list>
            </v-window-item>
          </v-window>
        </main>

        <aside class="summary">
          <v-card elevation="0">
            <v-card-title>{{ t('commerceUi.shop.wallet') }}</v-card-title>
            <v-card-text class="summary-stack">
              <div>
                <span>{{ t('commerceUi.shop.availableMoney') }}</span>
                <strong><MoneyText :copper="wallet?.availableCopper ?? 0" /></strong>
              </div>
              <div>
                <span>{{ t('commerceUi.shop.reservedMoney') }}</span>
                <MoneyText :copper="wallet?.reservedCopper ?? 0" />
              </div>
            </v-card-text>
          </v-card>

          <v-card elevation="0">
            <v-card-title>{{ t('commerceUi.shop.activeReservation') }}</v-card-title>
            <v-card-text v-if="activeReservation" class="summary-stack">
              <strong>{{ activeReservation.itemName }} ×{{ activeReservation.quantity }}</strong>
              <MoneyText :copper="activeReservation.totalPriceCopper" />
              <CountdownChip
                :expires-at-utc="activeReservation.expiresAtUtc"
                @expired="refreshAfterPurchase"
              />
              <v-btn
                color="primary"
                :loading="actionKey === `complete:${activeReservation.reservationKey}`"
                @click="complete(activeReservation)"
              >
                {{ t('commerceUi.shop.pay') }}
              </v-btn>
              <v-btn
                variant="outlined"
                :loading="actionKey === `cancel:${activeReservation.reservationKey}`"
                @click="cancel(activeReservation)"
              >
                {{ t('commerceUi.shop.cancelReservation') }}
              </v-btn>
            </v-card-text>
            <v-card-text v-else>{{ t('commerceUi.shop.noActiveReservation') }}</v-card-text>
          </v-card>
        </aside>
      </div>
    </template>

    <v-dialog :model-value="Boolean(sellCandidate)" max-width="480" @update:model-value="sellCandidate = null">
      <v-card v-if="sellCandidate">
        <v-card-title>{{ t('commerceUi.shop.sellConfirmTitle') }}</v-card-title>
        <v-card-text class="summary-stack">
          <strong>{{ sellCandidate.revision.name }} ×{{ sellCandidate.quantity }}</strong>
          <span>{{ t('commerceUi.shop.sellConfirmText') }}</span>
          <MoneyText :copper="quotes[sellCandidate.itemInstanceKey]?.totalPriceCopper ?? 0" />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="sellCandidate = null">{{ t('common.cancel') }}</v-btn>
          <v-btn
            color="primary"
            :loading="actionKey === `sell:${sellCandidate.itemInstanceKey}`"
            @click="confirmSale"
          >
            {{ t('commerceUi.shop.sell') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </section>
</template>

<style scoped>
.shop-page,
.summary,
.summary-stack {
  display: grid;
  gap: 16px;
}

.shop-header {
  display: flex;
  align-items: end;
  justify-content: space-between;
  gap: 24px;
}

.shop-header h1,
.eyebrow {
  margin: 0;
}

.shop-header h1 {
  color: rgb(var(--v-theme-primary));
  font-family: Georgia, 'Times New Roman', serif;
  font-size: clamp(2rem, 5vw, 3rem);
}

.eyebrow {
  color: rgb(var(--v-theme-secondary));
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.character-select {
  max-width: 320px;
}

.shop-layout {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 280px;
  gap: 24px;
}

.tab-content {
  margin-top: 16px;
}

.offer-list,
.summary .v-card,
.offer-row,
.reservation-row {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}

.offer-actions,
.sell-actions,
.reservation-actions {
  display: flex;
  align-items: center;
  gap: 10px;
}

.offer-actions .v-number-input {
  width: 120px;
}

.shortfall,
.quote-error {
  margin-top: 10px;
}

.summary-stack > div {
  display: flex;
  justify-content: space-between;
  gap: 8px;
}

@media (max-width: 900px) {
  .shop-layout {
    grid-template-columns: 1fr;
  }

  .summary {
    grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  }
}

@media (max-width: 600px) {
  .shop-header,
  .offer-actions,
  .reservation-actions {
    align-items: stretch;
    flex-direction: column;
  }

  .character-select,
  .offer-actions .v-number-input {
    max-width: none;
    width: 100%;
  }
}
</style>
