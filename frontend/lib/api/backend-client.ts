import axios from "axios"

// Direct-to-backend client — only for endpoints that don't need the
// httpOnly cookie session, e.g. registration.
export const backendClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5133",
  headers: { "Content-Type": "application/json" },
})