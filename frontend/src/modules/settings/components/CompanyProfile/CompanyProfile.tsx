// modules/settings/components/CompanyProfile/CompanyProfile.tsx

import { useState } from 'react'
import { BusinessInfoCard } from './BusinessInfoCard'
import { ContactCard } from './ContactCard'
import { AddressCard } from './AddressCard'
import { ActiveAwardsCard } from './ActiveAwardsCard'
import { mockCompanyProfile } from './mockData'
import type { BusinessInfo, ContactInfo, AddressInfo, Award } from '../../types/companyProfile.types'
import type { AwardType } from '@/shared/compliance-check'


export function CompanyProfile() {

  const [profile, setProfile] = useState(mockCompanyProfile)


  const handleSaveBusinessInfo = (data: BusinessInfo) => {
    setProfile(prev => ({
      ...prev,
      ...data,
    }))
    // TODO: 调用API
  }


  const handleSaveContact = (data: ContactInfo) => {
    setProfile(prev => ({
      ...prev,
      ...data,
    }))
    // TODO: 调用API
  }


  const handleSaveAddress = (data: AddressInfo) => {
    setProfile(prev => ({
      ...prev,
      ...data,
    }))
    // TODO: 调用API
  }

  const handleAddAward = (awardType: AwardType, employeeCount: number) => {
    const newAward: Award = {
      id: `temp-${Date.now()}`, // 临时ID
      awardType: awardType,
      isPrimary: profile.awards.length === 0, // 第一个设为Primary
      employeeCount: employeeCount,
      addedAt: new Date().toISOString(),
    }

    setProfile(prev => ({
      ...prev,
      awards: [...prev.awards, newAward],
    }))

    console.log('Award added:', newAward)
    // TODO: 调用API
    // const response = await organizationApi.addAward({ awardType, employeeCount })
    // setProfile(prev => ({ ...prev, awards: [...prev.awards, response.data] }))
  }

  const handleDeleteAward = (awardId: string) => {
    setProfile(prev => ({
      ...prev,
      awards: prev.awards.filter(award => award.id !== awardId),
    }))

    console.log('Award deleted:', awardId)
    // TODO: 调用API
    // await organizationApi.deleteAward(awardId)
  }
  return (
    <>
      <BusinessInfoCard
        data={{
          companyName: profile.companyName,
          abn: profile.abn,
          industryType: profile.industryType,
          logoUrl: profile.logoUrl,
        }}
        onSave={handleSaveBusinessInfo}
      />

      <ContactCard
        data={{
          contactEmail: profile.contactEmail,
          phoneNumber: profile.phoneNumber,
        }}
        onSave={handleSaveContact}
      />

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

      <ActiveAwardsCard
        awards={profile.awards}
        onAddAward={handleAddAward}
        onDeleteAward={handleDeleteAward}
      />
    </>
  )
}