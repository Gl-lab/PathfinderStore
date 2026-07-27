<script setup lang="ts">
import axios from 'axios'
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import { getShopOfferKindLabel } from '@/i18n/domain'
import MoneyInput from '@/components/MoneyInput.vue'
import MoneyText from '@/components/MoneyText.vue'
import OperationStatusChip from '@/components/OperationStatusChip.vue'
import { useSnackbar } from '@/composables/useSnackbar'
import { getCampaigns, type Campaign } from '@/features/campaigns/api'
import {
  adjustWallet,
  createCatalogOffer,
  createSettlement,
  createShop,
  createStockOffer,
  forceMoveItem,
  getAdminContainers,
  getAdminWallet,
  getAdminWallets,
  searchPublishedItemRevisions,
  updateShopPricingPolicy,
  type AdminInventoryContainer,
  type AdminWalletSummary,
  type PublishedItemRevision,
} from '@/features/commerce/adminApi'
import {
  canForceMove,
  catalogConfigurationOptions,
  containerTitle,
  signedAdjustment,
} from '@/features/commerce/admin'
import {
  getSettlements,
  getShopOffers,
  type Settlement,
  type ShopOffer,
  type Wallet,
} from '@/features/commerce/api'
import type { InventoryOperationItem } from '@/features/inventory/api'
import { isItemVersionConflict } from '@/features/inventory/versionConflict'

type AdminTab = 'settlements' | 'shop' | 'wallets' | 'tools'

const route = useRoute()
const router = useRouter()
const { d, t } = useI18n()
const snackbar = useSnackbar()
const campaignId = Number(route.params.campaignId)
const tab = ref<AdminTab>(
  ['settlements', 'shop', 'wallets', 'tools'].includes(String(route.query.tab))
    ? (String(route.query.tab) as AdminTab)
    : 'settlements',
)
const campaign = ref<Campaign | null>(null)
const settlements = ref<Settlement[]>([])
const wallets = ref<AdminWalletSummary[]>([])
const containers = ref<AdminInventoryContainer[]>([])
const offers = ref<ShopOffer[]>([])
const revisions = ref<PublishedItemRevision[]>([])
const selectedShopId = ref<number | null>(Number(route.query.shopId) || null)
const errors = ref<string[]>([])
const actionErrors = ref<string[]>([])
const isLoading = ref(true)
const isOffersLoading = ref(false)
const isSaving = ref(false)

const settlementDialog = ref(false)
const settlementName = ref('')
const settlementLevel = ref(1)
const settlementRegion = ref('')
const settlementTraits = ref('')
const settlementOperationId = ref('')

const shopDialog = ref(false)
const shopSettlementId = ref<number | null>(null)
const shopName = ref('')
const shopSpecialization = ref('')
const shopLevel = ref(1)
const shopOperationId = ref('')

const catalogOfferDialog = ref(false)
const catalogSearch = ref('')
const catalogConfigurationId = ref<number | null>(null)
const catalogQuantity = ref(1)
const catalogOfferOperationId = ref('')
const isCatalogSearching = ref(false)

const stockOfferDialog = ref(false)
const stockItemKey = ref<string | null>(null)
const stockQuantity = ref(1)
const stockPriceCopper = ref(0)
const stockOfferOperationId = ref('')
const pricingConfirmDialog = ref(false)
const pricingOperationId = ref('')

const catalogPricePercent = ref(100)
const buybackPricePercent = ref(50)

const adjustmentDialog = ref(false)
const adjustmentWallet = ref<AdminWalletSummary | null>(null)
const adjustmentDirection = ref<'credit' | 'debit'>('credit')
const adjustmentAmountCopper = ref(0)
const adjustmentDescription = ref('')
const adjustmentOperationId = ref('')

const ledgerDialog = ref(false)
const ledgerWalletName = ref('')
const ledger = ref<Wallet | null>(null)
const isLedgerLoading = ref(false)

const forceMoveDialog = ref(false)
const forceSourceContainerKey = ref<string | null>(null)
const forceItemKey = ref<string | null>(null)
const forceDestinationContainerKey = ref<string | null>(null)
const forceReason = ref('')
const forceOperationId = ref('')

