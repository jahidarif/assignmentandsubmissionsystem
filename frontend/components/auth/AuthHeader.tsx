interface AuthHeaderProps {
  title: string
  subTitle?: string
}

export function AuthHeader({ title, subTitle }: AuthHeaderProps) {
  return (
    <div className="text-center space-y-1">
      <h1 className="text-2xl font-semibold">{title}</h1>
      {subTitle && <p className="text-sm text-muted-foreground">{subTitle}</p>}
    </div>
  )
}