import type { ReactNode } from 'react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReduxProvider } from '@/app/providers/ReduxProvider'
import { ThemeProvider } from '@/app/providers/ThemeProvider'
import { PermissionProvider } from '@/app/providers/PermissionProvider'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
      staleTime: 60_000,
    },
  },
})

export function AppProviders({ children }: { children: ReactNode }) {
  return (
    <QueryClientProvider client={queryClient}>
      <ReduxProvider>
        <ThemeProvider>
          <PermissionProvider>{children}</PermissionProvider>
        </ThemeProvider>
      </ReduxProvider>
    </QueryClientProvider>
  )
}
