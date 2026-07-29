<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { getApiErrorMessages } from '@/api/errors'
import { getSettlements, type Settlement } from './api'

const props = defineProps<{ campaignId: number; isGameMaster: boolean }>()
const { t } = useI18n()
const settlements = ref<Settlement[]>([])
const errors = ref<string[]>([])
const isLoading = ref(false)

async function load(): Promise<void> {
  isLoading.value = true
  errors.value = []
  try {
    settlements.value = await getSettlements(props.campaignId)
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    isLoading.value = false
  }
}

watch(() => props.campaignId, load)
onMounted(load)
</script>

<template>
  <section class="commerce-tab">
    <header class="tab-header">
      <div>
        <p class="eyebrow">{{ t('commerceUi.campaign.eyebrow') }}</p>
        <h2>{{ t('commerceUi.campaign.title') }}</h2>
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

    <v-alert v-if="!isLoading && !errors.length && !settlements.length" type="info" variant="tonal">
      {{ t('commerceUi.campaign.empty') }}
    </v-alert>

    <v-card v-for="settlement in settlements" :key="settlement.id" class="settlement" elevation="0">
      <v-card-title>{{ settlement.name }}</v-card-title>
      <v-card-subtitle>
        {{
          t('commerceUi.campaign.settlementMeta', {
            level: settlement.level,
            region: settlement.region,
          })
        }}
      </v-card-subtitle>
      <v-card-text class="shops">
        <v-alert v-if="!settlement.shops.length" type="info" variant="tonal">
          {{ t('commerceUi.campaign.noShops') }}
        </v-alert>
        <v-card v-for="shop in settlement.shops" :key="shop.id" class="shop" elevation="0">
          <v-card-title>{{ shop.name }}</v-card-title>
          <v-card-subtitle>{{ shop.specialization }}</v-card-subtitle>
          <v-card-text>
            {{
              t('commerceUi.campaign.policy', {
                buy: shop.catalogPricePercent,
                sell: shop.buybackPricePercent,
              })
            }}
          </v-card-text>
          <v-card-actions>
            <v-btn
              color="primary"
              :to="{
                name: 'campaign-shop',
                params: { campaignId, shopId: shop.id },
              }"
            >
              {{ t('commerceUi.campaign.openShop') }}
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-card-text>
    </v-card>

    <div v-if="isGameMaster" class="admin-links">
      <v-btn
        color="secondary"
        prepend-icon="mdi-store-cog"
        :to="{ name: 'commerce-admin', params: { campaignId } }"
        variant="tonal"
      >
        {{ t('commerceUi.campaign.openAdmin') }}
      </v-btn>
      <v-btn
        color="secondary"
        prepend-icon="mdi-book-cog-outline"
        :to="{ name: 'campaign-item-catalog', params: { campaignId } }"
        variant="tonal"
      >
        {{ t('commerceUi.campaign.openItemCatalog') }}
      </v-btn>
    </div>
  </section>
</template>

<style scoped>
.commerce-tab,
.shops {
  display: grid;
  gap: 16px;
}

.tab-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.tab-header h2,
.eyebrow {
  margin: 0;
}

.tab-header h2 {
  color: rgb(var(--v-theme-primary));
  font-family: Georgia, 'Times New Roman', serif;
}

.eyebrow {
  color: rgb(var(--v-theme-secondary));
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.settlement,
.shop {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}

.shops {
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
}

.admin-links {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}
</style>
