export type AskComplianceRequest = {
    question: string;
    awardCode?: string;
    audience: "manager" | "employee";
  };

  export type AskComplianceQuestionResponse = Record<string,unknown>;