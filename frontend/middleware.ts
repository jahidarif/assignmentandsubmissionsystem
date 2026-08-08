import { NextRequest, NextResponse } from "next/server"
import { jwtVerify } from "jose"

const ACCESS_TOKEN_COOKIE = "access_token"
const REFRESH_TOKEN_COOKIE = "refresh_token"

const ROLE_HOME: Record<string, string> = {
  Admin: "/dashboard/admin",
  Teacher: "/dashboard/teacher",
  Student: "/dashboard/student",
}

const secret = new TextEncoder().encode(process.env.JWT_SECRET)

async function getRoleFromToken(token: string): Promise<string | null> {
  try {
    const { payload } = await jwtVerify(token, secret, {
      issuer: process.env.JWT_ISSUER,
      audience: process.env.JWT_AUDIENCE,
    })
    return (payload.role as string) ?? null
  } catch (err) {
    // Temporary — prints the exact validation failure reason to the
    // terminal running `npm run dev` (this runs server-side, so it
    // won't show in the browser console). Remove once login works.
    console.error("JWT verify failed:", err)
    return null
  }
}

export async function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl
  const accessToken = request.cookies.get(ACCESS_TOKEN_COOKIE)?.value

  let role = accessToken ? await getRoleFromToken(accessToken) : null
  let response = NextResponse.next()

  // Access token missing/expired but a refresh token exists — attempt a
  // silent refresh so a page reload/navigation doesn't bounce someone with
  // a still-valid session back to sign-in.
  if (!role) {
    const refreshToken = request.cookies.get(REFRESH_TOKEN_COOKIE)?.value

    if (refreshToken) {
      const refreshRes = await fetch(new URL("/api/auth/refresh", request.url), {
        method: "POST",
        headers: { cookie: request.headers.get("cookie") ?? "" },
      })

      if (refreshRes.ok) {
        const data = await refreshRes.json()
        role = data.role ?? null

        response = NextResponse.next()
        const setCookieHeaders =
          typeof refreshRes.headers.getSetCookie === "function"
            ? refreshRes.headers.getSetCookie()
            : [refreshRes.headers.get("set-cookie") ?? ""].filter(Boolean)

        setCookieHeaders.forEach((cookie) => {
          response.headers.append("set-cookie", cookie)
        })
      } else {
        console.error("Refresh attempt failed with status:", refreshRes.status)
      }
    }
  }

  if (!role) {
    const signInUrl = new URL("/auth/sign-in", request.url)
    signInUrl.searchParams.set("from", pathname)
    return NextResponse.redirect(signInUrl)
  }

  const allowedPrefix = ROLE_HOME[role]

  if (!allowedPrefix || !pathname.startsWith(allowedPrefix)) {
    return NextResponse.redirect(new URL(allowedPrefix ?? "/auth/sign-in", request.url))
  }

  return response
}

export const config = {
  matcher: ["/dashboard/:path*"],
}