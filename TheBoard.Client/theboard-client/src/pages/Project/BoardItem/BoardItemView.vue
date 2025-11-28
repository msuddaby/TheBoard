<script lang="ts" setup>
import { BoardItemClient, type BoardItemVM } from '@/client/theboard-api'
import DeleteModal from '@/components/DeleteModal.vue'
import Button from '@/components/ui/button/Button.vue'
import { MdPreview } from 'md-editor-v3'
import { toast } from 'vue-sonner'

interface Props {
  boardItem: BoardItemVM
  onItemDeleted: (itemId: number) => void
  onItemMarkedDone: (itemId: number) => void
}
const props = defineProps<Props>()

async function markAsDone() {
  const client = new BoardItemClient('http://localhost:5282')
  try {
    await client.markBoardItemAsCompleted(props.boardItem.id)
    toast.success('Board item marked as done!')
    props.onItemMarkedDone(props.boardItem.id)
  } catch (error) {
    console.error('Error marking board item as done:', error)
  }
}

async function deleteBoardItem() {
  const client = new BoardItemClient('http://localhost:5282')
  try {
    const idToDelete = props.boardItem.id
    await client.deleteBoardItem(idToDelete)
    toast.success('Board item deleted successfully!')
    props.onItemDeleted(idToDelete)
  } catch (error) {
    console.error('Error deleting board item:', error)
    toast.error('Failed to delete board item.')
  }
}
</script>

<template>
  <div class="p-3">
    <h3 class="text-2xl font-bold mb-4">Description</h3>
    <MdPreview class="rounded-lg p-3" :model-value="props.boardItem.description" theme="dark" />
    <p class="text-sm text-gray-500">Priority: {{ props.boardItem.priority }}</p>
  </div>
  <div>
    <div class="flex">
      <div class="p-3">
        <Button @click="markAsDone">Mark as Done</Button>
      </div>
      <div class="p-3">
        <DeleteModal deleteItem="board item" :onDeleteConfirmed="deleteBoardItem" />
      </div>
    </div>
  </div>
</template>
