export const FAIRBOT_ARIA = {
  CHAT_AREA: 'FairBot chat area',
  MESSAGE_LIST: 'FairBot conversation',
  MESSAGE_INPUT: 'FairBot message input',
  ACTION_CARDS: 'FairBot quick actions',
} as const

export const FAIRBOT_ROLES = {
  USER: 'user',
  ASSISTANT: 'assistant',
} as const

export const FAIRBOT_ACTION_CARDS = {
  ROSTER: {
    id: 'roster-check',
    title: 'Check Roster',
    description: 'Upload and validate roster against Fair Work regulations',
    route: '/roster',
  },
  PAYROLL: {
    id: 'payroll-check',
    title: 'Verify Payroll',
    description: 'Upload payslip CSV to check for underpayment risks',
    route: '/payroll',
  },
} as const