const shops = computed(() => settlements.value.flatMap((settlement) => settlement.shops))
const selectedShop = computed(
  () => shops.value.find((shop) => shop.id === selectedShopId.value) ?? null,
)
const shopSelectItems = computed(() =>
  shops.value.map((shop) => ({
    value: shop.id,
    title: `${shop.name} · ${settlementNameFor(shop.settlementId)}`,
  })),
)
const configurationOptions = computed(() => catalogConfigurationOptions(revisions.value))
const selectedRevision = computed(() =>
  revisions.value.find((revision) =>
    revision.configurations.some(
      (configuration) => configuration.itemConfigurationId === catalogConfigurationId.value,
    ),
  ),
)
const selectedCatalogPrice = computed(() =>
  selectedRevision.value && selectedShop.value
    ? Math.trunc(
        (selectedRevision.value.priceInCopperPieces * selectedShop.value.catalogPricePercent) / 100,
      )
    : 0,
)
const selectedShopContainers = computed(() =>
  containers.value.filter(
    (container) => container.ownerKind === 'Shop' && container.ownerId === selectedShopId.value,
  ),
)
const stockItemOptions = computed(() =>
  selectedShopContainers.value.flatMap((container) =>
    container.items.map((item) => ({
      value: item.itemInstanceKey,
      title: `${item.name} ×${item.quantity}`,
      item,
    })),
  ),
)
const adjustmentPreview = computed(
  () =>
    (adjustmentWallet.value?.balanceCopper ?? 0) +
    signedAdjustment(adjustmentAmountCopper.value, adjustmentDirection.value),
)
const adjustmentIsValid = computed(
  () =>
    adjustmentAmountCopper.value > 0 &&
    Boolean(adjustmentDescription.value.trim()) &&
    (adjustmentDirection.value === 'credit' ||
      adjustmentAmountCopper.value <= (adjustmentWallet.value?.availableCopper ?? 0)),
)
const forceSourceContainer = computed(
  () =>
    containers.value.find(
      (container) => container.containerKey === forceSourceContainerKey.value,
    ) ?? null,
)
const forceItemOptions = computed(() =>
  (forceSourceContainer.value?.items ?? []).map((item) => ({
    value: item.itemInstanceKey,
    title: `${item.name} ×${item.quantity}`,
  })),
)
const forceItem = computed(
  () =>
    forceSourceContainer.value?.items.find((item) => item.itemInstanceKey === forceItemKey.value) ??
    null,
)
const canSubmitForceMove = computed(() =>
  canForceMove(
    forceSourceContainerKey.value,
    forceItemKey.value,
    forceDestinationContainerKey.value,
    forceReason.value,
  ),
)

watch(selectedShopId, async (shopId) => {
  if (!shopId) {
    offers.value = []
    return
  }
  const shop = selectedShop.value
  if (shop) {
    catalogPricePercent.value = shop.catalogPricePercent
    buybackPricePercent.value = shop.buybackPricePercent
  }
  await loadOffers()
})
watch(forceSourceContainerKey, () => {
  forceItemKey.value = null
})

function settlementNameFor(settlementId: number): string {
  return settlements.value.find((settlement) => settlement.id === settlementId)?.name ?? ''
}

function selectShop(shopId: number): void {
  selectedShopId.value = shopId
  tab.value = 'shop'
}

function isForbidden(error: unknown): boolean {
  return axios.isAxiosError(error) && error.response?.status === 403
}

function isBusinessRejection(error: unknown): boolean {
  return (
    axios.isAxiosError(error) && Boolean(error.response) && (error.response?.status ?? 500) < 500
  )
}

async function redirectFromAdmin(): Promise<void> {
  snackbar.error(t('commerceAdmin.accessDenied'))
  await router.replace({ name: 'campaign-details', params: { campaignId } })
}

async function load(): Promise<void> {
  isLoading.value = true
  errors.value = []
  try {
    const campaigns = await getCampaigns()
    const currentCampaign = campaigns.find((item) => item.id === campaignId) ?? null
    if (
      !currentCampaign ||
      currentCampaign.status !== 'Active' ||
      !currentCampaign.roles.includes('GameMaster')
    ) {
      await redirectFromAdmin()
      return
    }
    campaign.value = currentCampaign
    const [settlementItems, walletItems, containerItems] = await Promise.all([
      getSettlements(campaignId),
      getAdminWallets(campaignId),
      getAdminContainers(campaignId),
    ])
    settlements.value = settlementItems
    wallets.value = walletItems
    containers.value = containerItems
    if (!selectedShopId.value || !shops.value.some((shop) => shop.id === selectedShopId.value)) {
      selectedShopId.value = shops.value[0]?.id ?? null
    } else {
      await loadOffers()
    }
  } catch (error) {
    if (isForbidden(error)) {
      await redirectFromAdmin()
    } else {
      errors.value = getApiErrorMessages(error)
    }
  } finally {
    isLoading.value = false
  }
}

