import { createRouter, createWebHistory } from 'vue-router'

import IndexPage from '../pages/Index/IndexPage.vue'
import ProjectPage from '../pages/Project/ProjectPage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: IndexPage,
    },
    {
      path: '/projects/:projectId',
      name: 'project',
      component: ProjectPage,
      props: true,
    },
  ],
})

export default router
