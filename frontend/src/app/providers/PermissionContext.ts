import { createContext } from 'react'
import type { PermissionContextValue } from '@/shared/types/permissions.types'

/**
 * Permission context for app-wide access.
 * Use usePermissions() hook to consume this context.
 */
export const PermissionContext = createContext<PermissionContextValue | null>(null)
