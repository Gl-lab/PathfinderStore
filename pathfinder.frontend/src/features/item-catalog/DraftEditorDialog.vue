<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import MoneyInput from '@/components/MoneyInput.vue'
import type { AdminItemDefinition } from './api'
import {
  createDraftFormModel,
  createAttack,
  draftValidationErrors,
  prefillDraftFromDefinition,
  visibleSections,
  type DraftFormModel,
} from './draftForm'
import { enumOptions } from './options'

const props = defineProps<{
  catalogMode: 'campaign' | 'global'
  campaignName: string | null
  definition: AdminItemDefinition | null
  isSaving: boolean
  errors: string[]
}>()
const open = defineModel<boolean>({ required: true })
const emit = defineEmits<{ submit: [model: DraftFormModel] }>()
const { t } = useI18n()

const model = ref<DraftFormModel>(createDraftFormModel())
const isRevisionMode = computed(() => props.definition !== null)
const sections = computed(() => visibleSections(model.value.category))
const validationKeys = computed(() => draftValidationErrors(model.value, !isRevisionMode.value))
const canSubmit = computed(() => validationKeys.value.length === 0 && !props.isSaving)

watch(open, (isOpen) => {
  if (!isOpen) return
  model.value = props.definition
    ? prefillDraftFromDefinition(props.definition)
    : createDraftFormModel()
})

const categoryOptions = computed(() =>
  enumOptions(
    [
      'Weapon',
      'Armor',
      'Shield',
      'Consumable',
      'Ammunition',
      'Rune',
      'Tool',
      'Container',
      'OtherEquipment',
    ] as const,
    (value) => t(`inventoryUi.categories.${value}`),
  ),
)
const rarityOptions = computed(() =>
  enumOptions(['Common', 'Uncommon', 'Rare', 'Unique'] as const, (value) =>
    t(`itemCatalogUi.rarities.${value}`),
  ),
)
const dieSizeOptions = computed(() =>
  enumOptions(['D4', 'D6', 'D8', 'D10', 'D12'] as const, (value) => value.toLowerCase()),
)
const damageTypeOptions = computed(() =>
  enumOptions(
    [
      'Bludgeoning',
      'Piercing',
      'Slashing',
      'Acid',
      'Cold',
      'Electricity',
      'Fire',
      'Force',
      'Mental',
      'Poison',
      'Sonic',
      'Spirit',
      'Vitality',
      'Void',
    ] as const,
    (value) => t(`itemCatalogUi.damageTypes.${value}`),
  ),
)
const armorCategoryOptions = computed(() =>
  enumOptions(['Unarmored', 'Light', 'Medium', 'Heavy'] as const, (value) =>
    t(`itemCatalogUi.armorCategories.${value}`),
  ),
)
const usageOptions = computed(() =>
  enumOptions(['Held', 'Worn', 'Installed', 'Stored'] as const, (value) =>
    t(`itemCatalogUi.usages.${value}`),
  ),
)
const consumptionOptions = computed(() =>
  enumOptions(['DestroyInstance', 'ReduceStack', 'ConsumeAmmunition'] as const, (value) =>
    t(`itemCatalogUi.consumptionModes.${value}`),
  ),
)
const recoveryOptions = computed(() =>
  enumOptions(['None', 'DailyPreparations', 'Manual'] as const, (value) =>
    t(`itemCatalogUi.chargeRecovery.${value}`),
  ),
)

function addAttack(): void {
  model.value.attacks.push(createAttack())
}

function removeAttack(index: number): void {
  model.value.attacks.splice(index, 1)
}

function submit(): void {
  if (!canSubmit.value) return
  emit('submit', model.value)
}
</script>

