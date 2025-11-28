<script lang="ts" setup>
import type { BoardItemVM } from '@/client/theboard-api'
import { MdPreview } from 'md-editor-v3'

interface Props {
  boardItem: BoardItemVM
}
const props = defineProps<Props>()
</script>

<template>
  <div class="p-3">
    <h3 class="text-2xl font-bold mb-4">Description</h3>
    <MdPreview class="rounded-lg p-3" :model-value="props.boardItem.description" theme="dark" />
    <p class="text-sm text-gray-500">Priority: {{ props.boardItem.priority }}</p>
  </div>
</template>
