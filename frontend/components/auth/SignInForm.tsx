"use client"

import { useForm } from "react-hook-form"
import { useState } from "react"
import { AuthHeader, AuthButton, FormInput, Divider } from "@/components/auth"
import Link from "next/link"

interface SignInFormData {
  email: string
  password: string
}

interface SignInFormProps {
  onSubmit?: (data: SignInFormData) => void | Promise<void>
  signUpHref?: string
}

export function SignInForm({ onSubmit, signUpHref = "/auth/sign-up" }: SignInFormProps) {
  const [error, setError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<SignInFormData>({ mode: "onChange" })

  const handleFormSubmit = async (data: SignInFormData) => {
    setError(null)
    try {
      await onSubmit?.(data)
    } catch (err) {
      console.error("login error:", err)
      setError("An unexpected error occurred.")
    }
  }

  return (
    <div className="space-y-6">
      <AuthHeader title="Sign In" subTitle="Welcome back!" />

      <Divider />

      <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-4">
        {error && (
          <div className="p-3 bg-red-50 border border-red-200 rounded-lg">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        )}

        <FormInput
          label="Email"
          type="email"
          placeholder="example@mail.com"
          error={errors.email?.message}
          {...register("email", {
            required: "Email is required",
            pattern: {
              value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
              message: "Invalid email address",
            },
          })}
        />

        <FormInput
          label="Password"
          type="password"
          placeholder="Password"
          error={errors.password?.message}
          {...register("password", { required: "Password is required" })}
        />

        <AuthButton
          type="submit"
          label="Sign in"
          loadingLabel="Signing in..."
          isSubmitting={isSubmitting}
        />
      </form>

      <p className="text-sm text-muted-foreground">
        <Link href={signUpHref} className="font-medium text-accent hover:underline">
          Create an account.
        </Link>
      </p>
    </div>
  )
}