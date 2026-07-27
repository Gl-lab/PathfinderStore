<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import { useSnackbar } from '@/composables/useSnackbar'
import {
  createCampaign,
  getCampaignInvitations,
  getCampaigns,
  respondToCampaignInvitation,
  type Campaign,
  type CampaignInvitation,
} from '@/features/campaigns/api'
import {
  campaignNameMaxLength,
  isCampaignNameValid,
  normalizeCampaignName,
} from '@/features/campaigns/validation'

const { t } = useI18n()
const route = useRoute()
const snackbar = useSnackbar()
const campaigns = ref<Campaign[]>([])
const invitations = ref<CampaignInvitation[]>([])
const campaignName = ref('')
const errorMessages = ref<string[]>([])
const isLoading = ref(true)
const isSaving = ref(false)
const respondingInvitationId = ref<number | null>(null)
const canCreate = computed(() => isCampaignNameValid(campaignName.value) && !isSaving.value)

async function loadCampaigns(): Promise<void> {
  isLoading.value = true
  errorMessages.value = []
  try {
    const [campaignItems, invitationItems] = await Promise.all([
      getCampaigns(),
      getCampaignInvitations(),
    ])
    campaigns.value = campaignItems
    invitations.value = invitationItems
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

async function respond(invitation: CampaignInvitation, accept: boolean): Promise<void> {
  respondingInvitationId.value = invitation.id
  errorMessages.value = []
  try {
    const campaign = await respondToCampaignInvitation(invitation.id, accept)
    invitations.value = invitations.value.filter((item) => item.id !== invitation.id)
    if (campaign) {
      campaigns.value = [campaign, ...campaigns.value.filter((item) => item.id !== campaign.id)]
    }
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    respondingInvitationId.value = null
  }
}

onMounted(() => {
  if (route.query.error === 'campaign-access') {
    snackbar.error(t('campaigns.accessDenied'))
  }
  void loadCampaigns()
})
</script>

<template>
  <section class="campaigns">
    <header>
      <p class="eyebrow">{{ t('campaigns.eyebrow') }}</p>
      <h1>{{ t('campaigns.title') }}</h1>
      <p class="lead">{{ t('campaigns.lead') }}</p>
    </header>

    <v-card class="outlined-card" elevation="0">
      <v-card-title>{{ t('campaigns.createTitle') }}</v-card-title>
      <v-card-text>
        <v-form class="create-form" @submit.prevent="submitCampaign">
          <v-text-field
            v-model="campaignName"
            :counter="campaignNameMaxLength"
            :hint="t('campaigns.validation.campaignName', { max: campaignNameMaxLength })"
            :label="t('campaigns.name')"
            :maxlength="campaignNameMaxLength"
            persistent-hint
            :rules="[
              (value) =>
                isCampaignNameValid(String(value ?? '')) ||
                t('campaigns.validation.campaignName', { max: campaignNameMaxLength }),
            ]"
            hide-details="auto"
          />
          <v-btn
            aria-describedby="campaign-name-requirement"
            color="accent"
            :disabled="!canCreate"
            :loading="isSaving"
            prepend-icon="mdi-map-plus"
            size="large"
            type="submit"
            >{{ t('campaigns.create') }}</v-btn
          >
          <span id="campaign-name-requirement" class="d-sr-only">
            {{ t('campaigns.validation.campaignName', { max: campaignNameMaxLength }) }}
          </span>
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

    <v-card v-if="invitations.length" class="outlined-card" elevation="0">
      <v-card-title>{{ t('campaigns.invitationsTitle') }}</v-card-title>
      <v-list>
        <v-list-item v-for="invitation in invitations" :key="invitation.id">
          <v-list-item-title>{{ invitation.campaignName }}</v-list-item-title>
          <v-list-item-subtitle>
            {{ t('campaigns.invitedBy', { userId: invitation.invitedByUserId }) }}
          </v-list-item-subtitle>
          <template #append>
            <div class="actions">
              <v-btn
                color="primary"
                :loading="respondingInvitationId === invitation.id"
                size="small"
                @click="respond(invitation, true)"
                >{{ t('campaigns.accept') }}</v-btn
              >
              <v-btn
                :disabled="respondingInvitationId === invitation.id"
                size="small"
                variant="text"
                @click="respond(invitation, false)"
                >{{ t('campaigns.decline') }}</v-btn
              >
            </div>
          </template>
        </v-list-item>
      </v-list>
    </v-card>

    <v-card
      v-if="!isLoading && !errorMessages.length && !campaigns.length"
      class="outlined-card"
      elevation="0"
    >
      <v-card-item prepend-icon="mdi-map-outline">
        <v-card-title>{{ t('campaigns.emptyTitle') }}</v-card-title>
        <v-card-subtitle>{{ t('campaigns.emptyText') }}</v-card-subtitle>
      </v-card-item>
    </v-card>

    <div v-if="!isLoading" class="campaign-grid">
      <v-card
        v-for="campaign in campaigns"
        :key="campaign.id"
        class="campaign-card"
        elevation="0"
        :to="{
          name: 'campaign-details',
          params: { campaignId: campaign.id },
          query: { tab: 'overview' },
        }"
      >
        <v-card-item prepend-icon="mdi-map-marker-path">
          <v-card-title>{{ campaign.name }}</v-card-title>
          <v-card-subtitle>{{ t(`campaigns.statuses.${campaign.status}`) }}</v-card-subtitle>
        </v-card-item>
        <v-card-text class="campaign-summary">
          <v-chip v-for="role in campaign.roles" :key="role" size="small" variant="tonal">
            {{ t(`campaigns.roles.${role}`) }}
          </v-chip>
          <span>{{ t('campaigns.memberCount', { count: campaign.members.length }) }}</span>
        </v-card-text>
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

.outlined-card,
.campaign-card {
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

.campaign-summary,
.actions {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px;
}

@media (max-width: 600px) {
  .create-form {
    grid-template-columns: 1fr;
  }
}
</style>
