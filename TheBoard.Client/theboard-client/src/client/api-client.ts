import { BoardItemClient } from './theboard-api'
import { ProjectClient } from './theboard-api'

const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5282'

export function createBoardItemClient(): BoardItemClient {
  return new BoardItemClient(baseUrl)
}

export function createProjectClient(): ProjectClient {
  return new ProjectClient(baseUrl)
}
