import axios from "axios"

export const apiClient = axios.create({
  baseURL: "/api",
  headers: { "Content-Type": "application/json" },
  withCredentials: true,
})

let isRefreshing = false
let pendingQueue: Array<() => void> = []

function isOnPublicAuthPage() {
  if (typeof window === "undefined") return false
  return (
    window.location.pathname.startsWith("/auth/sign-in") ||
    window.location.pathname.startsWith("/auth/sign-up")
  )
}

function redirectToSignIn(message?: string) {
  if (typeof window === "undefined") return
  if (isOnPublicAuthPage()) return

  const url = message
    ? `/auth/sign-in?authError=${encodeURIComponent(message)}`
    : "/auth/sign-in"
  window.location.href = url
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config
    const serverMessage: string | undefined = error.response?.data?.message

    if (error.response?.status === 401 && isOnPublicAuthPage()) {
      return Promise.reject(error)
    }

    if (serverMessage?.toLowerCase().includes("deactivated")) {
      redirectToSignIn(serverMessage)
      return Promise.reject(error)
    }

    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      originalRequest.url !== "/auth/refresh"
    ) {
      if (isRefreshing) {
        await new Promise<void>((resolve) => pendingQueue.push(resolve))
        return apiClient(originalRequest)
      }

      originalRequest._retry = true
      isRefreshing = true

      try {
        await apiClient.post("/auth/refresh")
        pendingQueue.forEach((resolve) => resolve())
        pendingQueue = []
        return apiClient(originalRequest)
      } catch (refreshError: any) {
        pendingQueue = []
        const refreshMessage = refreshError.response?.data?.message
        redirectToSignIn(refreshMessage)
        return Promise.reject(refreshError)
      } finally {
        isRefreshing = false
      }
    }

    return Promise.reject(error)
  }
)