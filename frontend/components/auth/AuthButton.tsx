interface AuthButtonProps {
  type: "submit" | "button"
  label: string
  loadingLabel?: string
  isSubmitting?: boolean
  onClick?: () => void
}

export function AuthButton({
  type,
  label,
  loadingLabel = "Loading...",
  isSubmitting = false,
  onClick,
}: AuthButtonProps) {
  return (
    <button
      type={type}
      onClick={onClick}
      disabled={isSubmitting}
      className="w-full py-2.5 rounded-lg bg-black text-white font-medium hover:bg-gray-800 disabled:opacity-60 disabled:cursor-not-allowed transition-colors"
    >
      {isSubmitting ? loadingLabel : label}
    </button>
  )
}