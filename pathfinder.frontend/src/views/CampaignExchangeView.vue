<script setup lang="ts">
import axios from 'axios'
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import CountdownChip from '@/components/CountdownChip.vue'
import ItemListRow from '@/components/ItemListRow.vue'
import OperationStatusChip from '@/components/OperationStatusChip.vue'
import { useSnackbar } from '@/composables/useSnackbar'
import { getCampaigns, type Campaign } from '@/features/campaigns/api'
import {
  cancelPartyExchange,
  completePartyExchange,
  getPartyExchange,
  type PartyExchange,
} from '@/features/inventory/api'
import { exchangeItemsForViewer } from '@/features/inventory/exchange'

type FinalAction = 'complete' | 'cancel'

const route = useRoute()
const { t } = useI18n()
const snackbar = useSnackbar()
const campaignId = Number(route.params.campaignId)
const exchangeKey = String(route.params.exchangeKey)
const campaign = ref<Campaign | null>(null)
const exchange = ref<PartyExchange | null>(null)
const errors = ref<string[]>([])
const isLoading = ref(true)
const isFinalizing = ref(false)
const confirmAction = ref<FinalAction | null>(null)
const operationIds = new Map<FinalAction, string>()
let pollTimer: ReturnType<typeof globalThis.setInterval> | null = null

const viewerCharacterId = computed(() => {
  if (!campaign.value || !exchange.value) return null
  const participantIds = new Set([
    exchange.value.exchange.initiatorCharacterId,
    exchange.value.exchange.counterpartyCharacterId,
  ])
  return (
    campaign.value.parties
      .flatMap((party) => party.characters)
      .find(
        (assignment) =>
          participantIds.has(assignment.characterId) &&
          assignment.controlledByUserId === campaign.value?.currentUserId,
      )?.characterId ?? null
  )
})
const isInitiator = computed(
  () => viewerCharacterId.value === exchange.value?.exchange.initiatorCharacterId,
)
const isCounterparty = computed(
  () => viewerCharacterId.value === exchange.value?.exchange.counterpartyCharacterId,
)
const perspective = computed(() => {
  if (!exchange.value) return { giving: [], receiving: [] }
  return exchangeItemsForViewer(
    exchange.value,
    viewerCharacterId.value ?? exchange.value.exchange.initiatorCharacterId,
  )
})
const givingName = computed(() => characterNameFor(viewerCharacterId.value, true))
const receivingName = computed(() => characterNameFor(viewerCharacterId.value, false))
const isPending = computed(() => exchange.value?.exchange.status === 'Pending')

function characterNameFor(viewerId: number | null, giving: boolean): string {
  if (!exchange.value) return ''
  if (viewerId) {
    if (giving) return t('tradeUi.exchange.you')
    return viewerId === exchange.value.exchange.initiatorCharacterId
      ? exchange.value.counterpartyCharacter.name
      : exchange.value.initiatorCharacter.name
  }
  return giving ? exchange.value.initiatorCharacter.name : exchange.value.counterpartyCharacter.name
}

async function load(silent = false): Promise<void> {
  if (!silent) isLoading.value = true
  errors.value = []
  try {
    const [campaigns, detail] = await Promise.all([
      getCampaigns(),
      getPartyExchange(campaignId, exchangeKey),
    ])
    campaign.value = campaigns.find((item) => item.id === campaignId) ?? null
    exchange.value = detail
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    if (!silent) isLoading.value = false
  }
}

function requestFinalize(action: FinalAction): void {
  if (!operationIds.has(action)) {
    operationIds.set(action, globalThis.crypto.randomUUID())
  }
  confirmAction.value = action
}

async function finalize(): Promise<void> {
  const action = confirmAction.value
  if (!action || isFinalizing.value) return
  const operationId = operationIds.get(action) ?? globalThis.crypto.randomUUID()
  operationIds.set(action, operationId)
  isFinalizing.value = true
  errors.value = []
  try {
    if (action === 'complete') {
      await completePartyExchange(campaignId, exchangeKey, operationId)
      snackbar.success(t('tradeUi.exchange.completed'))
    } else {
      await cancelPartyExchange(campaignId, exchangeKey, operationId)
      snackbar.success(t('tradeUi.exchange.cancelled'))
    }
    confirmAction.value = null
    await load()
  } catch (error) {
    const messages = getApiErrorMessages(error)
    if (axios.isAxiosError(error) && error.response && error.response.status < 500) {
      operationIds.delete(action)
    }
    await load()
    errors.value = messages
  } finally {
    isFinalizing.value = false
  }
}

onMounted(() => {
  void load()
  pollTimer = globalThis.setInterval(() => {
    if (!isFinalizing.value) void load(true)
  }, 45_000)
})
onUnmounted(() => {
  if (pollTimer) globalThis.clearInterval(pollTimer)
})
</script>

