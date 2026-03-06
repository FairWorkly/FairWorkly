import { useState } from 'react'
import {
  Business as BusinessIcon,
  Group as GroupIcon,
  CreditCard as CreditCardIcon,
} from '@mui/icons-material'
import {
  SettingsContainer,
  SettingsLayout,
  SettingsNav,
  SettingsNavItem,
  SettingsNavItemText,
  SettingsContent,
} from '@/modules/settings/ui/Settings.styles'
import { CompanyProfileSection } from '../features/CompanyProfile/CompanyProfileSection'
import { TeamMembersSection } from '@/modules/settings/features'
import { BillingSection } from '../features/Billing/BillingSection'

type SettingsSection = 'company' | 'team' | 'billing'

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
  ]

  return (
    <SettingsContainer>
      <SettingsLayout>
        <SettingsNav>
          {navItems.map(item => (
            <SettingsNavItem
              key={item.id}
              onClick={() => setActiveSection(item.id)}
              className={activeSection === item.id ? 'active' : ''}
            >
              {item.icon}
              <SettingsNavItemText variant="body2">
                {item.label}
              </SettingsNavItemText>
            </SettingsNavItem>
          ))}
        </SettingsNav>

        <SettingsContent>
          {activeSection === 'company' && <CompanyProfileSection />}

          {activeSection === 'team' && <TeamMembersSection />}

          {activeSection === 'billing' && <BillingSection />}
        </SettingsContent>
      </SettingsLayout>
    </SettingsContainer>
  )
}
