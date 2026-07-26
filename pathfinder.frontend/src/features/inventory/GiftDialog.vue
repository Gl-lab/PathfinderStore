<script setup lang="ts">
import axios from 'axios'
import { ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { getApiErrorMessages } from '@/api/errors'
import ItemListRow from '@/components/ItemListRow.vue'
import CharacterAvatar from '@/features/characters/CharacterAvatar.vue'
import { createPartyGift, type CharacterInventoryItem } from './api'
import type { GiftRecipient } from './gift'
import { isItemVersionConflict } from './versionConflict'

const props = defineProps<{
  campaignId: number
  sourceCharacterId: number
  item: CharacterInventoryItem | null
  recipients: GiftRecipient[]
}>()
const emit = defineEmits<{
  sent: [recipient: GiftRecipient]
  versionConflict: []
}>()
const model = defineModel<boolean>({ required: true })
const { t } = useI18n()
const recipientId = ref<number | null>(null)
const giftKey = ref('')
const errors = ref<string[]>([])
const versionConflict = ref(false)
const isSending = ref(false)

watch(model, (isOpen) => {
  if (!isOpen) return
  recipientId.value = null
  giftKey.value = globalThis.crypto.randomUUID()
  errors.value = []
  versionConflict.value = false
})
watch(
  () => props.item?.version,
  () => {
    versionConflict.value = false
  },
)

async function send(): Promise<void> {
  if (!props.item || !recipientId.value || isSending.value) return

  isSending.value = true
  errors.value = []
  versionConflict.value = false
  try {
    await createPartyGift(props.campaignId, {
      giftKey: giftKey.value,
      sourceCharacterId: props.sourceCharacterId,
      destinationCharacterId: recipientId.value,
      itemInstanceKey: props.item.itemInstanceKey,
      expectedItemVersion: props.item.version,
    })
    const recipient = props.recipients.find((item) => item.characterId === recipientId.value)
    if (recipient) emit('sent', recipient)
    model.value = false
  } catch (error) {
    if (isItemVersionConflict(error)) {
      versionConflict.value = true
      emit('versionConflict')
    } else {
      errors.value = getApiErrorMessages(error)
      if (axios.isAxiosError(error) && error.response) {
        giftKey.value = globalThis.crypto.randomUUID()
      }
    }
  } finally {
    isSending.value = false
  }
}
</script>

<template>
  <v-dialog v-model="model" max-width="500" persistent>
    <v-card>
      <v-card-title>{{ t('tradeUi.gift.title') }}</v-card-title>
      <v-card-text class="gift-dialog">
        <ItemListRow
          v-if="item"
          :bulk-tenths="item.revision.bulkTenths * item.quantity"
          :category="item.revision.primaryCategory"
          :name="item.revision.name"
          :quantity="item.quantity"
        />
        <v-alert v-if="versionConflict" type="warning" variant="tonal">
          {{ t('tradeUi.gift.versionConflict') }}
        </v-alert>
        <v-alert v-for="message in errors" :key="message" type="error" variant="tonal">
          {{ message }}
        </v-alert>
        <v-select
          v-if="recipients.length"
          v-model="recipientId"
          item-title="name"
          item-value="characterId"
          :items="recipients"
          :label="t('tradeUi.gift.recipient')"
        >
          <template #item="{ props: itemProps, item: recipient }">
            <v-list-item v-bind="itemProps">
              <template #prepend>
                <CharacterAvatar
                  :alt="recipient.raw.name"
                  :path="recipient.raw.avatarPath"
                  :size="32"
                />
              </template>
              <v-list-item-subtitle>
                {{
                  t('tradeUi.gift.controlledBy', {
                    userId: recipient.raw.controlledByUserId,
                  })
                }}
              </v-list-item-subtitle>
            </v-list-item>
          </template>
        </v-select>
        <v-alert v-else type="info" variant="tonal">
          {{ t('tradeUi.gift.noRecipients') }}
        </v-alert>
        <v-alert type="info" variant="tonal">
          {{ t('tradeUi.gift.confirmationHint') }}
        </v-alert>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn :disabled="isSending" variant="text" @click="model = false">
          {{ t('common.cancel') }}
        </v-btn>
        <v-btn
          color="primary"
          :disabled="!recipientId || isSending || versionConflict"
          :loading="isSending"
          @click="send"
        >
          {{ t('tradeUi.gift.send') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<style scoped>
.gift-dialog {
  display: grid;
  gap: 16px;
}
</style>
