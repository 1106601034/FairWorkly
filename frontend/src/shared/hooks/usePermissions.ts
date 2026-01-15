import { useContext } from 'react'
import { PermissionContext } from '@/app/providers/PermissionContext'
import type { PermissionContextValue } from '@/shared/types/permissions.types'

/**
 * Hook to access permission context.
 *
 * Provides:
 * - `role`: Current user's role (or null if not authenticated)
 * - `permissions`: Array of permission tokens the user has
 * - `modules`: Array of module access info
 * - `hasPermission(permission)`: Check if user has a specific permission
 * - `canAccessModule(moduleId)`: Check if user can access a module
 * - `isLoading`: True while permissions are being fetched
 * - `error`: Error object if permission fetch failed
 *
 * @throws Error if used outside of PermissionProvider
 *
 * @example
 * ```tsx
 * function MyComponent() {
 *   const { hasPermission, canAccessModule } = usePermissions()
 *
 *   if (!hasPermission(Permission.CheckPayrollCompliance)) {
 *     return null
 *   }
 *
 *   return <PayrollChecker />
 * }
 * ```
 */
export function usePermissions(): PermissionContextValue {
  const context = useContext(PermissionContext)

  if (!context) {
    throw new Error('usePermissions must be used within a PermissionProvider')
  }

  return context
}
