<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { getApiErrorMessages } from '@/api/errors'
import {
  archiveCampaign,
  changeCampaignRole,
  createCampaign,
  getCampaignInvitations,
  getCampaigns,
  inviteCampaignMember,
  leaveCampaign,
  respondToCampaignInvitation,
  type Campaign,
  type CampaignInvitation,
} from '@/features/campaigns/api'
import {
  campaignNameMaxLength,
  isCampaignNameValid,
  isCampaignUserNameValid,
  normalizeCampaignName,
} from '@/features/campaigns/validation'

const { t } = useI18n()
const campaigns = ref<Campaign[]>([])
const invitations = ref<CampaignInvitation[]>([])
const campaignName = ref('')
const invitedUserNames = ref<Record<number, string>>({})
const errorMessages = ref<string[]>([])
const isLoading = ref(true)
const isSaving = ref(false)
const archivingCampaignId = ref<number | null>(null)
const invitingCampaignId = ref<number | null>(null)
const respondingInvitationId = ref<number | null>(null)
const leavingCampaignId = ref<number | null>(null)
const changingRoleKey = ref<string | null>(null)
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

function isGameMaster(campaign: Campaign): boolean {
  return campaign.roles.includes('GameMaster')
}

function canInvite(campaignId: number): boolean {
  return isCampaignUserNameValid(invitedUserNames.value[campaignId] ?? '')
}

function replaceCampaign(updatedCampaign: Campaign): void {
  const existingIndex = campaigns.value.findIndex((campaign) => campaign.id === updatedCampaign.id)
  if (existingIndex < 0) {
    campaigns.value = [updatedCampaign, ...campaigns.value]
    return
  }

  campaigns.value = campaigns.value.map((campaign) =>
    campaign.id === updatedCampaign.id ? updatedCampaign : campaign,
  )
}

async function invite(campaign: Campaign): Promise<void> {
  const userName = invitedUserNames.value[campaign.id] ?? ''
  if (!isCampaignUserNameValid(userName)) {
    return
  }

  invitingCampaignId.value = campaign.id
  errorMessages.value = []
  try {
    await inviteCampaignMember(campaign.id, userName.trim())
    invitedUserNames.value[campaign.id] = ''
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    invitingCampaignId.value = null
  }
}

async function respond(invitation: CampaignInvitation, accept: boolean): Promise<void> {
  respondingInvitationId.value = invitation.id
  errorMessages.value = []
  try {
    const campaign = await respondToCampaignInvitation(invitation.id, accept)
    invitations.value = invitations.value.filter((item) => item.id !== invitation.id)
    if (campaign) {
      replaceCampaign(campaign)
    }
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    respondingInvitationId.value = null
  }
}

async function leave(campaign: Campaign): Promise<void> {
  leavingCampaignId.value = campaign.id
  errorMessages.value = []
  try {
    await leaveCampaign(campaign.id)
    campaigns.value = campaigns.value.filter((item) => item.id !== campaign.id)
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    leavingCampaignId.value = null
  }
}

async function toggleGameMaster(
  campaign: Campaign,
  memberUserId: number,
  assign: boolean,
): Promise<void> {
  changingRoleKey.value = `${campaign.id}:${memberUserId}`
  errorMessages.value = []
  try {
    replaceCampaign(await changeCampaignRole(campaign.id, memberUserId, 'GameMaster', assign))
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    changingRoleKey.value = null
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

    <v-card v-if="invitations.length" class="invitations-card" elevation="0">
      <v-card-title>{{ t('campaigns.invitationsTitle') }}</v-card-title>
      <v-list>
        <v-list-item v-for="invitation in invitations" :key="invitation.id">
          <v-list-item-title>{{ invitation.campaignName }}</v-list-item-title>
          <v-list-item-subtitle>
            {{ t('campaigns.invitedBy', { userId: invitation.invitedByUserId }) }}
          </v-list-item-subtitle>
          <template #append>
            <div class="invitation-actions">
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
          <div class="member-list">
            <p class="member-list__title">{{ t('campaigns.members') }}</p>
            <div v-for="member in campaign.members" :key="member.userId" class="member-row">
              <span>{{ t('campaigns.userId', { userId: member.userId }) }}</span>
              <v-chip v-for="role in member.roles" :key="role" size="x-small" variant="outlined">{{
                t(`campaigns.roles.${role}`)
              }}</v-chip>
              <v-btn
                v-if="isGameMaster(campaign)"
                :loading="changingRoleKey === `${campaign.id}:${member.userId}`"
                size="x-small"
                variant="text"
                @click="
                  toggleGameMaster(campaign, member.userId, !member.roles.includes('GameMaster'))
                "
                >{{
                  member.roles.includes('GameMaster')
                    ? t('campaigns.revokeGameMaster')
                    : t('campaigns.assignGameMaster')
                }}</v-btn
              >
            </div>
          </div>
          <v-form
            v-if="campaign.status === 'Active' && isGameMaster(campaign)"
            class="invite-form"
            @submit.prevent="invite(campaign)"
          >
            <v-text-field
              v-model="invitedUserNames[campaign.id]"
              density="compact"
              hide-details="auto"
              :label="t('campaigns.invitedUserName')"
            />
            <v-btn
              :disabled="!canInvite(campaign.id)"
              :loading="invitingCampaignId === campaign.id"
              size="small"
              type="submit"
              >{{ t('campaigns.invite') }}</v-btn
            >
          </v-form>
        </v-card-text>
        <v-card-actions v-if="campaign.status === 'Active'">
          <v-btn
            color="warning"
            :loading="leavingCampaignId === campaign.id"
            variant="text"
            @click="leave(campaign)"
            >{{ t('campaigns.leave') }}</v-btn
          >
          <v-spacer />
          <v-btn
            v-if="isGameMaster(campaign)"
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
.invitations-card,
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

.member-list {
  display: grid;
  flex-basis: 100%;
  gap: 8px;
}

.member-list__title {
  margin: 8px 0 0;
  font-weight: 700;
}

.member-row,
.invitation-actions {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px;
}

.invite-form {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto;
  flex-basis: 100%;
  gap: 8px;
  align-items: start;
}

@media (max-width: 600px) {
  .create-form {
    grid-template-columns: 1fr;
  }

  .invite-form {
    grid-template-columns: 1fr;
  }
}
</style>
