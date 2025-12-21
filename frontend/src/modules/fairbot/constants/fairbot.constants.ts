export const FAIRBOT_LABELS = {
  TITLE: 'FairBot AI Assistant',
  SUBTITLE:
    'Ask questions, upload roster/ payroll files, and get help with Fair Work compliance',
  WELCOME_TITLE:
    "Hi John! I'm FairBot, your AI-powered Fair Work assistant. I can help you with:",
  WELCOME_BULLETS: [
    'Roster compliance - Upload and check for penalty rate issues',
    'Payroll verification - Identify underpayment risks',
    'Fair Work questions',
  ],
  PROMPT_QUESTION: 'What would you like help with today?',
  INPUT_PLACEHOLDER: 'Type your message or upload a file...',
  EMPTY_RESULTS_TITLE: 'Results will appear here',
  EMPTY_RESULTS_SUBTITLE:
    "Upload a file or ask a question, and I'll display a quick summary with links to detailed reports.",
  UPLOAD_TIP: 'Tip: Upload roster or payroll files (.xlsx, .csv) for instant analysis',
  CHECK_ROSTER: 'Check Roster',
  VERIFY_PAYROLL: 'Verify Payroll',
  ASK_QUESTION: 'Ask a Question',
  VIEW_DETAILED_REPORT: 'View Detailed Report',
  LOADING_MESSAGE: 'FairBot is thinking...',
  ERROR_GENERIC: 'Something went wrong. Please try again.',
  ATTACHMENT_LABEL: 'Attachment',
} as const

export const FAIRBOT_ARIA = {
  CHAT_AREA: 'FairBot chat area',
  RESULTS_PANEL: 'FairBot results panel',
  MESSAGE_LIST: 'FairBot conversation',
  MESSAGE_INPUT: 'FairBot message input',
  FILE_UPLOAD: 'File upload',
  QUICK_ACTIONS: 'FairBot quick actions',
} as const

export const FAIRBOT_SESSION_KEYS = {
  CONVERSATION: 'fairbot_conversation',
  RESULTS: 'fairbot_results',
} as const

export const FAIRBOT_LAYOUT = {
  CHAT_MAX_WIDTH: 760,
  RESULTS_PANEL_WIDTH: 360,
  SIDEBAR_WIDTH: 220,
  CONTENT_GAP: 24,
  CHAT_SCROLL_HEIGHT: 540,
  MOBILE_BREAKPOINT: 900,
} as const

export const FAIRBOT_FILE = {
  ACCEPTED_TYPES: ['csv', 'xlsx'],
  ACCEPTED_MIME: [
    'text/csv',
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  ],
  MAX_SIZE_BYTES: 5 * 1024 * 1024,
} as const

export const FAIRBOT_RESULTS = {
  TYPES: {
    PAYROLL: 'payroll',
    ROSTER: 'roster',
    EMPLOYEE: 'employee',
    DOCUMENT: 'document',
  },
} as const

export const FAIRBOT_QUICK_ACTIONS = {
  COLORS: {
    BLUE: 'blue',
    GREEN: 'green',
    PURPLE: 'purple',
    ORANGE: 'orange',
  },
  IDS: {
    CHECK_PAYROLL: 'check-payroll',
    CHECK_ROSTER: 'check-roster',
    ASK_QUESTION: 'ask-question',
    GENERATE_CONTRACT: 'generate-contract',
  },
} as const
