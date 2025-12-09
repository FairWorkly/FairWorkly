import * as Constants from "../constants/ComplianceConstants";

export type AudienceOption = "manager" | "employee";
export type AwardCode = keyof typeof Constants.AWARD_OPTIONS;
export interface ComplianceQAFormValues {
  question: string;
  awardCode?: AwardCode;
  audience: AudienceOption;
}