<template>
  <section class="exchange-view">
    <v-btn
      prepend-icon="mdi-arrow-left"
      variant="text"
      :to="{ name: 'campaign-details', params: { campaignId }, query: { tab: 'party' } }"
    >
      {{ t('inventoryUi.backToCampaign') }}
    </v-btn>

    <v-progress-linear v-if="isLoading" color="accent" indeterminate rounded />
    <v-alert v-for="message in errors" :key="message" type="error" variant="tonal">
      {{ message }}
      <template #append>
        <v-btn variant="text" @click="load()">{{ t('common.retry') }}</v-btn>
      </template>
    </v-alert>

    <template v-if="exchange">
      <header class="exchange-header">
        <div>
          <p class="eyebrow">{{ t('tradeUi.exchange.eyebrow') }}</p>
          <h1>{{ t('tradeUi.exchange.title') }}</h1>
          <p>
            {{ exchange.initiatorCharacter.name }}
            <v-icon icon="mdi-swap-horizontal" />
            {{ exchange.counterpartyCharacter.name }}
          </p>
        </div>
        <div class="status">
          <OperationStatusChip :status="exchange.exchange.status" />
          <CountdownChip
            v-if="isPending"
            :expires-at-utc="exchange.exchange.expiresAtUtc"
            @expired="load()"
          />
        </div>
      </header>

      <div class="status-track">
        <v-chip color="secondary" variant="tonal">
          {{ t('tradeUi.exchange.draftStatus') }}
        </v-chip>
        <v-icon icon="mdi-chevron-right" />
        <v-chip :color="isPending ? 'primary' : 'secondary'" variant="tonal">
          {{ t('tradeUi.exchange.pendingStatus') }}
        </v-chip>
        <v-icon icon="mdi-chevron-right" />
        <v-chip
          :color="exchange.exchange.status === 'Pending' ? undefined : 'primary'"
          variant="tonal"
        >
          {{ t(`tradeUi.exchange.${exchange.exchange.status.toLowerCase()}Status`) }}
        </v-chip>
      </div>

      <div class="exchange-columns">
        <v-card class="panel" elevation="0">
          <v-card-title>{{ t('tradeUi.exchange.youGive') }}</v-card-title>
          <v-card-subtitle>{{ givingName }}</v-card-subtitle>
          <v-card-text class="item-stack">
            <ItemListRow
              v-for="line in perspective.giving"
              :key="line.item.itemInstanceKey"
              :bulk-tenths="line.item.bulkTenths * line.item.quantity"
              :category="line.item.primaryCategory"
              :name="line.item.name"
              :quantity="line.item.quantity"
            >
              <template #append>
                <v-chip color="secondary" size="small" variant="tonal">
                  {{ t('tradeUi.exchange.reserved') }}
                </v-chip>
              </template>
            </ItemListRow>
            <v-empty-state
              v-if="perspective.giving.length === 0"
              :title="t('tradeUi.exchange.noItems')"
            />
          </v-card-text>
        </v-card>

        <v-card class="panel" elevation="0">
          <v-card-title>{{ t('tradeUi.exchange.youReceive') }}</v-card-title>
          <v-card-subtitle>{{ receivingName }}</v-card-subtitle>
          <v-card-text class="item-stack">
            <ItemListRow
              v-for="line in perspective.receiving"
              :key="line.item.itemInstanceKey"
              :bulk-tenths="line.item.bulkTenths * line.item.quantity"
              :category="line.item.primaryCategory"
              :name="line.item.name"
              :quantity="line.item.quantity"
            >
              <template #append>
                <v-chip color="secondary" size="small" variant="tonal">
                  {{ t('tradeUi.exchange.reserved') }}
                </v-chip>
              </template>
            </ItemListRow>
            <v-empty-state
              v-if="perspective.receiving.length === 0"
              :title="t('tradeUi.exchange.noItems')"
            />
          </v-card-text>
        </v-card>
      </div>

      <div v-if="isPending" class="actions">
        <v-btn
          v-if="isCounterparty"
          color="primary"
          prepend-icon="mdi-check"
          @click="requestFinalize('complete')"
        >
          {{ t('tradeUi.exchange.complete') }}
        </v-btn>
        <v-btn v-if="isInitiator" disabled prepend-icon="mdi-clock-outline">
          {{ t('tradeUi.exchange.partnerCompletes') }}
        </v-btn>
        <v-btn
          v-if="isInitiator || isCounterparty"
          color="error"
          prepend-icon="mdi-close"
          variant="tonal"
          @click="requestFinalize('cancel')"
        >
          {{
            isCounterparty ? t('tradeUi.exchange.decline') : t('tradeUi.exchange.cancelExchange')
          }}
        </v-btn>
      </div>
    </template>

    <v-dialog
      :model-value="confirmAction !== null"
      max-width="480"
      @update:model-value="!$event && (confirmAction = null)"
    >
      <v-card>
        <v-card-title>
          {{
            confirmAction === 'complete'
              ? t('tradeUi.exchange.complete')
              : t('tradeUi.exchange.cancelExchange')
          }}
        </v-card-title>
        <v-card-text>
          {{
            confirmAction === 'complete'
              ? t('tradeUi.exchange.completeConfirm')
              : t('tradeUi.exchange.cancelConfirm')
          }}
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isFinalizing" variant="text" @click="confirmAction = null">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            :color="confirmAction === 'complete' ? 'primary' : 'error'"
            :loading="isFinalizing"
            @click="finalize"
          >
            {{ t('common.confirm') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </section>
</template>

<style scoped>
.exchange-view,
.item-stack {
  display: grid;
  gap: 16px;
}

.exchange-header,
.status,
.status-track,
.actions {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
}

.exchange-header {
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

.status-track {
  justify-content: center;
}

.exchange-columns {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

.panel {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}

.actions {
  justify-content: flex-end;
}

@media (max-width: 799px) {
  .exchange-columns {
    grid-template-columns: 1fr;
  }
}
</style>
