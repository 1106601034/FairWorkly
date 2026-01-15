import {
  Description as DescriptionOutlinedIcon,
  Payments as PaymentsOutlinedIcon,
  Settings as SettingsOutlinedIcon,
  Shield as ShieldOutlinedIcon,
} from '@mui/icons-material'
import type { ModuleId } from '@/shared/types/permissions.types'

export interface NavItemConfig {
  to: string
  icon: React.ReactElement
  label: string
  /** Module required to access this nav item. Uses canAccessModule() for filtering. */
  requiredModule: ModuleId
}

export const mainNavItems: NavItemConfig[] = [
  {
    to: '/roster',
    icon: <ShieldOutlinedIcon />,
    label: 'Check Roster',
    requiredModule: 'roster',
  },
  {
    to: '/payroll',
    icon: <PaymentsOutlinedIcon />,
    label: 'Verify Payroll',
    requiredModule: 'payroll',
  },
  {
    to: '/documents',
    icon: <DescriptionOutlinedIcon />,
    label: 'Documents Compliance',
    requiredModule: 'documents',
  },
]

export const settingsNavItems: NavItemConfig[] = [
  {
    to: '/settings',
    icon: <SettingsOutlinedIcon />,
    label: 'Settings',
    requiredModule: 'settings',
  },
]
