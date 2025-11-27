<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import ProjectForm from './components/BoardItemForm.vue'
import ProjectList from './components/ProjectBoardItemList.vue'
import { BoardItemClient, type BoardItemVM } from '@/client/theboard-api'

interface Props {
  projectId: string
}
const props = defineProps<Props>()
const loading = ref<boolean>(false)
const projectBoardItems = ref<BoardItemVM[]>([])
const highestPriority = ref<number>(0)

async function fetchBoardItems() {
  loading.value = true
  try {
    const parsedId = parseInt(props.projectId, 10)
    if (isNaN(parsedId)) {
      console.error('Invalid project ID:', props.projectId)
      loading.value = false
      return
    }
    const client = new BoardItemClient('http://localhost:5282')
    const items = await client.getBoardItemsByProjectId(parsedId)
    const sortedItems = items.sort((a, b) => a.priority - b.priority)
    projectBoardItems.value = sortedItems
    highestPriority.value =
      projectBoardItems.value.length > 0
        ? Math.max(...projectBoardItems.value.map((item) => item.priority))
        : 0
  } catch (error) {
    console.error('Error fetching board items:', error)
  } finally {
    loading.value = false
  }
}

function handleBoardItemCreated() {
  fetchBoardItems()
}

function getNextPriority(): number {
  return projectBoardItems.value.length > 0
    ? Math.max(...projectBoardItems.value.map((item) => item.priority)) + 1
    : 0
}

watchEffect(() => {
  fetchBoardItems()
})
</script>

<template>
  <div></div>
  <template v-if="loading">
    <p>Loading board items...</p>
  </template>
  <template v-else>
    <div class="mb-6">
      <ProjectForm
        :project-id="props.projectId"
        :get-next-priority="getNextPriority"
        @boardItemCreated="handleBoardItemCreated"
      />
    </div>

    <div class="mb-6">
      <h2>Project Board Items</h2>
      <ProjectList :project-list="projectBoardItems" />
    </div>
  </template>
</template>
