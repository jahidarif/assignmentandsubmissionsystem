"use client"

import { useForm, Controller } from "react-hook-form"
import { useState } from "react"
import { AuthHeader, AuthButton, FormInput, Divider } from "@/components/auth"
import Link from "next/link"

type UserRole = "Teacher" | "Student"

interface RegisterFormData {
  fullName: string
  email: string
  password: string
  confirmPassword: string
  role: UserRole
}

interface RegisterFormProps {
  onSubmit?: (data: Omit<RegisterFormData, "confirmPassword">) => void | Promise<void>
  signInHref?: string
}

export function RegisterForm({ onSubmit, signInHref = "/auth/sign-in" }: RegisterFormProps) {
  const [error, setError] = useState<string | null>(null)

  const {
  register,
  handleSubmit,
  watch,
  control,
  formState: { errors, isSubmitting },
} = useForm<RegisterFormData>({
  mode: "onChange",   
  defaultValues: {
    fullName: "",
    email: "",
    password: "",
    confirmPassword: "",
    role: "Student",
  },
})

  const password = watch("password")

  const handleFormSubmit = async (data: RegisterFormData) => {
    setError(null)

    try {
      const { confirmPassword, ...payload } = data
      await onSubmit?.(payload)
    } catch (err) {
      console.error("registration error:", err)
      setError("An unexpected error occurred.")
    }
  }

  return (
    <div className="space-y-6">
      <AuthHeader title="Create Account" subTitle="Join your class or start teaching." />

      <Divider text="account details" />

      <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-4">
        {error && (
          <div className="p-3 bg-red-50 border border-red-200 rounded-lg">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        )}

        {/* Role selector — Controller keeps this in sync with RHF's internal
            state directly, no hidden input / setValue combo needed. */}
        <Controller
          name="role"
          control={control}
          rules={{ required: "Please select a role" }}
          render={({ field }) => (
            <div className="space-y-1">
              <label className="text-sm font-medium">I am a</label>
              <div className="grid grid-cols-2 gap-3">
                <button
                  type="button"
                  onClick={() => field.onChange("Student")}
                  className={`p-3 rounded-lg border-2 text-sm font-medium transition-colors ${
                    field.value === "Student"
                      ? "border-black bg-black text-white"
                      : "border-gray-200 hover:border-gray-300"
                  }`}
                >
                  Student
                </button>
                <button
                  type="button"
                  onClick={() => field.onChange("Teacher")}
                  className={`p-3 rounded-lg border-2 text-sm font-medium transition-colors ${
                    field.value === "Teacher"
                      ? "border-black bg-black text-white"
                      : "border-gray-200 hover:border-gray-300"
                  }`}
                >
                  Teacher
                </button>
              </div>
              {errors.role && <p className="text-xs text-red-500">{errors.role.message}</p>}
            </div>
          )}
        />

        <FormInput
          label="Full Name"
          type="text"
          placeholder="Jane Doe"
          error={errors.fullName?.message}
          {...register("fullName", {
            required: "Full name is required",
            minLength: { value: 2, message: "Name is too short" },
          })}
        />

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
          {...register("password", {
            required: "Password is required",
            minLength: {
              value: 6,
              message: "Password must be at least 6 characters",
            },
          })}
        />

        <FormInput
          label="Confirm Password"
          type="password"
          placeholder="Confirm your password"
          error={errors.confirmPassword?.message}
          {...register("confirmPassword", {
            required: "Please confirm your password",
            validate: (value) => value === password || "Passwords do not match",
          })}
        />

        <AuthButton
          type="submit"
          label="Sign up"
          loadingLabel="Creating account..."
          isSubmitting={isSubmitting}
        />
      </form>

      <p className="text-sm text-muted-foreground">
        Already have an account?{" "}
        <Link href={signInHref} className="font-medium text-accent hover:underline">
          Sign in.
        </Link>
      </p>
    </div>
  )
}