<script setup lang="ts">
import { ProjectVM } from '@/client/theboard-api'
import { ref, watchEffect } from 'vue'
import ProjectForm from './ProjectForm.vue'
import Card from '@/components/ui/card/Card.vue'
import CardHeader from '@/components/ui/card/CardHeader.vue'
import CardTitle from '@/components/ui/card/CardTitle.vue'
import router from '@/router'
import { createProjectClient } from '@/client/api-client'

const projects = ref<ProjectVM[]>([])

const projectClient = createProjectClient()

watchEffect(async () => {
  try {
    await refreshProjects()
  } catch (error) {
    console.error('Error fetching projects:', error)
  }
})

async function refreshProjects() {
  try {
    const response = await projectClient.getAllProjects()
    projects.value = response
  } catch (error) {
    console.error('Error fetching projects:', error)
  }
}

function generateGradientBackground(index: number): string {
  const colors = [
    'from-purple-500/70 to-pink-500/70',
    'from-green-500/70 to-blue-500/70',
    'from-yellow-500/70 to-red-500/70',
    'from-indigo-500/70 to-purple-500/70',
    'from-pink-500/70 to-red-500/70',
    'from-teal-500/70 to-green-500/70',
  ]
  return `bg-gradient-to-r ${colors[index % colors.length]}`
}

function handleProjectClick(projectId: number) {
  router.push({ name: 'project', params: { projectId: projectId.toString() } })
}
</script>

<template>
  <div>
    <section class="mb-5">
      <ProjectForm @projectCreated="refreshProjects" />
    </section>

    <section class="mb-5">
      <h2>Projects List</h2>
      <template v-if="projects.length === 0">
        <p>No projects available.</p>
      </template>
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 mt-4">
        <Card
          v-for="(project, index) in projects"
          :key="project.projectId"
          @click="handleProjectClick(project.projectId)"
          :class="`${generateGradientBackground(index)} border-0  duration-300 hover:scale-105 hover:cursor-pointer transform-gpu text-white`"
        >
          <CardHeader>
            <CardTitle>{{ project.name }}</CardTitle>
          </CardHeader>
        </Card>
      </div>
    </section>
  </div>
</template>
