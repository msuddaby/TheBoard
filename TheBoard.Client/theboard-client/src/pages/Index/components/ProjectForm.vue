<script setup lang="ts">
import { ref } from 'vue'
import { ProjectClient, ProjectVM } from '@/client/theboard-api'
import Button from '@/components/ui/button/Button.vue'
import Input from '@/components/ui/input/Input.vue'

const model = ref<ProjectVM>(
  new ProjectVM({
    name: '',
    projectId: 0,
  }),
)

const emits = defineEmits<{
  (e: 'projectCreated'): void
}>()

async function onSubmit() {
  console.log('Creating project with data:', model.value)
  const client = new ProjectClient('http://localhost:5282')
  const projectName = model.value.name
  try {
    await client.createProject(projectName)
    console.log('Project created successfully')
    window.alert('Project created successfully')
    model.value.name = ''
    emits('projectCreated')
  } catch (error) {
    console.error('Error creating project:', error)
  }
}
</script>

<template>
  <div>
    <h2>Create New Project</h2>
    <form @submit.prevent="onSubmit">
      <div class="flex w-full max-w-sm items-center space-x-2">
        <label for="projectName" class="sr-only">Project Name</label>
        <Input
          v-model="model.name"
          placeholder="Project Name"
          type="text"
          id="projectName"
          name="projectName"
        />
        <Button type="submit">Create Project</Button>
      </div>
    </form>
  </div>
</template>
