export function Divider({ text = "or" }: { text?: string }) {
  return (
    <div className="flex items-center gap-3">
      <div className="flex-1 h-px bg-gray-200" />
      <span className="text-xs text-muted-foreground">{text}</span>
      <div className="flex-1 h-px bg-gray-200" />
    </div>
  )
}