import { NextResponse } from "next/server"
import { cookies } from "next/headers"

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5133"
const ACCESS_TOKEN_COOKIE = "access_token"

export async function GET() {
  const cookieStore = await cookies()
  const accessToken = cookieStore.get(ACCESS_TOKEN_COOKIE)?.value

  if (!accessToken) {
    console.log("[/api/auth/me proxy] no access_token cookie present")
    return NextResponse.json({ message: "Not authenticated." }, { status: 401 })
  }

  const backendRes = await fetch(`${API_URL}/api/auth/me`, {
    method: "GET",
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  })

  const data = await backendRes.json()

  console.log("[/api/auth/me proxy] backend responded:", backendRes.status, JSON.stringify(data))

  if (!backendRes.ok) {
    return NextResponse.json(data, { status: backendRes.status })
  }

  return NextResponse.json(data)
}