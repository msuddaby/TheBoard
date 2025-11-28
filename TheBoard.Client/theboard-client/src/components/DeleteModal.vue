<template>
  <Dialog v-slot="{ close }">
    <form>
      <DialogTrigger as-child>
        <Button variant="destructive"> Delete {{ props.deleteItem }} </Button>
      </DialogTrigger>
      <DialogContent class="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle> Delete {{ props.deleteItem }}? </DialogTitle>
          <DialogDescription>
            Are you sure you want to delete this {{ props.deleteItem }}? This action cannot be
            undone.
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <div class="mt-4 flex justify-end space-x-2">
            <DialogClose as-child>
              <Button variant="outline">Cancel</Button>
            </DialogClose>
            <Button variant="destructive" @click="onConfirm(close)">Delete</Button>
          </div>
        </DialogFooter>
      </DialogContent>
    </form>
  </Dialog>
</template>

<script lang="ts" setup>
import Button from './ui/button/Button.vue'
import Dialog from './ui/dialog/Dialog.vue'
import DialogClose from './ui/dialog/DialogClose.vue'
import DialogContent from './ui/dialog/DialogContent.vue'
import DialogDescription from './ui/dialog/DialogDescription.vue'
import DialogFooter from './ui/dialog/DialogFooter.vue'
import DialogHeader from './ui/dialog/DialogHeader.vue'
import DialogTitle from './ui/dialog/DialogTitle.vue'
import DialogTrigger from './ui/dialog/DialogTrigger.vue'

interface Props {
  deleteItem: string
  onDeleteConfirmed: () => void
}

const props = defineProps<Props>()

function onConfirm(close: () => void) {
  props.onDeleteConfirmed()
  close()
}
</script>
