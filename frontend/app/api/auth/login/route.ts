import { NextRequest, NextResponse } from "next/server"
import { cookies } from "next/headers"

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5133"
const ACCESS_TOKEN_COOKIE = "access_token"
const REFRESH_TOKEN_COOKIE = "refresh_token"

export async function POST(request: NextRequest) {
  const body = await request.json()

  const backendRes = await fetch(`${API_URL}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  })

  const data = await backendRes.json()

  if (!backendRes.ok) {
    return NextResponse.json(data, { status: backendRes.status })
  }

  const cookieStore = await cookies()

  cookieStore.set(ACCESS_TOKEN_COOKIE, data.accessToken, {
    httpOnly: true,
    sameSite: "lax",
    secure: false, // flip to true once deployed behind HTTPS
    path: "/",
    expires: new Date(data.accessTokenExpiresAt),
  })

  cookieStore.set(REFRESH_TOKEN_COOKIE, data.refreshToken, {
    httpOnly: true,
    sameSite: "lax",
    secure: false,
    path: "/",
    maxAge: 60 * 60 * 24 * 7, // 7 days — matches backend RefreshTokenExpirationDays
  })

  return NextResponse.json({
    userId: data.userId,
    fullName: data.fullName,
    email: data.email,
    role: data.role,
  })
}