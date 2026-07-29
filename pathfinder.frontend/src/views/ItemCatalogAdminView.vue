<script setup lang="ts">
import axios from 'axios'
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import MoneyText from '@/components/MoneyText.vue'
import { useSnackbar } from '@/composables/useSnackbar'
import { useAuthStore } from '@/features/auth/store'
import { getCampaigns, type Campaign } from '@/features/campaigns/api'
import {
  searchPublishedItemRevisions,
  type PublishedItemRevision,
} from '@/features/commerce/adminApi'
import {
  createConfiguration,
  createDraft,
  createRevisionDraft,
  getAdminDefinitions,
  publishRevision,
  retireRevision,
  type AdminItemDefinition,
  type AdminItemRevision,
  type ItemCatalogScopeFilter,
  type ItemMaterialGrade,
  type ItemMaterialType,
  type ItemRevisionStatus,
  type ItemSize,
  type PermanentUpgradeRequest,
} from '@/features/item-catalog/api'
import ConfigurationDialog from '@/features/item-catalog/ConfigurationDialog.vue'
import DraftEditorDialog from '@/features/item-catalog/DraftEditorDialog.vue'
import { buildRulesRequest, type DraftFormModel } from '@/features/item-catalog/draftForm'
import { catalogEmptyReason } from '@/features/item-catalog/emptyState'
import {
  filterDefinitions,
  resetFilters,
  type CatalogFilters,
} from '@/features/item-catalog/filters'
import {
  canConfigure,
  canManageDefinition,
  canPublish,
  canRetire,
  latestDraft,
  newRevisionBlockReason,
  publishConsequence,
  type CatalogMode,
} from '@/features/item-catalog/lifecycle'
import {
  configurationSummary,
  type ConfigurationSummaryInput,
} from '@/features/item-catalog/options'
import {
  getItemCategoryLabel,
  getItemRarityLabel,
  getItemRevisionStatusLabel,
  getItemScopeLabel,
} from '@/i18n/domain'

const route = useRoute()
const router = useRouter()
const { d, t } = useI18n()
const snackbar = useSnackbar()
const auth = useAuthStore()

const mode = computed<CatalogMode>(() => (route.params.campaignId ? 'campaign' : 'global'))
const campaignId = computed(() => Number(route.params.campaignId) || null)

const campaign = ref<Campaign | null>(null)
const definitions = ref<AdminItemDefinition[]>([])
const totalCount = ref(0)
const configurationsByRevision = ref<Map<number, ConfigurationSummaryInput[]>>(new Map())
const search = ref('')
const statusFilter = ref<ItemRevisionStatus | 'All'>('All')
const scopeFilter = ref<ItemCatalogScopeFilter>('All')
const errors = ref<string[]>([])
const actionErrors = ref<string[]>([])
const isLoading = ref(true)
const isSaving = ref(false)

const draftDialog = ref(false)
const draftTarget = ref<AdminItemDefinition | null>(null)
const configurationDialog = ref(false)
const configurationDefinition = ref<AdminItemDefinition | null>(null)
const configurationRevision = ref<AdminItemRevision | null>(null)
const publishDialog = ref(false)
const retireDialog = ref(false)
const lifecycleDefinition = ref<AdminItemDefinition | null>(null)
const lifecycleRevision = ref<AdminItemRevision | null>(null)

const clientFilters = computed<CatalogFilters>(() => ({
  search: '',
  status: statusFilter.value,
}))
const visibleDefinitions = computed(() => filterDefinitions(definitions.value, clientFilters.value))
const emptyReason = computed(() =>
  isLoading.value || errors.value.length
    ? null
    : catalogEmptyReason(definitions.value, {
        search: search.value,
        status: statusFilter.value,
        scope: scopeFilter.value,
      }),
)
const publishReplaced = computed(() =>
  lifecycleDefinition.value ? publishConsequence(lifecycleDefinition.value) : null,
)
const statusChips: (ItemRevisionStatus | 'All')[] = ['All', 'Draft', 'Published', 'Retired']

function isForbidden(error: unknown): boolean {
  return axios.isAxiosError(error) && error.response?.status === 403
}

