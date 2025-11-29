<script lang="ts" setup>
import { onMounted } from 'vue'
import ProjectsList from './components/ProjectsList.vue'
import { createProjectClient } from '@/client/api-client'

const projectClient = createProjectClient()

onMounted(async () => {
  const result = await projectClient.getAllProjects()
  console.log('Projects:', result)
})
</script>
<template>
  <ProjectsList />
</template>
