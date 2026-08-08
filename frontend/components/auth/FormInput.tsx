import { forwardRef, InputHTMLAttributes } from "react"

interface FormInputProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string
  error?: string
}

export const FormInput = forwardRef<HTMLInputElement, FormInputProps>(
  ({ label, error, ...rest }, ref) => {
    return (
      <div className="space-y-1">
        <label className="text-sm font-medium">{label}</label>
        <input
          ref={ref}
          {...rest}
          className={`w-full px-3 py-2 border rounded-lg text-sm outline-none focus:ring-2 focus:ring-black/20 ${
            error ? "border-red-400" : "border-gray-300"
          }`}
        />
        {error && <p className="text-xs text-red-500">{error}</p>}
      </div>
    )
  }
)

FormInput.displayName = "FormInput"