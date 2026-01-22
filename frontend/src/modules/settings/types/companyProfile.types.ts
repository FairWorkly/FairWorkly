// modules/settings/types/companyProfile.types.ts

/**
 * 公司资料完整数据类型
 * 用于表示后端返回的组织信息
 */
export interface CompanyProfile {
    companyName: string
    abn: string
    industryType: string
    contactEmail: string
    phoneNumber: string
    addressLine1: string
    addressLine2: string
    suburb: string
    state: string
    postcode: string
    logoUrl: string | null
    awards: Award[]
}

/**
 * Award规则信息
 */
export interface Award {
    id: string
    awardType: string
    isPrimary: boolean
    employeeCount: number
    addedAt: string
}

/**
 * 商业信息卡片数据
 */
export interface BusinessInfo {
    companyName: string
    abn: string
    industryType: string
    logoUrl: string | null
}

/**
 * 联系方式卡片数据
 */
export interface ContactInfo {
    contactEmail: string
    phoneNumber: string
}

/**
 * 地址卡片数据
 */
export interface AddressInfo {
    addressLine1: string
    addressLine2: string
    suburb: string
    state: string
    postcode: string
}

/**
 * 表单验证错误
 */
export interface ValidationErrors {
    [key: string]: string
}

/**
 * 澳洲州/领地选项
 */
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

/**
 * 行业类型选项（示例）
 */
export const INDUSTRY_TYPES = [
    'Retail',
    'Hospitality',
    'Healthcare',
    'Manufacturing',
    'Construction',
    'Education',
    'Technology',
    'Other',
] as const