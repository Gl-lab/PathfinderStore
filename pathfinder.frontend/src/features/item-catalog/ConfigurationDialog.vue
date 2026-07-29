<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import type { ItemMaterialGrade, ItemMaterialType, ItemSize, PermanentUpgradeRequest } from './api'
import {
  enumOptions,
  isDuplicateConfiguration,
  upgradeValidationErrors,
  type ConfigurationShape,
  type ConfigurationSummaryInput,
} from './options'

const props = defineProps<{
  contextName: string
  revisionNumber: number
  existingConfigurations: ConfigurationSummaryInput[]
  isSaving: boolean
  errors: string[]
}>()
const open = defineModel<boolean>({ required: true })
const emit = defineEmits<{ submit: [shape: ConfigurationShape] }>()
const { t } = useI18n()

const size = ref<ItemSize>('Medium')
const materialType = ref<ItemMaterialType>('Standard')
const materialGrade = ref<ItemMaterialGrade>('Standard')
const upgrades = ref<PermanentUpgradeRequest[]>([])

watch(open, (isOpen) => {
  if (!isOpen) return
  size.value = 'Medium'
  materialType.value = 'Standard'
  materialGrade.value = 'Standard'
  upgrades.value = []
})

const shape = computed<ConfigurationShape>(() => ({
  size: size.value,
  materialType: materialType.value,
  materialGrade: materialGrade.value,
  permanentUpgrades: upgrades.value,
}))
const validationKeys = computed(() => upgradeValidationErrors(upgrades.value))
const isDuplicate = computed(() =>
  isDuplicateConfiguration(
    props.existingConfigurations.filter((configuration) => configuration.campaignId !== null),
    shape.value,
  ),
)
const canSubmit = computed(
  () => validationKeys.value.length === 0 && !isDuplicate.value && !props.isSaving,
)

const sizeOptions = computed(() =>
  enumOptions(['Tiny', 'Small', 'Medium', 'Large', 'Huge', 'Gargantuan'] as const, (value) =>
    t(`itemCatalogUi.sizes.${value}`),
  ),
)
const materialOptions = computed(() =>
  enumOptions(['Standard', 'ColdIron', 'Silver', 'Adamantine', 'Darkwood'] as const, (value) =>
    t(`itemCatalogUi.materials.${value}`),
  ),
)
const gradeOptions = computed(() =>
  enumOptions(['Low', 'Standard', 'High'] as const, (value) => t(`itemCatalogUi.grades.${value}`)),
)
const kindOptions = computed(() =>
  enumOptions(
    [
      'WeaponPotencyRune',
      'StrikingRune',
      'ArmorPotencyRune',
      'ResilientRune',
      'PropertyRune',
      'TypedEffect',
    ] as const,
    (value) => t(`itemCatalogUi.upgradeKinds.${value}`),
  ),
)
const visibilityOptions = computed(() =>
  enumOptions(['Public', 'Hidden'] as const, (value) =>
    t(`itemCatalogUi.upgradeVisibilities.${value}`),
  ),
)

function addUpgrade(): void {
  upgrades.value.push({ code: '', kind: 'PropertyRune', rank: 1, visibility: 'Public' })
}

function removeUpgrade(index: number): void {
  upgrades.value.splice(index, 1)
}

function submit(): void {
  if (!canSubmit.value) return
  emit('submit', shape.value)
}
</script>

<template>
  <v-dialog v-model="open" max-width="620" persistent>
    <v-card>
      <v-card-title>{{ t('itemCatalogUi.configuration.title') }}</v-card-title>
      <v-card-subtitle>
        {{
          t('itemCatalogUi.configuration.context', {
            name: contextName,
            number: revisionNumber,
          })
        }}
      </v-card-subtitle>
      <v-card-text class="form-stack">
        <v-alert v-for="message in errors" :key="message" type="error" variant="tonal">
          {{ message }}
        </v-alert>
        <div class="field-row">
          <v-select
            v-model="size"
            item-title="title"
            item-value="value"
            :items="sizeOptions"
            :label="t('itemCatalogUi.configuration.size')"
          />
          <v-select
            v-model="materialType"
            item-title="title"
            item-value="value"
            :items="materialOptions"
            :label="t('itemCatalogUi.configuration.material')"
          />
          <v-select
            v-model="materialGrade"
            item-title="title"
            item-value="value"
            :items="gradeOptions"
            :label="t('itemCatalogUi.configuration.grade')"
          />
        </div>

        <section class="upgrades">
          <header class="upgrades__header">
            <h3>{{ t('itemCatalogUi.configuration.upgrades') }}</h3>
            <v-btn size="small" variant="text" @click="addUpgrade">
              {{ t('itemCatalogUi.configuration.addUpgrade') }}
            </v-btn>
          </header>
          <div v-for="(upgrade, index) in upgrades" :key="index" class="field-row upgrade-row">
            <v-text-field
              v-model="upgrade.code"
              :label="t('itemCatalogUi.configuration.upgradeCode')"
            />
            <v-select
              v-model="upgrade.kind"
              item-title="title"
              item-value="value"
              :items="kindOptions"
              :label="t('itemCatalogUi.configuration.upgradeKind')"
            />
            <v-number-input
              v-model="upgrade.rank"
              :label="t('itemCatalogUi.configuration.upgradeRank')"
              :min="1"
            />
            <v-select
              v-model="upgrade.visibility"
              item-title="title"
              item-value="value"
              :items="visibilityOptions"
              :label="t('itemCatalogUi.configuration.upgradeVisibility')"
            />
            <v-btn
              :aria-label="t('common.delete')"
              icon="mdi-close"
              size="small"
              variant="text"
              @click="removeUpgrade(index)"
            />
          </div>
        </section>

        <v-alert v-if="validationKeys.length" type="warning" variant="tonal">
          <ul class="validation-list">
            <li v-for="key in validationKeys" :key="key">{{ t(key) }}</li>
          </ul>
        </v-alert>
        <v-alert
          v-if="isDuplicate"
          id="configuration-duplicate-hint"
          type="warning"
          variant="tonal"
        >
          {{ t('itemCatalogUi.configuration.duplicateHint') }}
        </v-alert>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn :disabled="isSaving" variant="text" @click="open = false">
          {{ t('common.cancel') }}
        </v-btn>
        <v-btn
          :aria-describedby="isDuplicate ? 'configuration-duplicate-hint' : undefined"
          color="secondary"
          :disabled="!canSubmit"
          :loading="isSaving"
          @click="submit"
        >
          {{ t('common.create') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<style scoped>
.form-stack {
  display: grid;
  gap: 12px;
}

.field-row {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.field-row > * {
  flex: 1 1 130px;
}

.upgrade-row > .v-btn {
  flex: 0 0 auto;
  align-self: center;
}

.upgrades {
  border: 1px solid rgb(var(--v-theme-surface-variant));
  border-radius: 12px;
  padding: 12px 16px;
  display: grid;
  gap: 8px;
}

.upgrades h3 {
  margin: 0;
  font-size: 0.9rem;
  color: rgb(var(--v-theme-primary));
}

.upgrades__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.validation-list {
  margin: 0;
  padding-left: 18px;
}
</style>
