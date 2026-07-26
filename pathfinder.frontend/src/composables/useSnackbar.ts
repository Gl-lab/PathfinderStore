import { reactive } from 'vue'

export type SnackbarKind = 'success' | 'error' | 'info'

interface SnackbarMessage {
  id: number
  text: string
  kind: SnackbarKind
}

const state = reactive({
  queue: [] as SnackbarMessage[],
  nextId: 1,
})

function enqueue(text: string, kind: SnackbarKind): void {
  state.queue.push({ id: state.nextId++, text, kind })
}

export function useSnackbar() {
  return {
    state,
    success: (text: string) => enqueue(text, 'success'),
    error: (text: string) => enqueue(text, 'error'),
    info: (text: string) => enqueue(text, 'info'),
    dismiss: () => state.queue.shift(),
  }
}
