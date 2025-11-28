import { createRouter, createWebHistory, type RouteLocationNormalized } from 'vue-router'

import IndexPage from '../pages/Index/IndexPage.vue'
import ProjectPage from '../pages/Project/ProjectPage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: IndexPage,
      meta: { title: 'TheBoard - Home', breadcrumb: 'Home' },
    },
    {
      path: '/projects/:projectId',
      name: 'project',
      component: ProjectPage,
      props: true,
      meta: {
        title: 'TheBoard - Project Details',
        breadcrumb: (route: RouteLocationNormalized) => 'Project ' + route.params.projectId,
        // Add parent reference for breadcrumb building
        breadcrumbParent: 'home',
      },
    },
  ],
})

router.afterEach((to) => {
  const defaultTitle = 'TheBoard'
  document.title = to.meta.title ? String(to.meta.title) : defaultTitle
})

export default router
