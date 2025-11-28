<script lang="ts" setup>
import { BoardItemClient, BoardItemPriorityUpdateVM, type BoardItemVM } from '@/client/theboard-api'
import BalatroFlame from '@/components/BalatroFlame.vue'
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet'
import { ref } from 'vue'
import { VueDraggable } from 'vue-draggable-plus'
import { hslToRgb } from '@/misc/utilities'
import BoardItemView from '../BoardItem/BoardItemView.vue'

interface Props {
  projectList: BoardItemVM[]
}

const props = defineProps<Props>()

const emits = defineEmits<{
  (e: 'projectListUpdated', value: BoardItemVM[]): void
}>()

const projects = ref<BoardItemVM[]>(props.projectList)
const selectedItem = ref<BoardItemVM | null>(null)
const sheetOpen = ref(false)
const removingItemId = ref<number | null>(null)

function openItem(item: BoardItemVM) {
  selectedItem.value = item
  sheetOpen.value = true
}

async function onDragEnd() {
  const updates = props.projectList.map((item, index) => {
    const update = new BoardItemPriorityUpdateVM()
    update.boardItemId = item.id
    update.newPriority = index
    return update
  })
  try {
    const client = new BoardItemClient('http://localhost:5282')
    await client.updateBoardItemPriority(updates)
  } catch (error) {
    console.error('Error updating board item priorities:', error)
  }
}

function getItemHue(index: number, totalItems: number): number {
  const t = totalItems > 1 ? index / (totalItems - 1) : 0
  return t * 240
}

function getItemStyle(index: number, totalItems: number): string {
  const hue = getItemHue(index, totalItems)
  return `background: hsl(${hue}, 40%, 30%)`
}

function getFlameColors(index: number, totalItems: number) {
  const baseHue = getItemHue(index, totalItems)
  const cardColor = hslToRgb(baseHue, 40, 30)
  return {
    top: cardColor,
    middle: hslToRgb(1, 60, 30),
    bottom: hslToRgb(baseHue, 70, 60),
  }
}

function onItemDeleted(closeDialog: () => void, boardItemId: number) {
  closeDialog()
  animateRemoval(boardItemId)
}

function onItemMarkedDone(closeDialog: () => void, boardItemId: number) {
  closeDialog()
  animateRemoval(boardItemId)
}

function animateRemoval(boardItemId: number) {
  removingItemId.value = boardItemId
  setTimeout(() => {
    projects.value = projects.value.filter((item) => item.id !== boardItemId)
    removingItemId.value = null
    emits('projectListUpdated', projects.value)
  }, 500) // Match the duration of the CSS animation!
}
</script>

<template>
  <template v-if="projects.length === 0">
    <p>No board items available for this project.</p>
  </template>
  <template v-else>
    <div class="w-full">
      <VueDraggable v-model="projects" ref="draggableElement" @end="onDragEnd">
        <div
          v-for="(item, index) in projects"
          :key="item.id"
          :style="getItemStyle(index, projects.length)"
          :class="[
            'relative p-4 mb-4 rounded-lg text-white cursor-move shadow-md transition-all duration-500',
            removingItemId === item.id ? 'item-removing' : '',
          ]"
          @click="openItem(item)"
        >
          <div v-if="index === 0" class="absolute -top-8 left-0 right-0 h-10">
            <BalatroFlame
              :bottom-color="getFlameColors(index, projects.length).bottom"
              :middle-color="getFlameColors(index, projects.length).middle"
              :top-color="getFlameColors(index, projects.length).top"
              :fire-speed="[0.0, 0.8]"
              :fire-aperture="0.22"
            />
          </div>
          <span class="relative z-10">{{ item.title }}</span>
        </div>
      </VueDraggable>
    </div>
    <Sheet v-model:open="sheetOpen" v-slot="{ close: closeDialog }">
      <SheetContent side="right" class="min-w-[400px] sm:min-w-[1000px]">
        <SheetHeader>
          <SheetTitle>{{ selectedItem?.title }}</SheetTitle>
        </SheetHeader>
        <div class="mt-6">
          <BoardItemView
            v-if="selectedItem"
            :board-item="selectedItem"
            :onItemDeleted="(boardItemId) => onItemDeleted(closeDialog, boardItemId)"
            :onItemMarkedDone="(boardItemId) => onItemMarkedDone(closeDialog, boardItemId)"
          />
        </div>
      </SheetContent>
    </Sheet>
  </template>
</template>

<style scoped>
.item-removing {
  animation: slideOutAndFade 0.5s ease-out forwards;
}

@keyframes slideOutAndFade {
  0% {
    opacity: 1;
    transform: translateX(0) scale(1);
    max-height: 200px;
    margin-bottom: 1rem;
  }
  50% {
    opacity: 0.5;
    transform: translateX(100px) scale(0.95);
  }
  100% {
    opacity: 0;
    transform: translateX(200px) scale(0.8);
    max-height: 0;
    margin-bottom: 0;
    padding-top: 0;
    padding-bottom: 0;
  }
}
</style>