async function loadOffers(): Promise<void> {
  if (!selectedShopId.value) return
  isOffersLoading.value = true
  actionErrors.value = []
  try {
    offers.value = await getShopOffers(campaignId, selectedShopId.value, 'All')
  } catch (error) {
    if (isForbidden(error)) {
      await redirectFromAdmin()
    } else {
      actionErrors.value = getApiErrorMessages(error)
    }
  } finally {
    isOffersLoading.value = false
  }
}

function openSettlementDialog(): void {
  settlementName.value = ''
  settlementLevel.value = 1
  settlementRegion.value = ''
  settlementTraits.value = ''
  settlementOperationId.value = globalThis.crypto.randomUUID()
  actionErrors.value = []
  settlementDialog.value = true
}

async function saveSettlement(): Promise<void> {
  if (!settlementName.value.trim() || isSaving.value) return
  isSaving.value = true
  actionErrors.value = []
  try {
    await createSettlement(campaignId, {
      operationId: settlementOperationId.value,
      name: settlementName.value,
      level: settlementLevel.value,
      region: settlementRegion.value,
      traits: settlementTraits.value,
    })
    settlementDialog.value = false
    snackbar.success(t('commerceAdmin.settlements.created'))
    await load()
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
    if (isBusinessRejection(error)) {
      settlementOperationId.value = globalThis.crypto.randomUUID()
    }
  } finally {
    isSaving.value = false
  }
}

function openShopDialog(settlementId: number): void {
  shopSettlementId.value = settlementId
  shopName.value = ''
  shopSpecialization.value = ''
  shopLevel.value = 1
  shopOperationId.value = globalThis.crypto.randomUUID()
  actionErrors.value = []
  shopDialog.value = true
}

async function saveShop(): Promise<void> {
  if (!shopSettlementId.value || !shopName.value.trim() || isSaving.value) return
  isSaving.value = true
  actionErrors.value = []
  try {
    const shop = await createShop(campaignId, shopSettlementId.value, {
      operationId: shopOperationId.value,
      name: shopName.value,
      specialization: shopSpecialization.value,
      shopLevel: shopLevel.value,
    })
    shopDialog.value = false
    selectedShopId.value = shop.id
    tab.value = 'shop'
    snackbar.success(t('commerceAdmin.shops.created'))
    await load()
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
    if (isBusinessRejection(error)) {
      shopOperationId.value = globalThis.crypto.randomUUID()
    }
  } finally {
    isSaving.value = false
  }
}

async function savePricingPolicy(): Promise<void> {
  if (!selectedShopId.value || isSaving.value) return
  isSaving.value = true
  actionErrors.value = []
  try {
    await updateShopPricingPolicy(campaignId, selectedShopId.value, {
      operationId: pricingOperationId.value,
      catalogPricePercent: catalogPricePercent.value,
      buybackPricePercent: buybackPricePercent.value,
    })
    pricingConfirmDialog.value = false
    snackbar.success(t('commerceAdmin.shop.policySaved'))
    await load()
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
    if (isBusinessRejection(error)) {
      pricingOperationId.value = globalThis.crypto.randomUUID()
    }
  } finally {
    isSaving.value = false
  }
}

function openPricingConfirmDialog(): void {
  pricingOperationId.value = globalThis.crypto.randomUUID()
  actionErrors.value = []
  pricingConfirmDialog.value = true
}

function openCatalogOfferDialog(): void {
  catalogSearch.value = ''
  catalogConfigurationId.value = null
  catalogQuantity.value = 1
  catalogOfferOperationId.value = globalThis.crypto.randomUUID()
  revisions.value = []
  actionErrors.value = []
  catalogOfferDialog.value = true
  void searchCatalog()
}

async function searchCatalog(): Promise<void> {
  isCatalogSearching.value = true
  actionErrors.value = []
  try {
    revisions.value = await searchPublishedItemRevisions(campaignId, catalogSearch.value)
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
  } finally {
    isCatalogSearching.value = false
  }
}

async function saveCatalogOffer(): Promise<void> {
  if (
    !selectedShopId.value ||
    !catalogConfigurationId.value ||
    catalogQuantity.value <= 0 ||
    isSaving.value
  ) {
    return
  }
  isSaving.value = true
  actionErrors.value = []
  try {
    await createCatalogOffer(campaignId, selectedShopId.value, {
      operationId: catalogOfferOperationId.value,
      itemConfigurationId: catalogConfigurationId.value,
      quantity: catalogQuantity.value,
    })
    catalogOfferDialog.value = false
    snackbar.success(t('commerceAdmin.shop.offerCreated'))
    await loadOffers()
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
    if (isBusinessRejection(error)) {
      catalogOfferOperationId.value = globalThis.crypto.randomUUID()
    }
  } finally {
    isSaving.value = false
  }
}

