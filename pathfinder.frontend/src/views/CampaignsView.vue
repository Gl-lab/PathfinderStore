<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { getApiErrorMessages } from '@/api/errors'
import {
  archiveCampaign,
  createCampaign,
  getCampaigns,
  type Campaign,
} from '@/features/campaigns/api'
import {
  campaignNameMaxLength,
  isCampaignNameValid,
  normalizeCampaignName,
} from '@/features/campaigns/validation'

const { t } = useI18n()
const campaigns = ref<Campaign[]>([])
const campaignName = ref('')
const errorMessages = ref<string[]>([])
const isLoading = ref(true)
const isSaving = ref(false)
const archivingCampaignId = ref<number | null>(null)
const canCreate = computed(() => isCampaignNameValid(campaignName.value) && !isSaving.value)

async function loadCampaigns(): Promise<void> {
  isLoading.value = true
  errorMessages.value = []
  try {
    campaigns.value = await getCampaigns()
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isLoading.value = false
  }
}

async function submitCampaign(): Promise<void> {
  if (!canCreate.value) {
    return
  }

  isSaving.value = true
  errorMessages.value = []
  try {
    const campaign = await createCampaign(normalizeCampaignName(campaignName.value))
    campaigns.value = [campaign, ...campaigns.value]
    campaignName.value = ''
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isSaving.value = false
  }
}

async function archive(campaign: Campaign): Promise<void> {
  archivingCampaignId.value = campaign.id
  errorMessages.value = []
  try {
    const updatedCampaign = await archiveCampaign(campaign.id)
    campaigns.value = campaigns.value.map((item) =>
      item.id === updatedCampaign.id ? updatedCampaign : item,
    )
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    archivingCampaignId.value = null
  }
}

onMounted(loadCampaigns)
</script>

<template>
  <section class="campaigns">
    <header>
      <p class="eyebrow">{{ t('campaigns.eyebrow') }}</p>
      <h1>{{ t('campaigns.title') }}</h1>
      <p class="lead">{{ t('campaigns.lead') }}</p>
    </header>

    <v-card class="create-card" elevation="0">
      <v-card-title>{{ t('campaigns.createTitle') }}</v-card-title>
      <v-card-text>
        <v-form @submit.prevent="submitCampaign">
          <div class="create-form">
            <v-text-field
              v-model="campaignName"
              :counter="campaignNameMaxLength"
              :label="t('campaigns.name')"
              :maxlength="campaignNameMaxLength"
              hide-details="auto"
            />
            <v-btn
              color="accent"
              :disabled="!canCreate"
              :loading="isSaving"
              prepend-icon="mdi-map-plus"
              size="large"
              type="submit"
              >{{ t('campaigns.create') }}</v-btn
            >
          </div>
        </v-form>
      </v-card-text>
    </v-card>

    <v-progress-linear v-if="isLoading" color="accent" indeterminate rounded />
    <v-alert v-for="message in errorMessages" :key="message" type="error" variant="tonal">
      {{ message }}
      <template #append>
        <v-btn variant="text" @click="loadCampaigns">{{ t('common.retry') }}</v-btn>
      </template>
    </v-alert>

    <v-card
      v-if="!isLoading && !errorMessages.length && !campaigns.length"
      class="empty-state"
      elevation="0"
    >
      <v-card-item prepend-icon="mdi-map-outline">
        <v-card-title>{{ t('campaigns.emptyTitle') }}</v-card-title>
        <v-card-subtitle>{{ t('campaigns.emptyText') }}</v-card-subtitle>
      </v-card-item>
    </v-card>

    <div v-if="!isLoading" class="campaign-grid">
      <v-card v-for="campaign in campaigns" :key="campaign.id" elevation="0">
        <v-card-item prepend-icon="mdi-map-marker-path">
          <v-card-title>{{ campaign.name }}</v-card-title>
          <v-card-subtitle>{{ t(`campaigns.statuses.${campaign.status}`) }}</v-card-subtitle>
        </v-card-item>
        <v-card-text class="campaign-details">
          <v-chip
            v-for="role in campaign.roles"
            :key="role"
            color="secondary"
            size="small"
            variant="tonal"
            >{{ t(`campaigns.roles.${role}`) }}</v-chip
          >
        </v-card-text>
        <v-card-actions v-if="campaign.status === 'Active'">
          <v-spacer />
          <v-btn
            color="warning"
            :loading="archivingCampaignId === campaign.id"
            variant="text"
            @click="archive(campaign)"
            >{{ t('campaigns.archive') }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </div>
  </section>
</template>

<style scoped>
.campaigns {
  display: grid;
  gap: 24px;
}

.eyebrow {
  margin: 0 0 8px;
  color: rgb(var(--v-theme-secondary));
  font-size: 0.875rem;
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

.lead {
  max-width: 680px;
  margin: 12px 0 0;
  color: #52606d;
}

.create-card,
.empty-state,
.campaign-grid > * {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}

.create-form {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto;
  gap: 16px;
  align-items: start;
}

.campaign-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 16px;
}

.campaign-details {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

@media (max-width: 600px) {
  .create-form {
    grid-template-columns: 1fr;
  }
}
</style>
