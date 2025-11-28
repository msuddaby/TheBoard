<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'
import router from '@/router'

const route = useRoute()

const breadcrumbs = computed(() => {
  const crumbs = []
  const current = route

  // Add current route
  if (current.meta.breadcrumb) {
    crumbs.unshift({
      label:
        typeof current.meta.breadcrumb === 'function'
          ? current.meta.breadcrumb(current)
          : current.meta.breadcrumb,
      to: current.path,
    })
  }

  // Walk up parent chain
  let parentName = current.meta.breadcrumbParent
  while (parentName) {
    const parentRoute = router.getRoutes().find((r) => r.name === parentName)
    if (parentRoute?.meta?.breadcrumb) {
      crumbs.unshift({
        label: parentRoute.meta.breadcrumb,
        to: parentRoute.path,
      })
      parentName = parentRoute.meta.breadcrumbParent
    } else {
      break
    }
  }

  return crumbs
})
</script>

<template>
  <Breadcrumb>
    <BreadcrumbList>
      <template v-for="(crumb, index) in breadcrumbs" :key="crumb.to">
        <BreadcrumbItem>
          <BreadcrumbLink v-if="index < breadcrumbs.length - 1" :href="crumb.to">
            {{ crumb.label }}
          </BreadcrumbLink>
          <BreadcrumbPage v-else>{{ crumb.label }}</BreadcrumbPage>
        </BreadcrumbItem>
        <BreadcrumbSeparator class="list-none" v-if="index < breadcrumbs.length - 1" />
      </template>
    </BreadcrumbList>
  </Breadcrumb>
</template>