function openStockOfferDialog(): void {
  stockItemKey.value = null
  stockQuantity.value = 1
  stockPriceCopper.value = 0
  stockOfferOperationId.value = globalThis.crypto.randomUUID()
  actionErrors.value = []
  stockOfferDialog.value = true
}

async function saveStockOffer(): Promise<void> {
  if (
    !selectedShopId.value ||
    !stockItemKey.value ||
    stockQuantity.value <= 0 ||
    stockPriceCopper.value <= 0 ||
    isSaving.value
  ) {
    return
  }
  isSaving.value = true
  actionErrors.value = []
  try {
    await createStockOffer(campaignId, selectedShopId.value, {
      operationId: stockOfferOperationId.value,
      itemInstanceKey: stockItemKey.value,
      quantity: stockQuantity.value,
      unitPriceCopper: stockPriceCopper.value,
    })
    stockOfferDialog.value = false
    snackbar.success(t('commerceAdmin.shop.offerCreated'))
    await Promise.all([loadOffers(), refreshContainers()])
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
    if (isBusinessRejection(error)) {
      stockOfferOperationId.value = globalThis.crypto.randomUUID()
    }
  } finally {
    isSaving.value = false
  }
}

function openAdjustment(wallet: AdminWalletSummary): void {
  adjustmentWallet.value = wallet
  adjustmentDirection.value = 'credit'
  adjustmentAmountCopper.value = 0
  adjustmentDescription.value = ''
  adjustmentOperationId.value = globalThis.crypto.randomUUID()
  actionErrors.value = []
  adjustmentDialog.value = true
}

async function saveAdjustment(): Promise<void> {
  if (!adjustmentWallet.value || !adjustmentIsValid.value || isSaving.value) {
    return
  }
  isSaving.value = true
  actionErrors.value = []
  try {
    await adjustWallet(campaignId, adjustmentWallet.value.characterId, {
      operationId: adjustmentOperationId.value,
      amountCopper: signedAdjustment(adjustmentAmountCopper.value, adjustmentDirection.value),
      description: adjustmentDescription.value,
    })
    adjustmentDialog.value = false
    snackbar.success(t('commerceAdmin.wallets.adjusted'))
    wallets.value = await getAdminWallets(campaignId)
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
    if (isBusinessRejection(error)) {
      adjustmentOperationId.value = globalThis.crypto.randomUUID()
    }
  } finally {
    isSaving.value = false
  }
}

async function openLedger(wallet: AdminWalletSummary): Promise<void> {
  ledgerWalletName.value = wallet.characterName
  ledger.value = null
  ledgerDialog.value = true
  isLedgerLoading.value = true
  actionErrors.value = []
  try {
    ledger.value = await getAdminWallet(campaignId, wallet.characterId)
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
  } finally {
    isLedgerLoading.value = false
  }
}

function openForceMove(): void {
  forceSourceContainerKey.value = null
  forceItemKey.value = null
  forceDestinationContainerKey.value = null
  forceReason.value = ''
  forceOperationId.value = globalThis.crypto.randomUUID()
  actionErrors.value = []
  forceMoveDialog.value = true
}

async function saveForceMove(): Promise<void> {
  if (
    !canSubmitForceMove.value ||
    !forceItem.value ||
    !forceDestinationContainerKey.value ||
    isSaving.value
  ) {
    return
  }
  isSaving.value = true
  actionErrors.value = []
  try {
    await forceMoveItem(campaignId, {
      itemInstanceKey: forceItem.value.itemInstanceKey,
      destinationContainerKey: forceDestinationContainerKey.value,
      expectedItemVersion: forceItem.value.version,
      operationId: forceOperationId.value,
      reason: forceReason.value,
    })
    forceMoveDialog.value = false
    snackbar.success(t('commerceAdmin.tools.moved'))
    await refreshContainers()
  } catch (error) {
    const messages = getApiErrorMessages(error)
    if (isBusinessRejection(error)) {
      forceOperationId.value = globalThis.crypto.randomUUID()
    }
    if (isItemVersionConflict(error)) {
      try {
        await refreshContainers()
      } catch {
        // Preserve the original conflict as the actionable error.
      }
      actionErrors.value = [t('tradeUi.gift.versionConflict'), ...messages]
    } else {
      actionErrors.value = messages
    }
  } finally {
    isSaving.value = false
  }
}

