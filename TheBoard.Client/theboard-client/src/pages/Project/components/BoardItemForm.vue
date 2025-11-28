<script setup lang="ts">
import { BoardItemClient, BoardItemCreateVM } from '@/client/theboard-api'
import Button from '@/components/ui/button/Button.vue'
import Dialog from '@/components/ui/dialog/Dialog.vue'
import DialogContent from '@/components/ui/dialog/DialogContent.vue'
import DialogDescription from '@/components/ui/dialog/DialogDescription.vue'
import DialogHeader from '@/components/ui/dialog/DialogHeader.vue'
import DialogTitle from '@/components/ui/dialog/DialogTitle.vue'
import DialogTrigger from '@/components/ui/dialog/DialogTrigger.vue'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import { MdEditor } from 'md-editor-v3'
import 'md-editor-v3/lib/style.css'
import { ref } from 'vue'

interface Props {
  projectId: string
  projectTitle?: string
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
    <h2 class="mb-6">Create New Board Item for {{ props.projectTitle }}</h2>
    <Dialog>
      <DialogTrigger as-child>
        <Button class="cursor-pointer" size="lg">Create Board Item</Button>
      </DialogTrigger>
      <DialogContent class="sm:max-w-[425px] lg:max-w-[1200px]">
        <DialogHeader>
          <DialogTitle> Create New Board Item </DialogTitle>
          <DialogDescription>
            Create a new board item for {{ props.projectTitle }}
          </DialogDescription>
        </DialogHeader>
        <form @submit.prevent="createBoardItem">
          <div class="grid w-full items-center gap-1.5 mb-3">
            <Label for="title">Title</Label>
            <Input id="title" v-model="boardItemModel.title" type="text" />
          </div>
          <div class="grid w-full items-center gap-1.5 mb-3">
            <Label for="description">Description</Label>
            <MdEditor
              language="english"
              theme="dark"
              v-model="boardItemModel.description"
              height="200px"
              :preview="true"
            />
          </div>
          <div class="grid w-full max-w-sm items-center gap-1.5 mb-3">
            <Label for="priority">Priority</Label>
            <Input id="priority" v-model="boardItemModel.priority" type="number" />
          </div>
          <Button type="submit">Create Board Item</Button>
        </form>
      </DialogContent>
    </Dialog>
  </div>
</template>
