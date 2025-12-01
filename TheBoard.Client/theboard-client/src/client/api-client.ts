import { BoardItemClient } from './theboard-api'
import { ProjectClient } from './theboard-api'

const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5282'

console.log('API Base URL:', baseUrl)
console.log('Mode:', import.meta.env.MODE)
console.log('Prod?:', import.meta.env.PROD)
console.log('ENV baseurl:', import.meta.env.VITE_API_BASE_URL)

export function createBoardItemClient(): BoardItemClient {
  return new BoardItemClient(baseUrl)
}

export function createProjectClient(): ProjectClient {
  return new ProjectClient(baseUrl)
}