async function redirectAway(): Promise<void> {
  snackbar.error(t('itemCatalogUi.accessDenied'))
  if (mode.value === 'campaign' && campaignId.value) {
    await router.replace({ name: 'campaign-details', params: { campaignId: campaignId.value } })
  } else {
    await router.replace({ name: 'characters' })
  }
}

async function load(): Promise<void> {
  isLoading.value = true
  errors.value = []
  try {
    if (mode.value === 'campaign' && campaignId.value) {
      const campaigns = await getCampaigns()
      const current = campaigns.find((item) => item.id === campaignId.value) ?? null
      if (!current || current.status !== 'Active' || !current.roles.includes('GameMaster')) {
        await redirectAway()
        return
      }
      campaign.value = current
    } else {
      await auth.loadCapabilities()
      if (!auth.capabilities?.canManageGlobalCatalog) {
        await redirectAway()
        return
      }
      campaign.value = null
    }
    const list = await getAdminDefinitions({
      scope: mode.value === 'campaign' ? scopeFilter.value : 'All',
      campaignId: campaignId.value ?? undefined,
      search: search.value.trim() || undefined,
      skip: 0,
      take: 200,
    })
    definitions.value = list.items
    totalCount.value = list.totalCount
    await loadConfigurations()
  } catch (error) {
    if (isForbidden(error)) {
      await redirectAway()
    } else {
      errors.value = getApiErrorMessages(error)
    }
  } finally {
    isLoading.value = false
  }
}

async function loadConfigurations(): Promise<void> {
  if (mode.value !== 'campaign' || !campaignId.value) {
    configurationsByRevision.value = new Map()
    return
  }
  const revisions: PublishedItemRevision[] = await searchPublishedItemRevisions(
    campaignId.value,
    '',
  )
  const map = new Map<number, ConfigurationSummaryInput[]>()
  for (const revision of revisions) {
    map.set(revision.itemRevisionId, revision.configurations)
  }
  configurationsByRevision.value = map
}

function configurationsFor(revision: AdminItemRevision): ConfigurationSummaryInput[] {
  return configurationsByRevision.value.get(revision.itemRevisionId) ?? []
}

function summaryLabel(configuration: ConfigurationSummaryInput): string {
  return configurationSummary(configuration, (group, code) => t(`itemCatalogUi.${group}.${code}`))
}

function openCreateDialog(): void {
  draftTarget.value = null
  actionErrors.value = []
  draftDialog.value = true
}

function openRevisionDialog(definition: AdminItemDefinition): void {
  draftTarget.value = definition
  actionErrors.value = []
  draftDialog.value = true
}

async function saveDraft(model: DraftFormModel): Promise<void> {
  if (isSaving.value) return
  isSaving.value = true
  actionErrors.value = []
  try {
    const base = {
      name: model.name,
      description: model.description,
      level: model.level,
      priceInCopperPieces: model.priceInCopperPieces,
      bulk: model.bulk,
      rules: buildRulesRequest(model),
    }
    if (draftTarget.value) {
      await createRevisionDraft(draftTarget.value.itemDefinitionId, base)
    } else {
      await createDraft({
        ...base,
        scope: mode.value === 'campaign' ? 'Campaign' : 'Global',
        campaignId: campaignId.value,
        key: model.key,
      })
    }
    draftDialog.value = false
    snackbar.success(t('itemCatalogUi.draft.created'))
    await load()
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
  } finally {
    isSaving.value = false
  }
}

function openPublishDialog(definition: AdminItemDefinition, revision: AdminItemRevision): void {
  lifecycleDefinition.value = definition
  lifecycleRevision.value = revision
  actionErrors.value = []
  publishDialog.value = true
}

function openRetireDialog(definition: AdminItemDefinition, revision: AdminItemRevision): void {
  lifecycleDefinition.value = definition
  lifecycleRevision.value = revision
  actionErrors.value = []
  retireDialog.value = true
}

async function confirmPublish(): Promise<void> {
  if (!lifecycleDefinition.value || !lifecycleRevision.value || isSaving.value) return
  isSaving.value = true
  actionErrors.value = []
  try {
    await publishRevision(
      lifecycleDefinition.value.itemDefinitionId,
      lifecycleRevision.value.revisionNumber,
    )
    publishDialog.value = false
    snackbar.success(t('itemCatalogUi.publish.published'))
    await load()
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
  } finally {
    isSaving.value = false
  }
}

