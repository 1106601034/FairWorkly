import { List } from '@mui/material'
import { NavContainer, SectionLabel, SectionTitle } from '../Sidebar.styles'
import { mainNavItems, settingsNavItems } from '../config/navigation.config.tsx'
import { NavItem } from './NavItem'
import { usePermissions } from '@/shared/hooks/usePermissions'

export function SidebarNav() {
  const { canAccessModule } = usePermissions()

  const visibleMainItems = mainNavItems.filter((item) => canAccessModule(item.requiredModule))
  const visibleSettingsItems = settingsNavItems.filter((item) => canAccessModule(item.requiredModule))

  return (
    <NavContainer>
      <nav aria-label="Main navigation">
        {visibleMainItems.length > 0 && (
          <>
            <SectionLabel>
              <SectionTitle variant="overline">Quick Actions</SectionTitle>
            </SectionLabel>

            <List disablePadding>
              {visibleMainItems.map(item => (
                <NavItem key={item.to} to={item.to} icon={item.icon} label={item.label} end />
              ))}
            </List>
          </>
        )}

        {visibleSettingsItems.length > 0 && (
          <>
            <SectionLabel>
              <SectionTitle variant="overline">Management</SectionTitle>
            </SectionLabel>

            <List disablePadding>
              {visibleSettingsItems.map(item => (
                <NavItem key={item.to} to={item.to} icon={item.icon} label={item.label} end />
              ))}
            </List>
          </>
        )}
      </nav>
    </NavContainer>
  )
}
