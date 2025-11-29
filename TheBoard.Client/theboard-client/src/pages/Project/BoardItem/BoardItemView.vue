<script lang="ts" setup>
import { type BoardItemVM } from '@/client/theboard-api'
import DeleteModal from '@/components/DeleteModal.vue'
import Button from '@/components/ui/button/Button.vue'
import { MdPreview } from 'md-editor-v3'
import { toast } from 'vue-sonner'
import BoardItemForm from '../components/BoardItemForm.vue'
import { ref } from 'vue'
import { createBoardItemClient } from '@/client/api-client'

interface Props {
  boardItem: BoardItemVM
  onItemDeleted: (itemId: number) => void
  onItemMarkedDone: (itemId: number) => void
  onItemEdited: (item: BoardItemVM) => void
}
const props = defineProps<Props>()

const boardItemFormRef = ref<InstanceType<typeof BoardItemForm>>()

const client = createBoardItemClient()

async function markAsDone() {
  try {
    await client.markBoardItemAsCompleted(props.boardItem.id)
    toast.success('Board item marked as done!')
    props.onItemMarkedDone(props.boardItem.id)
  } catch (error) {
    console.error('Error marking board item as done:', error)
  }
}

async function deleteBoardItem() {
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

function itemEdited(item: BoardItemVM) {
  props.onItemEdited(item)
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
        <Button variant="default" @click="boardItemFormRef?.openDialog()">Edit</Button>
      </div>
      <div class="p-3">
        <Button variant="secondary" @click="markAsDone">Mark as Done</Button>
      </div>
      <div class="p-3">
        <DeleteModal deleteItem="board item" :onDeleteConfirmed="deleteBoardItem" />
      </div>
    </div>
  </div>

  <BoardItemForm
    ref="boardItemFormRef"
    :projectId="props.boardItem.projectId.toString()"
    :projectTitle="props.boardItem.title"
    :nextPriority="props.boardItem.priority + 1"
    :currentData="props.boardItem"
    :hideButton="true"
    @boardItemCreated="() => {}"
    @boardItemUpdated="itemEdited"
  />
</template>
