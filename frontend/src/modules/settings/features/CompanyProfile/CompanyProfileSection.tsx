import { Alert, Snackbar } from '@mui/material'
import { BusinessInfoCard } from '../../components/CompanyProfile/BusinessInfoCard'
import { ContactCard } from '../../components/CompanyProfile/ContactCard'
import { AddressCard } from '../../components/CompanyProfile/AddressCard'
import {
  useOrganizationProfile,
  useUpdateOrganizationProfile,
} from '../../hooks/useOrganizationProfile'
import type {
  BusinessInfo,
  ContactInfo,
  AddressInfo,
} from '../../types/companyProfile.types'
import type { UpdateOrganizationProfileRequest } from '@/services/settingsApi'
import { useNotification } from '@/shared/hooks'
import { SectionWrapper, CardSkeleton } from './CompanyProfileSection.styles'

export function CompanyProfileSection() {
  const {
    data: profile,
    isLoading,
    error: loadError,
  } = useOrganizationProfile()
  const updateMutation = useUpdateOrganizationProfile()
  const { notification, notify, clear } = useNotification()

  if (isLoading) {
    return (
      <SectionWrapper>
        {[1, 2, 3].map(i => (
          <CardSkeleton key={i} variant="rounded" />
        ))}
      </SectionWrapper>
    )
  }

  if (loadError) {
    return (
      <SectionWrapper>
        <Alert severity="error">
          Failed to load organization profile. {loadError.message}
        </Alert>
      </SectionWrapper>
    )
  }

  if (!profile) return null

  function buildPayload(
    overrides: Partial<UpdateOrganizationProfileRequest>
  ): UpdateOrganizationProfileRequest {
    return {
      companyName: profile!.companyName,
      abn: profile!.abn,
      industryType: profile!.industryType,
      contactEmail: profile!.contactEmail,
      phoneNumber: profile!.phoneNumber,
      addressLine1: profile!.addressLine1,
      addressLine2: profile!.addressLine2,
      suburb: profile!.suburb,
      state: profile!.state,
      postcode: profile!.postcode,
      ...overrides,
    }
  }

  function saveCard(
    overrides: Partial<UpdateOrganizationProfileRequest>,
    message: string
  ): Promise<boolean> {
    return new Promise(resolve => {
      updateMutation.mutate(buildPayload(overrides), {
        onSuccess: () => {
          notify(message)
          resolve(true)
        },
        onError: () => {
          notify('Failed to save. Please try again.', 'error')
          resolve(false)
        },
      })
    })
  }

  const handleSaveBusinessInfo = (data: BusinessInfo) =>
    saveCard(data, 'Business info updated successfully')

  const handleSaveContact = (data: ContactInfo) =>
    saveCard(data, 'Contact updated successfully')

  const handleSaveAddress = (data: AddressInfo) =>
    saveCard(data, 'Address updated successfully')

  const isSaving = updateMutation.isPending

  return (
    <SectionWrapper>
      <BusinessInfoCard
        data={{
          companyName: profile.companyName,
          abn: profile.abn,
          industryType: profile.industryType,
          logoUrl: profile.logoUrl,
        }}
        onSave={handleSaveBusinessInfo}
        isSaving={isSaving}
      />

      <ContactCard
        data={{
          contactEmail: profile.contactEmail,
          phoneNumber: profile.phoneNumber ?? '',
        }}
        onSave={handleSaveContact}
        isSaving={isSaving}
      />

      <AddressCard
        data={{
          addressLine1: profile.addressLine1,
          addressLine2: profile.addressLine2 ?? undefined,
          suburb: profile.suburb,
          state: profile.state,
          postcode: profile.postcode,
        }}
        onSave={handleSaveAddress}
        isSaving={isSaving}
      />

      <Snackbar open={!!notification} autoHideDuration={3000} onClose={clear}>
        {notification ? (
          <Alert
            onClose={clear}
            severity={notification.severity}
            variant="filled"
          >
            {notification.message}
          </Alert>
        ) : undefined}
      </Snackbar>
    </SectionWrapper>
  )
}
