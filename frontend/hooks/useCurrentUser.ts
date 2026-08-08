import { useQuery } from "@tanstack/react-query"
import { getCurrentUser } from "@/lib/api/me"

export function useCurrentUser() {
  return useQuery({
    queryKey: ["currentUser"],
    queryFn: getCurrentUser,
    retry: false,
    staleTime: 60 * 1000,
  })
}