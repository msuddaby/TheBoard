<script setup lang="ts">
import { createBoardItemClient } from '@/client/api-client'
import { BoardItemCreateVM, BoardItemVM } from '@/client/theboard-api'
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
import { ref, watch } from 'vue'
import { toast } from 'vue-sonner'

interface Props {
  projectId: string
  projectTitle?: string
  nextPriority: number
  currentData?: BoardItemCreateVM
  hideButton?: boolean
}

const props = defineProps<Props>()

const open = ref(false)

const boardItemModel = ref<BoardItemCreateVM>(
  props.currentData ??
    new BoardItemCreateVM({
      id: 0,
      title: '',
      description: '',
      priority: props.nextPriority,
      projectId: parseInt(props.projectId, 10),
    }),
)

watch(
  () => props.nextPriority,
  (newPriority) => {
    console.log('Updating priority to:', newPriority)
    boardItemModel.value.priority = newPriority
  },
)

const emits = defineEmits<{
  (e: 'boardItemCreated'): void
  (e: 'boardItemUpdated', item: BoardItemVM): void
}>()

const boardItemClient = createBoardItemClient()

async function createBoardItem() {
  try {
    if (props.currentData != null) {
      const result = await boardItemClient.updateBoardItem(
        props.currentData.id,
        boardItemModel.value,
      )
      toast.success('Board item updated successfully!')
      emits('boardItemUpdated', result)
    } else {
      await boardItemClient.createBoardItem(boardItemModel.value)
      toast.success('Board item created successfully!')
      emits('boardItemCreated')
    }
    boardItemModel.value.title = ''
    boardItemModel.value.description = ''
    boardItemModel.value.priority = props.nextPriority
    boardItemModel.value.projectId = parseInt(props.projectId, 10)
  } catch (error) {
    console.error('Error creating board item:', error)
  }
}

defineExpose({
  openDialog: () => {
    open.value = true
  },
})
</script>

<template>
  <div>
    <h2 v-if="!hideButton" class="mb-6">Create New Board Item for {{ props.projectTitle }}</h2>
    <Dialog v-model:open="open">
      <DialogTrigger v-if="!hideButton" as-child>
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
          <Button type="submit">{{ props.currentData ? 'Update' : 'Create' }} Board Item</Button>
        </form>
      </DialogContent>
    </Dialog>
  </div>
</template>
