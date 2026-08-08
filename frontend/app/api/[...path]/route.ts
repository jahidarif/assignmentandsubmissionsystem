import { NextRequest, NextResponse } from "next/server"
import { cookies } from "next/headers"

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5133"
const ACCESS_TOKEN_COOKIE = "access_token"

async function forward(request: NextRequest, method: string, path: string[]) {
  const cookieStore = await cookies()
  const accessToken = cookieStore.get(ACCESS_TOKEN_COOKIE)?.value

  const targetPath = path.join("/")
  const search = request.nextUrl.search // includes leading "?" if present
  const url = `${API_URL}/api/${targetPath}${search}`

  const headers: Record<string, string> = { "Content-Type": "application/json" }
  if (accessToken) {
    headers["Authorization"] = `Bearer ${accessToken}`
  }

  let body: string | undefined
  if (method !== "GET" && method !== "DELETE") {
    const text = await request.text()
    if (text) body = text
  }

  const backendRes = await fetch(url, { method, headers, body })

  if (backendRes.status === 204) {
    return new NextResponse(null, { status: 204 })
  }

  const contentType = backendRes.headers.get("content-type") ?? ""
  if (contentType.includes("application/json")) {
    const data = await backendRes.json()
    return NextResponse.json(data, { status: backendRes.status })
  }

  const text = await backendRes.text()
  return new NextResponse(text, { status: backendRes.status })
}

type RouteParams = { params: Promise<{ path: string[] }> }

export async function GET(request: NextRequest, { params }: RouteParams) {
  return forward(request, "GET", (await params).path)
}
export async function POST(request: NextRequest, { params }: RouteParams) {
  return forward(request, "POST", (await params).path)
}
export async function PUT(request: NextRequest, { params }: RouteParams) {
  return forward(request, "PUT", (await params).path)
}
export async function PATCH(request: NextRequest, { params }: RouteParams) {
  return forward(request, "PATCH", (await params).path)
}
export async function DELETE(request: NextRequest, { params }: RouteParams) {
  return forward(request, "DELETE", (await params).path)
}