async function confirmRetire(): Promise<void> {
  if (!lifecycleDefinition.value || !lifecycleRevision.value || isSaving.value) return
  isSaving.value = true
  actionErrors.value = []
  try {
    await retireRevision(
      lifecycleDefinition.value.itemDefinitionId,
      lifecycleRevision.value.revisionNumber,
    )
    retireDialog.value = false
    snackbar.success(t('itemCatalogUi.retire.retired'))
    await load()
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
  } finally {
    isSaving.value = false
  }
}

function openConfigurationDialog(
  definition: AdminItemDefinition,
  revision: AdminItemRevision,
): void {
  configurationDefinition.value = definition
  configurationRevision.value = revision
  actionErrors.value = []
  configurationDialog.value = true
}

async function saveConfiguration(shape: {
  size: string
  materialType: string
  materialGrade: string
  permanentUpgrades: PermanentUpgradeRequest[]
}): Promise<void> {
  if (!campaignId.value || !configurationDefinition.value || !configurationRevision.value) return
  if (isSaving.value) return
  isSaving.value = true
  actionErrors.value = []
  try {
    const result = await createConfiguration(campaignId.value, {
      itemDefinitionId: configurationDefinition.value.itemDefinitionId,
      revisionNumber: configurationRevision.value.revisionNumber,
      size: shape.size as ItemSize,
      materialType: shape.materialType as ItemMaterialType,
      materialGrade: shape.materialGrade as ItemMaterialGrade,
      permanentUpgrades: shape.permanentUpgrades,
    })
    configurationDialog.value = false
    if (result.wasCreated) {
      snackbar.success(t('itemCatalogUi.configuration.created'))
    } else {
      snackbar.info(t('itemCatalogUi.configuration.alreadyExists'))
    }
    await loadConfigurations()
  } catch (error) {
    actionErrors.value = getApiErrorMessages(error)
  } finally {
    isSaving.value = false
  }
}

function resetAllFilters(): void {
  search.value = ''
  statusFilter.value = resetFilters().status
  scopeFilter.value = 'All'
  void load()
}

function handleDeepLink(): void {
  const action = String(route.query.action ?? '')
  if (!action) return
  if (action === 'create-draft') {
    openCreateDialog()
  }
  if (action === 'configure') {
    const status = String(route.query.status ?? '')
    if (status === 'Published') {
      statusFilter.value = 'Published'
    }
  }
  const rest = { ...route.query }
  delete rest.action
  delete rest.status
  void router.replace({ query: rest })
}

watch(
  () => [route.name, route.params.campaignId],
  () => {
    if (route.name?.toString().includes('item-catalog')) {
      void load()
    }
  },
)

watch(scopeFilter, () => {
  void load()
})

onMounted(async () => {
  await load()
  handleDeepLink()
})
</script>

