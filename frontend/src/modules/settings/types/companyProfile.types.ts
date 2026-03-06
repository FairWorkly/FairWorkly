export interface BusinessInfo {
    companyName: string
    abn: string
    industryType: string
    primaryAward: AwardValue | null
    logoUrl: string | null
}

export interface ContactInfo {
  contactEmail: string
  phoneNumber: string
}

export interface AddressInfo {
  addressLine1: string
  addressLine2?: string
  suburb: string
  state: string
  postcode: string
}

export interface ValidationErrors {
  [key: string]: string
}

export const AUSTRALIAN_STATES = [
  { value: 'NSW', label: 'New South Wales' },
  { value: 'VIC', label: 'Victoria' },
  { value: 'QLD', label: 'Queensland' },
  { value: 'SA', label: 'South Australia' },
  { value: 'WA', label: 'Western Australia' },
  { value: 'TAS', label: 'Tasmania' },
  { value: 'NT', label: 'Northern Territory' },
  { value: 'ACT', label: 'Australian Capital Territory' },
] as const

export const INDUSTRY_TYPES = [
    'Retail',
    'Hospitality',
    'Clerks',
    'Other',
] as const


export const AWARD_TYPES = [
    {
        value: 'GeneralRetailIndustryAward2020',
        label: 'General Retail Industry Award 2020',
        maCode: 'MA000004',
        industry: 'Retail',
    },
    {
        value: 'HospitalityIndustryAward2020',
        label: 'Hospitality Industry (General) Award 2020',
        maCode: 'MA000009',
        industry: 'Hospitality',
    },
    {
        value: 'ClerksPrivateSectorAward2020',
        label: 'Clerks—Private Sector Award 2020',
        maCode: 'MA000002',
        industry: 'Clerks',
    },
] as const

export type AwardValue = typeof AWARD_TYPES[number]['value']

/** Returns the suggested award value for a given industry type, or null for 'Other'. */
export function suggestAwardForIndustry(industryType: string): AwardValue | null {
    return AWARD_TYPES.find(a => a.industry === industryType)?.value ?? null
}
