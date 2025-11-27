<script lang="ts" setup>
import { ProjectClient } from '@/client/theboard-api'
import { onMounted } from 'vue'
import ProjectsList from './components/ProjectsList.vue'

onMounted(async () => {
  const client = new ProjectClient('http://localhost:5282')

  const result = await client.getAllProjects()
  console.log('Projects:', result)
})
</script>
<template>
  <h1>Welcome to TheBoard</h1>
  <p>Your central hub for managing projects and tasks efficiently.</p>

  <ProjectsList />
</template>
