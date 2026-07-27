<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import {
  archiveCampaign,
  assignCampaignPartyCharacter,
  changeCampaignRole,
  createCampaignParty,
  getCampaignCharacters,
  getCampaigns,
  inviteCampaignMember,
  leaveCampaign,
  type Campaign,
  type CampaignCharacterReference,
} from '@/features/campaigns/api'
import { isCampaignUserNameValid } from '@/features/campaigns/validation'
import PartyStorageTab from '@/features/inventory/PartyStorageTab.vue'
import CommerceCampaignTab from '@/features/commerce/CommerceCampaignTab.vue'
import { usePendingOperations } from '@/features/inventory/usePendingOperations'

type CampaignTab = 'overview' | 'members' | 'party' | 'storage' | 'commerce'

const allowedTabs: CampaignTab[] = ['overview', 'members', 'party', 'storage', 'commerce']
const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const campaign = ref<Campaign | null>(null)
const characters = ref<CampaignCharacterReference[]>([])
const invitedUserName = ref('')
const partyName = ref('')
const partyCharacterId = ref<number | null>(null)
const partyControllerId = ref<number | null>(null)
const errorMessages = ref<string[]>([])
const isLoading = ref(true)
const actionKey = ref<string | null>(null)
const archiveDialog = ref(false)
const campaignId = computed(() => Number(route.params.campaignId))
const activeTab = computed<CampaignTab>({
  get: () => {
    const requested = String(route.query.tab ?? 'overview') as CampaignTab
    return allowedTabs.includes(requested) ? requested : 'overview'
  },
  set: (value) => {
    void router.replace({ query: { ...route.query, tab: value } })
  },
})
const activeParty = computed(() =>
  campaign.value?.parties.find((party) => party.status === 'Active'),
)
const isGameMaster = computed(() => campaign.value?.roles.includes('GameMaster') ?? false)
const playerMembers = computed(
  () => campaign.value?.members.filter((member) => member.roles.includes('Player')) ?? [],
)
const pendingCharacterIds = computed(
  () =>
    activeParty.value?.characters
      .filter((character) => character.controlledByUserId === campaign.value?.currentUserId)
      .map((character) => character.characterId) ?? [],
)
const pendingOperations = usePendingOperations(campaignId, pendingCharacterIds)

async function load(): Promise<void> {
  isLoading.value = true
  errorMessages.value = []
  try {
    const [campaigns, characterItems] = await Promise.all([getCampaigns(), getCampaignCharacters()])
    campaign.value = campaigns.find((item) => item.id === campaignId.value) ?? null
    characters.value = characterItems
    if (!campaign.value) {
      await router.replace({ name: 'campaigns', query: { error: 'campaign-access' } })
    }
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isLoading.value = false
  }
}

function characterName(characterId: number): string {
  return characters.value.find((item) => item.id === characterId)?.name ?? `#${characterId}`
}

async function runAction(key: string, action: () => Promise<Campaign>): Promise<void> {
  actionKey.value = key
  errorMessages.value = []
  try {
    campaign.value = await action()
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    actionKey.value = null
  }
}

async function invite(): Promise<void> {
  if (!campaign.value || !isCampaignUserNameValid(invitedUserName.value)) {
    return
  }
  actionKey.value = 'invite'
  errorMessages.value = []
  try {
    await inviteCampaignMember(campaign.value.id, invitedUserName.value.trim())
    invitedUserName.value = ''
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    actionKey.value = null
  }
}

async function toggleGameMaster(userId: number, assign: boolean): Promise<void> {
  if (!campaign.value) return
  await runAction(`role:${userId}`, () =>
    changeCampaignRole(campaign.value!.id, userId, 'GameMaster', assign),
  )
}

async function createParty(): Promise<void> {
  if (!campaign.value || !partyName.value.trim()) return
  await runAction('party:create', () =>
    createCampaignParty(campaign.value!.id, partyName.value.trim()),
  )
  partyName.value = ''
}

async function assignCharacter(): Promise<void> {
  if (!campaign.value || !partyCharacterId.value) return
  await runAction('party:assign', () =>
    assignCampaignPartyCharacter(
      campaign.value!.id,
      partyCharacterId.value!,
      partyControllerId.value ?? undefined,
    ),
  )
  partyCharacterId.value = null
}

async function archive(): Promise<void> {
  if (!campaign.value) return
  archiveDialog.value = false
  await runAction('archive', () => archiveCampaign(campaign.value!.id))
}

