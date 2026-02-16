import type { CompanyProfile } from "@/modules/settings/types/companyProfile.types";


export const mockCompanyProfile: CompanyProfile = {
  companyName: 'FairWorkly Pty Ltd',
  abn: '12345678901',
  industryType: 'Retail',
  contactEmail: 'hello@fairworkly.com',
  phoneNumber: '+61 400 000 000',
  addressLine1: '123 Collins St',
  addressLine2: 'Level 4',
  suburb: 'Melbourne',
  state: 'VIC',
  postcode: '3000',
  logoUrl: null,
  awards: [
    {
      id: '1',
      awardType: 'Retail Award',
      isPrimary: true,
      employeeCount: 32,
      addedAt: '2024-01-20',
    },
  ],
}