<template>
  <v-dialog v-model="open" max-width="760" persistent>
    <v-card>
      <v-card-title>
        {{
          isRevisionMode
            ? t('itemCatalogUi.draft.newRevisionTitle', { key: definition?.key })
            : t('itemCatalogUi.draft.createTitle')
        }}
      </v-card-title>
      <v-card-subtitle>
        {{
          catalogMode === 'campaign'
            ? t('itemCatalogUi.draft.scopeCampaign', { name: campaignName ?? '' })
            : t('itemCatalogUi.draft.scopeGlobal')
        }}
      </v-card-subtitle>
      <v-card-text class="form-stack">
        <v-alert v-for="message in errors" :key="message" type="error" variant="tonal">
          {{ message }}
        </v-alert>

        <v-text-field
          v-if="!isRevisionMode"
          v-model="model.key"
          :hint="t('itemCatalogUi.draft.keyHint')"
          :label="t('itemCatalogUi.draft.key')"
          persistent-hint
        />
        <v-text-field
          v-else
          :aria-describedby="'draft-key-locked-hint'"
          disabled
          :label="t('itemCatalogUi.draft.key')"
          :model-value="definition?.key"
        />
        <span v-if="isRevisionMode" id="draft-key-locked-hint" class="field-hint">
          {{ t('itemCatalogUi.draft.keyLockedHint') }}
        </span>
        <v-alert v-if="isRevisionMode" type="info" variant="tonal">
          {{ t('itemCatalogUi.draft.rulesNotCarried') }}
        </v-alert>

        <v-text-field v-model="model.name" :label="t('itemCatalogUi.draft.name')" />
        <v-textarea
          v-model="model.description"
          auto-grow
          :label="t('itemCatalogUi.draft.description')"
          rows="2"
        />
        <div class="field-row">
          <v-number-input v-model="model.level" :label="t('itemCatalogUi.draft.level')" :min="0" />
          <v-number-input
            v-model="model.bulk"
            :label="t('itemCatalogUi.draft.bulk')"
            :min="0"
            :precision="2"
            :step="0.1"
          />
        </div>
        <p class="field-hint">{{ t('itemCatalogUi.draft.price') }}</p>
        <MoneyInput v-model="model.priceInCopperPieces" />
        <div class="field-row">
          <v-select
            v-model="model.category"
            item-title="title"
            item-value="value"
            :items="categoryOptions"
            :label="t('itemCatalogUi.draft.category')"
          />
          <v-select
            v-model="model.rarity"
            item-title="title"
            item-value="value"
            :items="rarityOptions"
            :label="t('itemCatalogUi.draft.rarity')"
          />
        </div>

        <section v-if="sections.includes('attacks')" class="rules-section">
          <header class="rules-section__header">
            <h3>{{ t('itemCatalogUi.draft.attacks') }}</h3>
            <v-btn size="small" variant="text" @click="addAttack">
              {{ t('itemCatalogUi.draft.addAttack') }}
            </v-btn>
          </header>
          <div v-for="(attack, index) in model.attacks" :key="index" class="field-row attack-row">
            <v-text-field v-model="attack.name" :label="t('itemCatalogUi.draft.attackName')" />
            <v-number-input
              v-model="attack.damageDieCount"
              :label="t('itemCatalogUi.draft.dieCount')"
              :min="1"
            />
            <v-select
              v-model="attack.damageDieSize"
              item-title="title"
              item-value="value"
              :items="dieSizeOptions"
              :label="t('itemCatalogUi.draft.dieSize')"
            />
            <v-select
              v-model="attack.damageType"
              item-title="title"
              item-value="value"
              :items="damageTypeOptions"
              :label="t('itemCatalogUi.draft.damageType')"
            />
            <v-number-input
              v-model="attack.hands"
              :label="t('itemCatalogUi.draft.hands')"
              :min="1"
            />
            <v-btn
              :aria-label="t('common.delete')"
              :disabled="model.attacks.length <= 1 && model.category !== 'Ammunition'"
              icon="mdi-close"
              size="small"
              variant="text"
              @click="removeAttack(index)"
            />
          </div>
        </section>

        <section v-if="sections.includes('armor')" class="rules-section">
          <h3>{{ t('itemCatalogUi.draft.armor') }}</h3>
          <div class="field-row">
            <v-select
              v-model="model.armor.category"
              item-title="title"
              item-value="value"
              :items="armorCategoryOptions"
              :label="t('itemCatalogUi.draft.armorCategory')"
            />
            <v-number-input
              v-model="model.armor.armorClassBonus"
              :label="t('itemCatalogUi.draft.armorClassBonus')"
            />
            <v-number-input
              v-model="model.armor.dexterityCap"
              :label="t('itemCatalogUi.draft.dexterityCap')"
            />
          </div>
          <div class="field-row">
            <v-number-input
              v-model="model.armor.checkPenalty"
              :label="t('itemCatalogUi.draft.checkPenalty')"
            />
            <v-number-input
              v-model="model.armor.speedPenaltyFeet"
              :label="t('itemCatalogUi.draft.speedPenalty')"
            />
            <v-number-input
              v-model="model.armor.strengthRequirement"
              :label="t('itemCatalogUi.draft.strengthRequirement')"
            />
          </div>
        </section>

        <section v-if="sections.includes('shield')" class="rules-section">
          <h3>{{ t('itemCatalogUi.draft.shield') }}</h3>
          <v-number-input
            v-model="model.shield.raisedArmorClassBonus"
            :label="t('itemCatalogUi.draft.raisedArmorClassBonus')"
            :min="0"
          />
        </section>

        <section v-if="sections.includes('equipment')" class="rules-section">
          <h3>{{ t('itemCatalogUi.draft.equipment') }}</h3>
          <div class="field-row">
            <v-select
              v-model="model.equipment.usage"
              item-title="title"
              item-value="value"
              :items="usageOptions"
              :label="t('itemCatalogUi.draft.usage')"
            />
            <v-number-input
              v-model="model.equipment.requiredHands"
              :label="t('itemCatalogUi.draft.requiredHands')"
              :min="0"
            />
          </div>
        </section>

        <section v-if="sections.includes('consumption')" class="rules-section">
          <h3>{{ t('itemCatalogUi.draft.consumption') }}</h3>
          <div class="field-row">
            <v-select
              v-model="model.consumption.mode"
              item-title="title"
              item-value="value"
              :items="consumptionOptions"
              :label="t('itemCatalogUi.draft.consumptionMode')"
            />
            <v-number-input
              v-model="model.consumption.quantity"
              :label="t('itemCatalogUi.draft.consumptionQuantity')"
              :min="1"
            />
          </div>
        </section>

        <v-expansion-panels>
          <v-expansion-panel :title="t('itemCatalogUi.draft.advanced')">
            <template #text>
              <v-checkbox
                v-model="model.chargesEnabled"
                hide-details
                :label="t('itemCatalogUi.draft.chargesEnabled')"
              />
              <div v-if="model.chargesEnabled" class="field-row">
                <v-number-input
                  v-model="model.charges.maximumCharges"
                  :label="t('itemCatalogUi.draft.maximumCharges')"
                  :min="1"
                />
                <v-number-input
                  v-model="model.charges.defaultActivationCost"
                  :label="t('itemCatalogUi.draft.activationCost')"
                  :min="0"
                />
                <v-select
                  v-model="model.charges.recoveryRule"
                  item-title="title"
                  item-value="value"
                  :items="recoveryOptions"
                  :label="t('itemCatalogUi.draft.recoveryRule')"
                />
              </div>
              <v-checkbox
                v-model="model.durabilityEnabled"
                hide-details
                :label="t('itemCatalogUi.draft.durabilityEnabled')"
              />
              <div v-if="model.durabilityEnabled" class="field-row">
                <v-number-input
                  v-model="model.durability.hardness"
                  :label="t('itemCatalogUi.draft.hardness')"
                  :min="0"
                />
                <v-number-input
                  v-model="model.durability.maximumHitPoints"
                  :label="t('itemCatalogUi.draft.maximumHitPoints')"
                  :min="1"
                />
                <v-number-input
                  v-model="model.durability.brokenThreshold"
                  :label="t('itemCatalogUi.draft.brokenThreshold')"
                  :min="0"
                />
              </div>
            </template>
          </v-expansion-panel>
        </v-expansion-panels>

        <v-alert v-if="validationKeys.length" type="warning" variant="tonal">
          <ul class="validation-list">
            <li v-for="key in validationKeys" :key="key">{{ t(key) }}</li>
          </ul>
        </v-alert>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn :disabled="isSaving" variant="text" @click="open = false">
          {{ t('common.cancel') }}
        </v-btn>
        <v-btn color="primary" :disabled="!canSubmit" :loading="isSaving" @click="submit">
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
  flex: 1 1 140px;
}

.attack-row > .v-btn {
  flex: 0 0 auto;
  align-self: center;
}

.field-hint {
  margin: 0;
  color: rgb(var(--v-theme-on-surface-variant));
  font-size: 0.75rem;
}

.rules-section {
  border: 1px solid rgb(var(--v-theme-surface-variant));
  border-radius: 12px;
  padding: 12px 16px;
  display: grid;
  gap: 8px;
}

.rules-section h3 {
  margin: 0;
  font-size: 0.9rem;
  color: rgb(var(--v-theme-primary));
}

.rules-section__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.validation-list {
  margin: 0;
  padding-left: 18px;
}
</style>