<template>
  <section class="catalog">
    <header class="page-head">
      <div>
        <p class="eyebrow">
          {{
            mode === 'campaign'
              ? t('itemCatalogUi.eyebrowCampaign', { name: campaign?.name ?? '' })
              : t('itemCatalogUi.eyebrowGlobal')
          }}
        </p>
        <h1>{{ t('itemCatalogUi.title') }}</h1>
      </div>
      <div class="page-head__actions">
        <v-btn color="primary" prepend-icon="mdi-plus" @click="openCreateDialog">
          {{ t('itemCatalogUi.createDefinition') }}
        </v-btn>
        <v-btn icon="mdi-refresh" :loading="isLoading" variant="text" @click="load" />
      </div>
    </header>

    <v-progress-linear v-if="isLoading" color="accent" indeterminate rounded />
    <v-alert v-for="message in errors" :key="message" type="error" variant="tonal">
      {{ message }}
      <template #append>
        <v-btn variant="text" @click="load">{{ t('common.retry') }}</v-btn>
      </template>
    </v-alert>

    <v-card class="panel" elevation="0">
      <v-card-text class="filters">
        <div class="filters__row">
          <v-text-field
            v-model="search"
            append-inner-icon="mdi-magnify"
            class="filters__search"
            density="comfortable"
            hide-details
            :label="t('itemCatalogUi.filters.search')"
            :loading="isLoading"
            @click:append-inner="load"
            @keyup.enter="load"
          />
          <v-btn-toggle
            v-if="mode === 'campaign'"
            v-model="scopeFilter"
            density="comfortable"
            divided
            mandatory
            variant="outlined"
          >
            <v-btn size="small" value="All">{{ t('itemCatalogUi.scopes.All') }}</v-btn>
            <v-btn size="small" value="Global">{{ t('itemCatalogUi.scopes.Global') }}</v-btn>
            <v-btn size="small" value="Campaign">{{ t('itemCatalogUi.scopes.Campaign') }}</v-btn>
          </v-btn-toggle>
        </div>
        <div class="filters__status">
          <span class="filters__label">{{ t('itemCatalogUi.filters.status') }}</span>
          <v-chip-group v-model="statusFilter" mandatory selected-class="text-primary">
            <v-chip v-for="chip in statusChips" :key="chip" filter :value="chip">
              {{
                chip === 'All' ? t('itemCatalogUi.filters.all') : getItemRevisionStatusLabel(chip)
              }}
            </v-chip>
          </v-chip-group>
        </div>
      </v-card-text>
    </v-card>

    <v-empty-state
      v-if="emptyReason === 'noDefinitions'"
      icon="mdi-book-plus-outline"
      :text="t('itemCatalogUi.empty.noDefinitionsText')"
      :title="t('itemCatalogUi.empty.noDefinitionsTitle')"
    >
      <template #actions>
        <v-btn color="primary" @click="openCreateDialog">
          {{ t('itemCatalogUi.createDefinition') }}
        </v-btn>
      </template>
    </v-empty-state>
    <v-empty-state
      v-else-if="!isLoading && !errors.length && !visibleDefinitions.length"
      icon="mdi-filter-off-outline"
      :text="t('itemCatalogUi.empty.noMatchesText')"
      :title="t('itemCatalogUi.empty.noMatchesTitle')"
    >
      <template #actions>
        <v-btn variant="tonal" @click="resetAllFilters">
          {{ t('itemCatalogUi.filters.reset') }}
        </v-btn>
      </template>
    </v-empty-state>

    <v-card
      v-for="definition in visibleDefinitions"
      :key="definition.itemDefinitionId"
      class="panel definition"
      elevation="0"
    >
      <v-card-text>
        <div class="definition__head">
          <div class="definition__title">
            <h2>{{ definition.revisions.at(-1)?.name ?? definition.key }}</h2>
            <code class="definition__key">{{ definition.key }}</code>
          </div>
          <v-chip
            size="small"
            :variant="definition.scope === 'Global' ? 'outlined' : 'flat'"
            :color="definition.scope === 'Global' ? 'info' : 'secondary'"
          >
            {{ getItemScopeLabel(definition.scope) }}
          </v-chip>
          <template v-if="canManageDefinition(definition, mode)">
            <v-btn
              :aria-describedby="
                newRevisionBlockReason(definition, mode) === 'draftExists'
                  ? `new-revision-hint-${definition.itemDefinitionId}`
                  : undefined
              "
              :disabled="newRevisionBlockReason(definition, mode) !== null"
              prepend-icon="mdi-plus"
              size="small"
              variant="outlined"
              @click="openRevisionDialog(definition)"
            >
              {{ t('itemCatalogUi.newRevision') }}
            </v-btn>
            <span
              v-if="newRevisionBlockReason(definition, mode) === 'draftExists'"
              :id="`new-revision-hint-${definition.itemDefinitionId}`"
              class="hint"
            >
              {{
                t('itemCatalogUi.hints.draftAlreadyExists', {
                  number: latestDraft(definition)?.revisionNumber,
                })
              }}
            </span>
          </template>
          <span v-else class="hint">{{ t('itemCatalogUi.hints.globalReadOnly') }}</span>
        </div>

        <v-expansion-panels multiple variant="accordion">
          <v-expansion-panel
            v-for="revision in [...definition.revisions].reverse()"
            :key="revision.itemRevisionId"
          >
            <v-expansion-panel-title>
              <div class="revision-row">
                <span class="revision-row__number">
                  {{ t('itemCatalogUi.list.revision', { number: revision.revisionNumber }) }}
                </span>
                <span class="revision-row__name">{{ revision.name }}</span>
                <span class="revision-row__meta">
                  {{ t('itemCatalogUi.list.level', { level: revision.level }) }}
                  · {{ getItemCategoryLabel(revision.primaryCategory) }} ·
                  {{ getItemRarityLabel(revision.rarity) }}
                </span>
                <MoneyText class="revision-row__price" :copper="revision.priceInCopperPieces" />
                <v-chip
                  size="x-small"
                  :color="
                    revision.status === 'Published'
                      ? 'success'
                      : revision.status === 'Draft'
                        ? 'secondary'
                        : undefined
                  "
                >
                  {{ getItemRevisionStatusLabel(revision.status) }}
                </v-chip>
              </div>
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <div class="revision-detail">
                <p v-if="revision.description" class="revision-detail__description">
                  {{ revision.description }}
                </p>
                <p class="revision-detail__dates">
                  {{ t('itemCatalogUi.list.bulk', { bulk: revision.bulk }) }}
                  ·
                  {{
                    t('itemCatalogUi.list.createdAt', {
                      date: d(new Date(revision.createdAtUtc), 'short'),
                    })
                  }}
                  <template v-if="revision.publishedAtUtc">
                    ·
                    {{
                      t('itemCatalogUi.list.publishedAt', {
                        date: d(new Date(revision.publishedAtUtc), 'short'),
                      })
                    }}
                  </template>
                  <template v-if="revision.retiredAtUtc">
                    ·
                    {{
                      t('itemCatalogUi.list.retiredAt', {
                        date: d(new Date(revision.retiredAtUtc), 'short'),
                      })
                    }}
                  </template>
                </p>

                <div class="revision-detail__actions">
                  <template v-if="canManageDefinition(definition, mode)">
                    <v-btn
                      v-if="canPublish(revision)"
                      color="warning"
                      size="small"
                      @click="openPublishDialog(definition, revision)"
                    >
                      {{ t('itemCatalogUi.publish.action') }}
                    </v-btn>
                    <v-btn
                      v-if="canRetire(revision)"
                      color="error"
                      size="small"
                      variant="outlined"
                      @click="openRetireDialog(definition, revision)"
                    >
                      {{ t('itemCatalogUi.retire.action') }}
                    </v-btn>
                    <span v-if="revision.status === 'Retired'" class="hint">
                      {{ t('itemCatalogUi.hints.retiredImmutable') }}
                    </span>
                  </template>
                </div>

                <div v-if="canConfigure(revision, mode)" class="configurations">
                  <h3>{{ t('itemCatalogUi.list.configurations') }}</h3>
                  <p v-if="!configurationsFor(revision).length" class="hint">
                    {{ t('itemCatalogUi.list.noConfigurations') }}
                  </p>
                  <div class="configurations__chips">
                    <v-chip
                      v-for="(configuration, index) in configurationsFor(revision)"
                      :key="index"
                      size="small"
                      variant="tonal"
                    >
                      {{ summaryLabel(configuration) }}
                    </v-chip>
                    <v-btn
                      color="secondary"
                      prepend-icon="mdi-plus"
                      size="small"
                      variant="tonal"
                      @click="openConfigurationDialog(definition, revision)"
                    >
                      {{ t('itemCatalogUi.addConfiguration') }}
                    </v-btn>
                  </div>
                </div>
              </div>
            </v-expansion-panel-text>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-card-text>
    </v-card>

    <p v-if="totalCount > definitions.length" class="hint">
      {{ t('itemCatalogUi.list.totalCount', { count: totalCount }) }}
    </p>

    <DraftEditorDialog
      v-model="draftDialog"
      :campaign-name="campaign?.name ?? null"
      :catalog-mode="mode"
      :definition="draftTarget"
      :errors="actionErrors"
      :is-saving="isSaving"
      @submit="saveDraft"
    />

    <ConfigurationDialog
      v-model="configurationDialog"
      :context-name="configurationRevision?.name ?? ''"
      :errors="actionErrors"
      :existing-configurations="
        configurationRevision ? configurationsFor(configurationRevision) : []
      "
      :is-saving="isSaving"
      :revision-number="configurationRevision?.revisionNumber ?? 0"
      @submit="saveConfiguration"
    />

    <v-dialog v-model="publishDialog" max-width="460" persistent>
      <v-card>
        <v-card-title>
          {{
            t('itemCatalogUi.publish.confirmTitle', {
              number: lifecycleRevision?.revisionNumber,
            })
          }}
        </v-card-title>
        <v-card-text class="form-stack">
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <p>{{ t('itemCatalogUi.publish.confirmText') }}</p>
          <v-alert v-if="publishReplaced" type="warning" variant="tonal">
            {{
              t('itemCatalogUi.publish.confirmReplaceText', {
                replaced: publishReplaced.revisionNumber,
              })
            }}
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isSaving" variant="text" @click="publishDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn color="warning" :loading="isSaving" @click="confirmPublish">
            {{ t('itemCatalogUi.publish.action') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="retireDialog" max-width="460" persistent>
      <v-card>
        <v-card-title>
          {{
            t('itemCatalogUi.retire.confirmTitle', {
              number: lifecycleRevision?.revisionNumber,
            })
          }}
        </v-card-title>
        <v-card-text class="form-stack">
          <v-alert v-for="message in actionErrors" :key="message" type="error" variant="tonal">
            {{ message }}
          </v-alert>
          <p>{{ t('itemCatalogUi.retire.confirmText') }}</p>
          <v-alert type="warning" variant="tonal">
            {{ t('itemCatalogUi.retire.irreversible') }}
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="isSaving" variant="text" @click="retireDialog = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn color="error" :loading="isSaving" @click="confirmRetire">
            {{ t('itemCatalogUi.retire.action') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </section>
</template>

<style scoped>
.catalog {
  display: grid;
  gap: 16px;
}

.page-head {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 12px;
  flex-wrap: wrap;
}

.page-head h1 {
  margin: 0;
  color: rgb(var(--v-theme-primary));
  font-family: Georgia, 'Times New Roman', serif;
}

.page-head__actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.eyebrow {
  margin: 0;
  color: rgb(var(--v-theme-secondary));
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.panel {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}

.filters {
  display: grid;
  gap: 8px;
}

.filters__row {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.filters__search {
  flex: 1 1 260px;
}

.filters__status {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.filters__label {
  color: rgb(var(--v-theme-on-surface-variant));
  font-size: 0.85rem;
}

.definition__head {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
  margin-bottom: 10px;
}

.definition__title {
  flex: 1 1 auto;
  min-width: 200px;
}

.definition__title h2 {
  margin: 0;
  font-family: Georgia, 'Times New Roman', serif;
  font-size: 1.15rem;
  color: rgb(var(--v-theme-primary));
}

.definition__key {
  color: rgb(var(--v-theme-on-surface-variant));
  font-size: 0.75rem;
}

.revision-row {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
  width: 100%;
}

.revision-row__number {
  font-weight: 700;
  color: rgb(var(--v-theme-primary));
  min-width: 34px;
}

.revision-row__name {
  flex: 1 1 auto;
  min-width: 140px;
}

.revision-row__meta {
  color: rgb(var(--v-theme-on-surface-variant));
  font-size: 0.8rem;
}

.revision-row__price {
  font-weight: 600;
}

.revision-detail {
  display: grid;
  gap: 10px;
}

.revision-detail__description {
  margin: 0;
  font-size: 0.9rem;
}

.revision-detail__dates {
  margin: 0;
  color: rgb(var(--v-theme-on-surface-variant));
  font-size: 0.8rem;
}

.revision-detail__actions {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.configurations {
  border-top: 1px dashed rgb(var(--v-theme-surface-variant));
  padding-top: 10px;
  display: grid;
  gap: 8px;
}

.configurations h3 {
  margin: 0;
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: rgb(var(--v-theme-on-surface-variant));
}

.configurations__chips {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.hint {
  color: rgb(var(--v-theme-on-surface-variant));
  font-size: 0.75rem;
  font-style: italic;
}

.form-stack {
  display: grid;
  gap: 12px;
}
</style>