async function leave(): Promise<void> {
  if (!campaign.value) return
  actionKey.value = 'leave'
  errorMessages.value = []
  try {
    await leaveCampaign(campaign.value.id)
    await router.replace({ name: 'campaigns' })
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
    actionKey.value = null
  }
}

watch(campaignId, load)
onMounted(load)
</script>

<template>
  <section class="campaign-details">
    <v-progress-linear v-if="isLoading" color="accent" indeterminate rounded />
    <v-alert v-for="message in errorMessages" :key="message" type="error" variant="tonal">
      {{ message }}
      <template #append
        ><v-btn variant="text" @click="load">{{ t('common.retry') }}</v-btn></template
      >
    </v-alert>

    <template v-if="campaign">
      <header class="campaign-header">
        <div>
          <p class="eyebrow">{{ t('campaigns.campaignEyebrow') }}</p>
          <h1>{{ campaign.name }}</h1>
          <div class="roles">
            <v-chip v-for="role in campaign.roles" :key="role" size="small" variant="tonal">
              {{ t(`campaigns.roles.${role}`) }}
            </v-chip>
          </div>
        </div>
        <v-btn
          v-if="isGameMaster && campaign.status === 'Active'"
          color="warning"
          :loading="actionKey === 'archive'"
          variant="tonal"
          @click="archiveDialog = true"
          >{{ t('campaigns.archive') }}</v-btn
        >
      </header>

      <v-tabs v-model="activeTab" color="primary" show-arrows>
        <v-tab value="overview">
          <v-badge
            v-if="pendingOperations.count.value"
            color="secondary"
            :content="pendingOperations.count.value"
          >
            {{ t('campaigns.tabs.overview') }}
          </v-badge>
          <template v-else>{{ t('campaigns.tabs.overview') }}</template>
        </v-tab>
        <v-tab value="members">{{ t('campaigns.tabs.members') }}</v-tab>
        <v-tab value="party">{{ t('campaigns.tabs.party') }}</v-tab>
        <v-tab value="storage">{{ t('campaigns.tabs.storage') }}</v-tab>
        <v-tab value="commerce">{{ t('campaigns.tabs.commerce') }}</v-tab>
      </v-tabs>

      <v-window v-model="activeTab">
        <v-window-item value="overview">
          <v-card class="panel" elevation="0">
            <v-card-title>{{ t('campaigns.overviewTitle') }}</v-card-title>
            <v-card-text class="stack">
              <p>{{ t(`campaigns.statuses.${campaign.status}`) }}</p>
              <p v-if="activeParty">{{ t('campaigns.activeParty', { name: activeParty.name }) }}</p>
              <p v-else>{{ t('campaigns.noActiveParty') }}</p>
              <div v-if="pendingCharacterIds.length" class="row">
                <span>
                  {{
                    t('campaigns.pendingOperations', {
                      count: pendingOperations.count.value,
                    })
                  }}
                </span>
                <v-btn
                  :loading="pendingOperations.isLoading.value"
                  size="small"
                  variant="text"
                  @click="pendingOperations.refresh"
                >
                  {{ t('common.refresh') }}
                </v-btn>
              </div>
              <v-alert
                v-for="message in pendingOperations.errors.value"
                :key="message"
                type="error"
                variant="tonal"
              >
                {{ message }}
              </v-alert>
            </v-card-text>
          </v-card>
        </v-window-item>

        <v-window-item value="members">
          <v-card class="panel" elevation="0">
            <v-card-title>{{ t('campaigns.members') }}</v-card-title>
            <v-card-text class="stack">
              <div v-for="member in campaign.members" :key="member.userId" class="row">
                <span>{{ t('campaigns.userId', { userId: member.userId }) }}</span>
                <v-chip v-for="role in member.roles" :key="role" size="x-small" variant="outlined">
                  {{ t(`campaigns.roles.${role}`) }}
                </v-chip>
                <v-btn
                  v-if="isGameMaster"
                  :loading="actionKey === `role:${member.userId}`"
                  size="x-small"
                  variant="text"
                  @click="toggleGameMaster(member.userId, !member.roles.includes('GameMaster'))"
                  >{{
                    member.roles.includes('GameMaster')
                      ? t('campaigns.revokeGameMaster')
                      : t('campaigns.assignGameMaster')
                  }}</v-btn
                >
              </div>
              <v-form
                v-if="isGameMaster && campaign.status === 'Active'"
                class="inline-form"
                @submit.prevent="invite"
              >
                <v-text-field
                  v-model="invitedUserName"
                  density="compact"
                  :label="t('campaigns.invitedUserName')"
                />
                <v-btn
                  :disabled="!isCampaignUserNameValid(invitedUserName)"
                  :loading="actionKey === 'invite'"
                  type="submit"
                  >{{ t('campaigns.invite') }}</v-btn
                >
              </v-form>
              <v-btn
                v-if="campaign.status === 'Active'"
                color="warning"
                :loading="actionKey === 'leave'"
                variant="text"
                @click="leave"
                >{{ t('campaigns.leave') }}</v-btn
              >
            </v-card-text>
          </v-card>
        </v-window-item>

        <v-window-item value="party">
          <v-card class="panel" elevation="0">
            <v-card-title>{{ t('campaigns.tabs.party') }}</v-card-title>
            <v-card-text class="stack">
              <v-form
                v-if="isGameMaster && !activeParty"
                class="inline-form"
                @submit.prevent="createParty"
              >
                <v-text-field v-model="partyName" :label="t('campaigns.partyName')" />
                <v-btn
                  :disabled="!partyName.trim()"
                  :loading="actionKey === 'party:create'"
                  type="submit"
                >
                  {{ t('campaigns.createParty') }}
                </v-btn>
              </v-form>
              <template v-if="activeParty">
                <p>{{ t('campaigns.activeParty', { name: activeParty.name }) }}</p>
                <div v-for="character in activeParty.characters" :key="character.id" class="row">
                  <v-btn
                    :to="`/characters/${character.characterId}?campaignId=${campaign.id}`"
                    variant="text"
                    >{{ characterName(character.characterId) }}</v-btn
                  >
                  <v-btn
                    prepend-icon="mdi-bag-personal-outline"
                    size="small"
                    :to="{
                      name: 'campaign-inventory',
                      params: {
                        campaignId: campaign.id,
                        characterId: character.characterId,
                      },
                    }"
                    variant="tonal"
                  >
                    {{ t('campaigns.inventory') }}
                  </v-btn>
                  <v-chip size="x-small" variant="outlined">
                    {{ t('campaigns.controlledBy', { userId: character.controlledByUserId }) }}
                  </v-chip>
                </div>
                <v-form class="party-form" @submit.prevent="assignCharacter">
                  <v-select
                    v-if="!partyControllerId || partyControllerId === campaign.currentUserId"
                    v-model="partyCharacterId"
                    item-title="name"
                    item-value="id"
                    :items="characters"
                    :label="t('campaigns.character')"
                  />
                  <v-text-field
                    v-else
                    v-model.number="partyCharacterId"
                    min="1"
                    :label="t('campaigns.characterId')"
                    type="number"
                  />
                  <v-select
                    v-if="isGameMaster"
                    v-model="partyControllerId"
                    clearable
                    item-title="userId"
                    item-value="userId"
                    :items="playerMembers"
                    :label="t('campaigns.controller')"
                  />
                  <v-btn
                    :disabled="!partyCharacterId"
                    :loading="actionKey === 'party:assign'"
                    type="submit"
                  >
                    {{ t('campaigns.assignCharacter') }}
                  </v-btn>
                </v-form>
              </template>
            </v-card-text>
          </v-card>
        </v-window-item>

        <v-window-item value="storage">
          <PartyStorageTab :campaign="campaign" :characters="characters" />
        </v-window-item>
        <v-window-item value="commerce">
          <CommerceCampaignTab :campaign-id="campaignId" :is-game-master="isGameMaster" />
        </v-window-item>
      </v-window>
    </template>

    <v-dialog v-model="archiveDialog" max-width="460">
      <v-card>
        <v-card-title>{{ t('campaigns.archiveConfirmTitle') }}</v-card-title>
        <v-card-text>{{ t('campaigns.archiveConfirmText') }}</v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="archiveDialog = false">{{ t('common.cancel') }}</v-btn>
          <v-btn color="warning" @click="archive">{{ t('campaigns.archive') }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </section>
</template>

<style scoped>
.campaign-details,
.stack {
  display: grid;
  gap: 16px;
}

.campaign-header,
.row,
.roles {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px;
}

.campaign-header {
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
  margin: 0 0 8px;
  color: rgb(var(--v-theme-primary));
  font-family: Georgia, 'Times New Roman', serif;
  font-size: clamp(2rem, 5vw, 3rem);
}

.panel {
  margin-top: 16px;
  border: 1px solid rgb(var(--v-theme-surface-variant));
}

.inline-form,
.party-form {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto;
  gap: 12px;
  align-items: start;
}

.party-form {
  grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
}

@media (max-width: 600px) {
  .inline-form {
    grid-template-columns: 1fr;
  }
}
</style>
