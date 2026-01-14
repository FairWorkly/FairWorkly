import { useState } from 'react'
import { Typography } from '@mui/material'
import {
  Business as BusinessIcon,
  Group as GroupIcon,
  CreditCard as CreditCardIcon,
  Security as SecurityIcon,
} from '@mui/icons-material'
import {
  SettingsContainer,
  SettingsLayout,
  SettingsNav,
  SettingsNavItem,
  SettingsNavItemText,
  SettingsContent,
  PageHeader,
  SectionContent,
} from './Settings.styles'

type SettingsSection = 'company' | 'team' | 'billing' | 'security'

export function Settings() {
  const [activeSection, setActiveSection] = useState<SettingsSection>('company')

  const navItems = [
    {
      id: 'company' as const,
      icon: <BusinessIcon />,
      label: 'Company Profile',
    },
    {
      id: 'team' as const,
      icon: <GroupIcon />,
      label: 'Team Members',
    },
    {
      id: 'billing' as const,
      icon: <CreditCardIcon />,
      label: 'Billing',
    },
    {
      id: 'security' as const,
      icon: <SecurityIcon />,
      label: 'Security',
    },
  ]

  return (
    <SettingsContainer>
      <PageHeader>
        <Typography variant="h3" component="h1" gutterBottom>
          Settings
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Manage your company settings and team members
        </Typography>
      </PageHeader>

      <SettingsLayout>
        <SettingsNav>
          {navItems.map((item) => (
            <SettingsNavItem
              key={item.id}
              onClick={() => setActiveSection(item.id)}
              className={activeSection === item.id ? 'active' : ''}
            >
              {item.icon}
              <SettingsNavItemText variant="body2">{item.label}</SettingsNavItemText>
            </SettingsNavItem>
          ))}
        </SettingsNav>

        <SettingsContent>
          {activeSection === 'company' && (
            <SectionContent>
              <Typography variant="h5">Company Profile</Typography>
              <Typography variant="body2" color="text.secondary">
                Coming soon...
              </Typography>
            </SectionContent>
          )}

          {activeSection === 'team' && (
            <SectionContent>
              <Typography variant="h5">Team Members</Typography>
              <Typography variant="body2" color="text.secondary">
                Coming soon...
              </Typography>
            </SectionContent>
          )}

          {activeSection === 'billing' && (
            <SectionContent>
              <Typography variant="h5">Billing</Typography>
              <Typography variant="body2" color="text.secondary">
                Coming soon...
              </Typography>
            </SectionContent>
          )}

          {activeSection === 'security' && (
            <SectionContent>
              <Typography variant="h5">Security</Typography>
              <Typography variant="body2" color="text.secondary">
                Coming soon...
              </Typography>
            </SectionContent>
          )}
        </SettingsContent>
      </SettingsLayout>
    </SettingsContainer>
  )
}