async function refreshContainers(): Promise<void> {
  containers.value = await getAdminContainers(campaignId)
}

function itemForStockKey(itemKey: string | null): InventoryOperationItem | null {
  return stockItemOptions.value.find((option) => option.value === itemKey)?.item ?? null
}

onMounted(() => {
  void load()
})
</script>

<template>
  <section class="admin-view">
    <v-btn
      prepend-icon="mdi-arrow-left"
      variant="text"
      :to="{
        name: 'campaign-details',
        params: { campaignId },
        query: { tab: 'commerce' },
      }"
    >
      {{ t('commerceAdmin.back') }}
    </v-btn>

    <header class="admin-header">
      <div>
        <p class="eyebrow">{{ t('commerceAdmin.eyebrow') }}</p>
        <h1>{{ t('commerceAdmin.title', { name: campaign?.name ?? `#${campaignId}` }) }}</h1>
      </div>
      <v-btn icon="mdi-refresh" :loading="isLoading" variant="text" @click="load" />
    </header>

    <v-progress-linear v-if="isLoading" color="accent" indeterminate rounded />
    <v-alert v-for="message in errors" :key="message" type="error" variant="tonal">
      {{ message }}
      <template #append>
        <v-btn variant="text" @click="load">{{ t('common.retry') }}</v-btn>
      </template>
    </v-alert>
    <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
      {{ message }}
    </v-alert>

    <template v-if="campaign">
      <v-tabs v-model="tab" color="primary">
        <v-tab value="settlements">{{ t('commerceAdmin.tabs.settlements') }}</v-tab>
        <v-tab value="shop">{{ t('commerceAdmin.tabs.shop') }}</v-tab>
        <v-tab value="wallets">{{ t('commerceAdmin.tabs.wallets') }}</v-tab>
        <v-tab value="tools">{{ t('commerceAdmin.tabs.tools') }}</v-tab>
      </v-tabs>

      <v-window v-model="tab">
        <v-window-item value="settlements">
          <section class="tab-panel">
            <div class="section-header">
              <h2>{{ t('commerceAdmin.settlements.title') }}</h2>
              <v-btn color="primary" prepend-icon="mdi-plus" @click="openSettlementDialog">
                {{ t('commerceAdmin.settlements.add') }}
              </v-btn>
            </div>
            <v-empty-state
              v-if="!settlements.length"
              icon="mdi-city-variant-outline"
              :text="t('commerceAdmin.settlements.emptyText')"
              :title="t('commerceAdmin.settlements.emptyTitle')"
            >
              <template #actions>
                <v-btn color="primary" @click="openSettlementDialog">
                  {{ t('commerceAdmin.settlements.addFirst') }}
                </v-btn>
              </template>
            </v-empty-state>
            <v-card
              v-for="settlement in settlements"
              :key="settlement.id"
              class="panel"
              elevation="0"
            >
              <v-card-title>{{ settlement.name }}</v-card-title>
              <v-card-subtitle>
                {{
                  t('commerceAdmin.settlements.meta', {
                    level: settlement.level,
                    region: settlement.region,
                  })
                }}
              </v-card-subtitle>
              <v-card-text class="card-grid">
                <v-card
                  v-for="shop in settlement.shops"
                  :key="shop.id"
                  class="nested-card"
                  variant="outlined"
                  @click="selectShop(shop.id)"
                >
                  <v-card-title>{{ shop.name }}</v-card-title>
                  <v-card-subtitle>{{ shop.specialization }}</v-card-subtitle>
                </v-card>
                <v-btn
                  prepend-icon="mdi-store-plus"
                  variant="tonal"
                  @click="openShopDialog(settlement.id)"
                >
                  {{ t('commerceAdmin.shops.add') }}
                </v-btn>
              </v-card-text>
            </v-card>
          </section>
        </v-window-item>

        <v-window-item value="shop">
          <section class="tab-panel">
            <v-select
              v-model="selectedShopId"
              item-title="title"
              item-value="value"
              :items="shopSelectItems"
              :label="t('commerceAdmin.shop.select')"
            />
            <v-empty-state
              v-if="!selectedShop"
              icon="mdi-store-off-outline"
              :title="t('commerceAdmin.shop.noShops')"
            />
            <template v-else>
              <v-card class="panel" elevation="0">
                <v-card-title class="section-header">
                  {{ t('commerceAdmin.shop.assortment') }}
                  <span class="button-row">
                    <v-btn
                      prepend-icon="mdi-book-plus"
                      size="small"
                      @click="openCatalogOfferDialog"
                    >
                      {{ t('commerceAdmin.shop.addCatalog') }}
                    </v-btn>
                    <v-btn
                      prepend-icon="mdi-package-variant-plus"
                      size="small"
                      variant="tonal"
                      @click="openStockOfferDialog"
                    >
                      {{ t('commerceAdmin.shop.addStock') }}
                    </v-btn>
                  </span>
                </v-card-title>
                <v-progress-linear v-if="isOffersLoading" indeterminate />
                <v-card-text class="list-stack">
                  <div v-for="offer in offers" :key="offer.offerKey" class="offer-row">
                    <div>
                      <strong>{{ offer.itemName }}</strong>
                      <p>
                        {{ getShopOfferKindLabel(offer.kind) }} ·
                        {{ t('commerceAdmin.shop.quantity', { count: offer.availableQuantity }) }}
                      </p>
                    </div>
                    <MoneyText :copper="offer.unitPriceCopper" />
                    <OperationStatusChip :status="offer.status" />
                  </div>
                  <v-empty-state
                    v-if="!isOffersLoading && !offers.length"
                    :text="t('commerceAdmin.shop.emptyText')"
                    :title="t('commerceAdmin.shop.emptyTitle')"
                  />
                </v-card-text>
              </v-card>

              <v-card class="panel" elevation="0">
                <v-card-title>{{ t('commerceAdmin.shop.policy') }}</v-card-title>
                <v-card-text class="form-grid">
                  <v-number-input
                    v-model="catalogPricePercent"
                    :label="t('commerceAdmin.shop.catalogPercent')"
                    :min="1"
                    :max="1000"
                  />
                  <v-number-input
                    v-model="buybackPricePercent"
                    :label="t('commerceAdmin.shop.buybackPercent')"
                    :min="0"
                    :max="100"
                  />
                </v-card-text>
                <v-card-actions>
                  <v-spacer />
                  <v-btn
                    color="primary"
                    :disabled="
                      catalogPricePercent < 1 ||
                      catalogPricePercent > 1000 ||
                      buybackPricePercent < 0 ||
                      buybackPricePercent > 100
                    "
                    :loading="isSaving"
                    @click="openPricingConfirmDialog"
                  >
                    {{ t('common.save') }}
                  </v-btn>
                </v-card-actions>
              </v-card>
            </template>
          </section>
        </v-window-item>

        <v-window-item value="wallets">
          <section class="tab-panel">
            <h2>{{ t('commerceAdmin.wallets.title') }}</h2>
            <v-card class="panel" elevation="0">
              <v-list>
                <v-list-item v-for="wallet in wallets" :key="wallet.characterId">
                  <v-list-item-title>{{ wallet.characterName }}</v-list-item-title>
                  <v-list-item-subtitle>
                    {{ t('commerceAdmin.wallets.reserved') }}
                    <MoneyText :copper="wallet.reservedCopper" compact />
                  </v-list-item-subtitle>
                  <template #append>
                    <MoneyText :copper="wallet.balanceCopper" />
                    <v-btn
                      icon="mdi-book-open-variant"
                      size="small"
                      variant="text"
                      @click="openLedger(wallet)"
                    />
                    <v-btn
                      icon="mdi-plus-minus-variant"
                      size="small"
                      variant="tonal"
                      @click="openAdjustment(wallet)"
                    />
                  </template>
                </v-list-item>
              </v-list>
              <v-empty-state v-if="!wallets.length" :title="t('commerceAdmin.wallets.empty')" />
            </v-card>
          </section>
        </v-window-item>

        <v-window-item value="tools">
          <section class="tab-panel">
            <h2>{{ t('commerceAdmin.tools.title') }}</h2>
            <v-card class="panel danger-panel" elevation="0">
              <v-card-title>{{ t('commerceAdmin.tools.forceMove') }}</v-card-title>
              <v-card-text>
                <v-alert type="warning" variant="tonal">
                  {{ t('commerceAdmin.tools.auditWarning') }}
                </v-alert>
              </v-card-text>
              <v-card-actions>
                <v-btn color="error" prepend-icon="mdi-swap-horizontal-bold" @click="openForceMove">
                  {{ t('commerceAdmin.tools.openForceMove') }}
                </v-btn>
              </v-card-actions>
            </v-card>
          </section>
        </v-window-item>
      </v-window>
    </template>

    <v-dialog v-model="settlementDialog" max-width="560" persistent>
      <v-card>
        <v-card-title>{{ t('commerceAdmin.settlements.add') }}</v-card-title>
        <v-card-text class="form-stack">
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <v-text-field v-model="settlementName" :label="t('common.name')" />
          <v-number-input
            v-model="settlementLevel"
            :label="t('commerceAdmin.settlements.level')"
            :min="0"
            :max="20"
          />
          <v-text-field v-model="settlementRegion" :label="t('commerceAdmin.settlements.region')" />
          <v-text-field v-model="settlementTraits" :label="t('commerceAdmin.settlements.traits')" />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isSaving" variant="text" @click="settlementDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            color="primary"
            :disabled="!settlementName.trim()"
            :loading="isSaving"
            @click="saveSettlement"
          >
            {{ t('common.create') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="shopDialog" max-width="560" persistent>
      <v-card>
        <v-card-title>{{ t('commerceAdmin.shops.add') }}</v-card-title>
        <v-card-text class="form-stack">
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <v-text-field v-model="shopName" :label="t('common.name')" />
          <v-text-field
            v-model="shopSpecialization"
            :label="t('commerceAdmin.shops.specialization')"
          />
          <v-number-input
            v-model="shopLevel"
            :label="t('commerceAdmin.shops.level')"
            :min="0"
            :max="20"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isSaving" variant="text" @click="shopDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn color="primary" :disabled="!shopName.trim()" :loading="isSaving" @click="saveShop">
            {{ t('common.create') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="catalogOfferDialog" max-width="680" persistent>
      <v-card>
        <v-card-title>{{ t('commerceAdmin.shop.addCatalog') }}</v-card-title>
        <v-card-text class="form-stack">
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <v-text-field
            v-model="catalogSearch"
            append-inner-icon="mdi-magnify"
            :label="t('commerceAdmin.shop.searchCatalog')"
            :loading="isCatalogSearching"
            @click:append-inner="searchCatalog"
            @keyup.enter="searchCatalog"
          />
          <v-select
            v-model="catalogConfigurationId"
            item-title="title"
            item-value="value"
            :items="configurationOptions"
            :label="t('commerceAdmin.shop.catalogItem')"
          />
          <v-number-input
            v-model="catalogQuantity"
            :label="t('commerceAdmin.shop.offerQuantity')"
            :min="1"
          />
          <v-alert v-if="selectedRevision" type="info" variant="tonal">
            {{ t('commerceAdmin.shop.resultingPrice') }}
            <MoneyText :copper="selectedCatalogPrice" />
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isSaving" variant="text" @click="catalogOfferDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            color="primary"
            :disabled="!catalogConfigurationId || catalogQuantity < 1"
            :loading="isSaving"
            @click="saveCatalogOffer"
          >
            {{ t('common.create') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="pricingConfirmDialog" max-width="500" persistent>
      <v-card>
        <v-card-title>{{ t('commerceAdmin.shop.policy') }}</v-card-title>
        <v-card-text class="form-stack">
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <p>{{ t('commerceAdmin.shop.policyConfirm') }}</p>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isSaving" variant="text" @click="pricingConfirmDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn color="primary" :loading="isSaving" @click="savePricingPolicy">
            {{ t('common.confirm') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="stockOfferDialog" max-width="680" persistent>
      <v-card>
        <v-card-title>{{ t('commerceAdmin.shop.addStock') }}</v-card-title>
        <v-card-text class="form-stack">
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <v-select
            v-model="stockItemKey"
            item-title="title"
            item-value="value"
            :items="stockItemOptions"
            :label="t('commerceAdmin.shop.stockItem')"
          />
          <v-number-input
            v-model="stockQuantity"
            :label="t('commerceAdmin.shop.offerQuantity')"
            :max="itemForStockKey(stockItemKey)?.quantity ?? 1"
            :min="1"
          />
          <MoneyInput v-model="stockPriceCopper" />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isSaving" variant="text" @click="stockOfferDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            color="primary"
            :disabled="!stockItemKey || stockQuantity < 1 || stockPriceCopper < 1"
            :loading="isSaving"
            @click="saveStockOffer"
          >
            {{ t('common.create') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="adjustmentDialog" max-width="640" persistent>
      <v-card>
        <v-card-title>
          {{ t('commerceAdmin.wallets.adjust', { name: adjustmentWallet?.characterName }) }}
        </v-card-title>
        <v-card-text class="form-stack">
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <v-btn-toggle v-model="adjustmentDirection" mandatory>
            <v-btn value="credit">{{ t('commerceAdmin.wallets.credit') }}</v-btn>
            <v-btn value="debit">{{ t('commerceAdmin.wallets.debit') }}</v-btn>
          </v-btn-toggle>
          <MoneyInput v-model="adjustmentAmountCopper" />
          <v-textarea
            v-model="adjustmentDescription"
            :label="t('commerceAdmin.wallets.description')"
            :rules="[(value: string) => Boolean(value.trim()) || t('commerceAdmin.required')]"
          />
          <v-alert type="info" variant="tonal">
            {{ t('commerceAdmin.wallets.balanceAfter') }}
            <MoneyText :copper="adjustmentPreview" />
          </v-alert>
          <v-alert
            v-if="
              adjustmentDirection === 'debit' &&
              adjustmentAmountCopper > (adjustmentWallet?.availableCopper ?? 0)
            "
            type="warning"
            variant="tonal"
          >
            {{ t('commerceAdmin.wallets.insufficient') }}
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isSaving" variant="text" @click="adjustmentDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            color="primary"
            :disabled="!adjustmentIsValid"
            :loading="isSaving"
            @click="saveAdjustment"
          >
            {{ t('common.confirm') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="ledgerDialog" max-width="720">
      <v-card>
        <v-card-title>{{
          t('commerceAdmin.wallets.ledger', { name: ledgerWalletName })
        }}</v-card-title>
        <v-progress-linear v-if="isLedgerLoading" indeterminate />
        <v-card-text>
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <v-list v-if="ledger?.entries.length">
            <v-list-item v-for="entry in ledger.entries" :key="entry.operationId">
              <v-list-item-title>{{ entry.description }}</v-list-item-title>
              <v-list-item-subtitle>
                {{ d(new Date(entry.occurredAtUtc), { dateStyle: 'medium', timeStyle: 'short' }) }}
                · {{ entry.kind }}
              </v-list-item-subtitle>
              <template #append>
                <MoneyText :copper="entry.amountCopper" />
              </template>
            </v-list-item>
          </v-list>
          <v-empty-state
            v-else-if="!isLedgerLoading"
            :title="t('inventoryUi.wallet.emptyLedger')"
          />
        </v-card-text>
      </v-card>
    </v-dialog>

    <v-dialog v-model="forceMoveDialog" max-width="760" persistent>
      <v-card>
        <v-card-title>{{ t('commerceAdmin.tools.forceMove') }}</v-card-title>
        <v-card-text class="form-stack">
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <v-alert type="warning" variant="tonal">
            {{ t('commerceAdmin.tools.auditWarning') }}
          </v-alert>
          <v-select
            v-model="forceSourceContainerKey"
            :item-title="containerTitle"
            item-value="containerKey"
            :items="containers"
            :label="t('commerceAdmin.tools.source')"
          />
          <v-select
            v-model="forceItemKey"
            item-title="title"
            item-value="value"
            :items="forceItemOptions"
            :label="t('commerceAdmin.tools.item')"
          />
          <v-select
            v-model="forceDestinationContainerKey"
            :item-title="containerTitle"
            item-value="containerKey"
            :items="containers"
            :label="t('commerceAdmin.tools.destination')"
          />
          <v-textarea
            v-model="forceReason"
            :label="t('commerceAdmin.tools.reason')"
            :rules="[(value: string) => Boolean(value.trim()) || t('commerceAdmin.required')]"
          />
          <v-alert v-if="canSubmitForceMove && forceItem" type="error" variant="tonal">
            {{
              t('commerceAdmin.tools.confirmMove', {
                item: forceItem.name,
                source: containerTitle(forceSourceContainer!),
                destination: containerTitle(
                  containers.find(
                    (container) => container.containerKey === forceDestinationContainerKey,
                  )!,
                ),
              })
            }}
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isSaving" variant="text" @click="forceMoveDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            color="error"
            :disabled="!canSubmitForceMove"
            :loading="isSaving"
            @click="saveForceMove"
          >
            {{ t('common.confirm') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </section>
</template>

<style scoped>
.admin-view,
.tab-panel,
.form-stack,
.list-stack {
  display: grid;
  gap: 16px;
}

.admin-header,
.section-header,
.offer-row,
.button-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
}

.admin-header,
.section-header,
.offer-row {
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

h2 {
  margin: 0;
  font-family: Georgia, 'Times New Roman', serif;
}

.tab-panel {
  padding: 20px 0;
}

.panel {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}

.danger-panel {
  border-color: rgb(var(--v-theme-error));
}

.card-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 12px;
}

.nested-card {
  cursor: pointer;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

.offer-row {
  padding: 12px;
  border-bottom: 1px solid rgb(var(--v-theme-surface-variant));
}

.offer-row p {
  margin: 4px 0 0;
  color: rgb(var(--v-theme-on-surface-variant));
}

@media (max-width: 699px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
