
export interface CompanyProfile {
    companyName: string
    abn: string
    industryType: string
    contactEmail: string
    phoneNumber: string
    addressLine1: string
    addressLine2?: string
    suburb: string
    state: string
    postcode: string
    logoUrl: string | null
    awards: Award[]
}


export interface Award {
    id: string
    awardType: string
    isPrimary: boolean
    employeeCount: number
    addedAt: string
}


export interface BusinessInfo {
    companyName: string
    abn: string
    industryType: string
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