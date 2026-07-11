import axios from 'axios'

export const http = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    Accept: 'application/json',
    'Content-Type': 'application/json',
  },
})

http.interceptors.request.use((config) => {
  const token = localStorage.getItem('pathfinder.auth-token')

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})
