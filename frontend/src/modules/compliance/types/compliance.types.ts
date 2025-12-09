export type AudienceOption = "manager" | "employee";

export interface ComplianceQAFormValues {
  question: string;
  awardCode?: string;
  audience: AudienceOption;
}
