// modules/settings/components/CompanyProfile/CompanyProfile.tsx

import { useState } from 'react'
import { CompanyProfileContainer } from './CompanyProfile.styles'
import { BusinessInfoCard } from './BusinessInfoCard'
import { ContactCard } from './ContactCard'
import { AddressCard } from './AddressCard'
import { ActiveAwardsCard } from './ActiveAwardsCard'
import { mockCompanyProfile } from './mockData'
import type { BusinessInfo, ContactInfo, AddressInfo } from '../../types/companyProfile.types'

/**
 * Company Profile主组件
 * 
 * 职责：
 * 1. 管理所有卡片的数据状态
 * 2. 协调各卡片之间的数据流
 * 3. 处理保存操作（MVP阶段只是更新本地state）
 * 
 * 未来：
 * - 集成真实API调用
 * - 添加loading状态
 * - 添加错误处理
 * - 添加成功提示
 */
export function CompanyProfile() {
  // 使用mock数据初始化state
  // 未来：从API加载数据
  const [profile, setProfile] = useState(mockCompanyProfile)

  /**
   * 保存Business Info
   * MVP: 只更新本地state
   * 未来: 调用API PUT /settings/organization
   */
  const handleSaveBusinessInfo = (data: BusinessInfo) => {
    setProfile(prev => ({
      ...prev,
      ...data,
    }))
    console.log('Business Info saved:', data)
    // TODO: 调用API
  }

  /**
   * 保存Contact Info
   */
  const handleSaveContact = (data: ContactInfo) => {
    setProfile(prev => ({
      ...prev,
      ...data,
    }))
    console.log('Contact Info saved:', data)
    // TODO: 调用API
  }

  /**
   * 保存Address Info
   */
  const handleSaveAddress = (data: AddressInfo) => {
    setProfile(prev => ({
      ...prev,
      ...data,
    }))
    console.log('Address saved:', data)
    // TODO: 调用API
  }

  return (
    <CompanyProfileContainer>
      {/* Business Info卡片 */}
      <BusinessInfoCard
        data={{
          companyName: profile.companyName,
          abn: profile.abn,
          industryType: profile.industryType,
          logoUrl: profile.logoUrl,
        }}
        onSave={handleSaveBusinessInfo}
      />

      {/* Contact卡片 */}
      <ContactCard
        data={{
          contactEmail: profile.contactEmail,
          phoneNumber: profile.phoneNumber,
        }}
        onSave={handleSaveContact}
      />

      {/* Address卡片 */}
      <AddressCard
        data={{
          addressLine1: profile.addressLine1,
          addressLine2: profile.addressLine2,
          suburb: profile.suburb,
          state: profile.state,
          postcode: profile.postcode,
        }}
        onSave={handleSaveAddress}
      />

      {/* Active Awards卡片 */}
      <ActiveAwardsCard awards={profile.awards} />
    </CompanyProfileContainer>
  )
}