<script setup lang="ts">
import { BoardItemClient, BoardItemCreateVM } from '@/client/theboard-api'
import Button from '@/components/ui/button/Button.vue'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import Textarea from '@/components/ui/textarea/Textarea.vue'
import { ref } from 'vue'

interface Props {
  projectId: string
  getNextPriority: () => number
}

const props = defineProps<Props>()
const boardItemModel = ref<BoardItemCreateVM>(
  new BoardItemCreateVM({
    title: '',
    description: '',
    priority: props.getNextPriority(),
    projectId: parseInt(props.projectId, 10),
  }),
)

const emits = defineEmits<{
  (e: 'boardItemCreated'): void
}>()

async function createBoardItem() {
  const client = new BoardItemClient('http://localhost:5282')
  try {
    await client.createBoardItem(boardItemModel.value)
    window.alert('Board item created successfully!')
    boardItemModel.value.title = ''
    boardItemModel.value.description = ''
    boardItemModel.value.priority = props.getNextPriority()
    boardItemModel.value.projectId = parseInt(props.projectId, 10)
    emits('boardItemCreated')
  } catch (error) {
    console.error('Error creating board item:', error)
  }
}
</script>

<template>
  <div>
    <h2>Create New Board Item for Project ID: {{ props.projectId }}</h2>
    <form @submit.prevent="createBoardItem">
      <div class="grid w-full max-w-sm items-center gap-1.5 mb-3">
        <Label for="title">Title</Label>
        <Input id="title" v-model="boardItemModel.title" type="text" />
      </div>
      <div class="grid w-full max-w-sm items-center gap-1.5 mb-3">
        <Label for="description">Description</Label>
        <Textarea id="description" v-model="boardItemModel.description" rows="4"></Textarea>
      </div>
      <div class="grid w-full max-w-sm items-center gap-1.5 mb-3">
        <Label for="priority">Priority</Label>
        <Input id="priority" v-model="boardItemModel.priority" type="number" />
      </div>
      <Button type="submit">Create Board Item</Button>
    </form>
  </div>
</template>
