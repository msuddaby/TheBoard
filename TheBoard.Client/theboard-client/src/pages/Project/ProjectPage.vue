<script setup lang="ts">
import { computed, ref, watchEffect } from 'vue'
import ProjectForm from './components/BoardItemForm.vue'
import ProjectList from './components/ProjectBoardItemList.vue'
import { BoardItemClient, ProjectClient, ProjectVM, type BoardItemVM } from '@/client/theboard-api'
import AutoBreadcrumb from '@/components/AutoBreadcrumb.vue'

interface Props {
  projectId: string
}
const props = defineProps<Props>()
const loading = ref<boolean>(false)
const projectBoardItems = ref<BoardItemVM[]>([])
const highestPriority = ref<number>(0)
const project = ref<ProjectVM>()

async function fetchProject() {
  try {
    const client = new ProjectClient('http://localhost:5282')
    const parsedId = parseInt(props.projectId, 10)
    if (isNaN(parsedId)) {
      console.error('Invalid project ID:', props.projectId)
      return
    }
    project.value = await client.getProjectById(parsedId)
  } catch (error) {
    console.error('Error fetching project:', error)
  }
}

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

const nextPriority = computed(() => {
  return projectBoardItems.value.length > 0
    ? Math.max(...projectBoardItems.value.map((item) => item.priority)) + 1
    : 0
})
function projectBoardItemsUpdated(newList: BoardItemVM[]) {
  console.log('Project board items updated:', newList)
  projectBoardItems.value = newList
}

watchEffect(() => {
  fetchProject()
  fetchBoardItems()
})
</script>

<template>
  <div class="mb-6">
    <AutoBreadcrumb />
  </div>
  <template v-if="loading">
    <p>Loading board items...</p>
  </template>
  <template v-else>
    <div class="mb-12">
      <ProjectForm
        :project-id="props.projectId"
        :next-priority="nextPriority"
        :project-title="project?.name"
        @boardItemCreated="handleBoardItemCreated"
      />
    </div>

    <div class="mb-6">
      <h2 class="mb-12">Project Board Items</h2>
      <ProjectList
        :project-list="projectBoardItems"
        @projectListUpdated="projectBoardItemsUpdated"
      />
    </div>
  </template>
</template>
