import { useMemo, useCallback, type ReactNode } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useAuth } from '@/shared/hooks/useAuth'
import { getPermissions } from '@/services/permissionsApi'
import { PermissionContext } from '@/app/providers/PermissionContext'
import {
  type Role,
  type Permission,
  type ModuleId,
  type ModuleAccess,
  DEFAULT_ROLE_PERMISSIONS,
  DEFAULT_ROLE_MODULES,
} from '@/shared/types/permissions.types'

/**
 * Query key for permissions - includes role to invalidate on role change.
 */
const PERMISSIONS_QUERY_KEY = 'permissions'

/**
 * Cache duration: 5 minutes.
 */
const PERMISSIONS_STALE_TIME = 5 * 60 * 1000

interface PermissionProviderProps {
  children: ReactNode
}

/**
 * PermissionProvider loads and caches user permissions.
 *
 * In dev mode (when backend returns 404), falls back to DEFAULT_ROLE_PERMISSIONS
 * based on the current user's role from useAuth.
 *
 * Usage:
 * ```tsx
 * <PermissionProvider>
 *   <App />
 * </PermissionProvider>
 * ```
 */
export function PermissionProvider({ children }: PermissionProviderProps) {
  const { user, isAuthenticated } = useAuth()
  const userRole = user?.role as Role | undefined

  // Fetch permissions from backend (with fallback for dev mode)
  const {
    data: permissionsData,
    isLoading: isQueryLoading,
    error: queryError,
  } = useQuery({
    queryKey: [PERMISSIONS_QUERY_KEY, userRole],
    queryFn: getPermissions,
    enabled: isAuthenticated,
    staleTime: PERMISSIONS_STALE_TIME,
    retry: false, // Don't retry on 401/404 - fall back to defaults
  })

  // Derive permissions: use backend data or fall back to role-based defaults
  const { role, permissions, modules } = useMemo(() => {
    // If we have backend data, use it
    if (permissionsData) {
      return {
        role: permissionsData.role,
        permissions: permissionsData.permissions,
        modules: permissionsData.modules,
      }
    }

    // Fall back to defaults based on user role (for dev mode)
    if (userRole) {
      const defaultPerms = DEFAULT_ROLE_PERMISSIONS[userRole] ?? []
      const defaultModuleIds = DEFAULT_ROLE_MODULES[userRole] ?? []
      const defaultModules: ModuleAccess[] = defaultModuleIds.map((m) => ({
        module: m,
        canAccess: true,
        features: [],
      }))

      return {
        role: userRole,
        permissions: defaultPerms,
        modules: defaultModules,
      }
    }

    // No user, no permissions
    return {
      role: null,
      permissions: [],
      modules: [],
    }
  }, [permissionsData, userRole])

  // Check if user has a specific permission
  const hasPermission = useCallback(
    (permission: Permission): boolean => {
      return permissions.includes(permission)
    },
    [permissions],
  )

  // Check if user can access a module
  const canAccessModule = useCallback(
    (moduleId: ModuleId): boolean => {
      return modules.some((m) => m.module === moduleId && m.canAccess)
    },
    [modules],
  )

  // Only show loading on initial load, not on background refetches
  const isLoading = isQueryLoading && !permissionsData && isAuthenticated

  // Convert query error to standard Error (or null), memoized to avoid reference changes
  const error = useMemo(
    () => (queryError ? new Error(String(queryError)) : null),
    [queryError],
  )

  const contextValue = useMemo<PermissionContextValue>(
    () => ({
      role,
      permissions,
      modules,
      hasPermission,
      canAccessModule,
      isLoading,
      error,
    }),
    [role, permissions, modules, hasPermission, canAccessModule, isLoading, error],
  )

  return <PermissionContext.Provider value={contextValue}>{children}</PermissionContext.Provider>
}
