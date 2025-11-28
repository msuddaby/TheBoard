<script setup lang="ts">
import { Toaster } from 'vue-sonner'
import 'vue-sonner/style.css'
</script>

<template>
  <Toaster position="top-center" />
  <div class="m-0 max-w-[1200px] w-full px-4 mx-auto">
    <h1 class="text-4xl font-bold my-6">THE Board</h1>
    <p class="mb-12 text-center text-xl">Keeping you organized through endless feature requests</p>
    <RouterView />
  </div>
</template>

<style scoped></style>
