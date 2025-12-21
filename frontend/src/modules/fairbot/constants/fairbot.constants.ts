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
  QUICK_ACTIONS_HEADER: 'Quick Actions',
  GENERATE_CONTRACT: 'Generate Employment Contract',
  CHECK_PAYROLL_COMPLIANCE: 'Check Payroll Compliance',
  CHECK_ROSTER_COMPLIANCE: 'Check Roster Compliance',
  RESULTS_PANEL_TITLE: 'Results Summary',
  RESULTS_PANEL_SUBTITLE: 'Summary based on your latest request.',
  PAYROLL_SUMMARY_TITLE: 'Payroll Check Complete',
  ROSTER_SUMMARY_TITLE: 'Roster Check Complete',
  EMPLOYEE_SUMMARY_TITLE: 'Employee Review Complete',
  DOCUMENT_SUMMARY_TITLE: 'Document Generation Complete',
  TOTAL_RECORDS_LABEL: 'Total Records',
  SHIFT_COUNT_LABEL: 'Shifts Reviewed',
  EMPLOYEES_REVIEWED_LABEL: 'Employees Reviewed',
  DOCUMENTS_GENERATED_LABEL: 'Documents Generated',
  ISSUES_FOUND_LABEL: 'Issues Found',
  TOP_ISSUES_LABEL: 'Top Issues',
  NO_PAYROLL_ISSUES: 'No payroll issues detected.',
  NO_ROSTER_ISSUES: 'No roster issues detected.',
  EMPLOYEE_ISSUES_SUMMARY: 'Employee review summary',
  DOCUMENTS_SUMMARY: 'Document generation summary',
  VIEW_RESULTS_TITLE: 'View results',
  RESULTS_PANEL_HINT: 'Open detailed reports to review findings.',
} as const

export const FAIRBOT_ARIA = {
  CHAT_AREA: 'FairBot chat area',
  RESULTS_PANEL: 'FairBot results panel',
  MESSAGE_LIST: 'FairBot conversation',
  MESSAGE_INPUT: 'FairBot message input',
  FILE_UPLOAD: 'File upload',
  QUICK_ACTIONS: 'FairBot quick actions',
} as const

export const FAIRBOT_TEXT = {
  EMPTY: '',
  FULL_SPAN: '1 / -1',
} as const

export const FAIRBOT_ENV = {
  TYPEOF_UNDEFINED: 'undefined',
} as const

export const FAIRBOT_SESSION_KEYS = {
  CONVERSATION: 'fairbot_conversation',
  RESULTS: 'fairbot_results',
} as const

export const FAIRBOT_ROLES = {
  USER: 'user',
  ASSISTANT: 'assistant',
} as const

export const FAIRBOT_MESSAGES = {
  ASSISTANT_DEFAULT: 'Thanks! I can help with that.',
  ASSISTANT_FILE_RECEIVED: 'Thanks for the file. I will review it now.',
} as const

export const FAIRBOT_NUMBERS = {
  ZERO: 0,
  ONE: 1,
  TWO: 2,
  THREE: 3,
  FOUR: 4,
  FIVE: 5,
} as const

export const FAIRBOT_KEYWORDS = {
  PAYROLL: 'payroll',
  ROSTER: 'roster',
  EMPLOYEE: 'employee',
  DOCUMENT: 'document',
  CONTRACT: 'contract',
} as const

export const FAIRBOT_LAYOUT = {
  CHAT_MAX_WIDTH: 760,
  RESULTS_PANEL_WIDTH: 360,
  SIDEBAR_WIDTH: 220,
  CONTENT_GAP: 24,
  CHAT_SCROLL_HEIGHT: 540,
  MOBILE_BREAKPOINT: 900,
  QUICK_ACTIONS_GAP: 12,
  QUICK_ACTIONS_COLUMNS: 2,
  RESULTS_PANEL_PADDING: 24,
  RESULTS_PANEL_GAP: 16,
} as const

export const FAIRBOT_UPLOAD = {
  BORDER_RADIUS: 16,
  BORDER_WIDTH: 1,
  PADDING: 12,
  GAP: 8,
  TRANSITION_MS: 150,
  MIN_HEIGHT: 48,
  BORDER_STYLE: 'dashed',
  TRANSITION_EASING: 'ease',
  TRANSITION_PROPERTIES: 'border-color, background-color',
} as const

export const FAIRBOT_QUICK_ACTIONS_UI = {
  CARD_PADDING: 12,
  CARD_RADIUS: 12,
  CARD_BORDER_WIDTH: 1,
  CARD_MIN_HEIGHT: 88,
  ICON_SIZE: 20,
  CONTENT_GAP: 4,
  TRANSITION_MS: 150,
  HOVER_TRANSLATE_Y_PX: -1,
  COLOR_MAP: {
    blue: { background: 'info', border: 'info' },
    green: { background: 'success', border: 'success' },
    purple: { background: 'secondary', border: 'secondary' },
    orange: { background: 'warning', border: 'warning' },
  },
  FULL_SPAN_COLUMNS: 2,
} as const

export const FAIRBOT_RESULTS_UI = {
  HEADER_ICON_SIZE: 56,
  CARD_RADIUS: 16,
  CARD_PADDING: 16,
  EMPTY_ICON_RADIUS: 48,
  EMPTY_ICON_SIZE: 32,
  STAT_GAP: 12,
  LIST_GAP: 8,
  EMPTY_ICON_BG: 'action.hover',
  STATS_GRID_COLUMNS: 2,
  STACK_GAP: 16,
  MIN_HEIGHT: 320,
  CARD_BORDER_WIDTH: 1,
  HEADER_GAP: 4,
} as const

export const FAIRBOT_TIMING = {
  TYPING_INDICATOR_DELAY_MS: 200,
  TYPING_INDICATOR_MIN_MS: 800,
} as const

export const FAIRBOT_FILE = {
  ACCEPTED_TYPES: ['csv', 'xlsx'],
  ACCEPTED_MIME: [
    'text/csv',
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  ],
  MAX_SIZE_BYTES: 5 * 1024 * 1024,
  MAX_SIZE_LABEL: '5MB',
  ACCEPT_ATTRIBUTE: '.csv,.xlsx',
  NAME_SEPARATOR: '.',
} as const

export const FAIRBOT_ERRORS = {
  INVALID_FILE_TYPE: 'Unsupported file type. Please upload a CSV or XLSX file.',
  FILE_TOO_LARGE: `File is too large. Max size is ${FAIRBOT_FILE.MAX_SIZE_LABEL}.`,
  FILE_REQUIRED: 'Please select a file to continue.',
} as const

export const FAIRBOT_RESULTS = {
  TYPES: {
    PAYROLL: 'payroll',
    ROSTER: 'roster',
    EMPLOYEE: 'employee',
    DOCUMENT: 'document',
  },
} as const

export const FAIRBOT_ROUTES = {
  PAYROLL: '/payroll/upload',
  ROSTER: '/compliance/upload',
  EMPLOYEE: '/my-profile',
  DOCUMENT: '/documents',
} as const

export const FAIRBOT_QUICK_ACTIONS_TEXT = {
  CHECK_PAYROLL: {
    TITLE: FAIRBOT_LABELS.CHECK_PAYROLL_COMPLIANCE,
    DESCRIPTION: 'Upload payslip CSV to verify Fair Work compliance',
    INITIAL_MESSAGE: 'I want to check payroll compliance',
  },
  CHECK_ROSTER: {
    TITLE: FAIRBOT_LABELS.CHECK_ROSTER_COMPLIANCE,
    DESCRIPTION: 'Upload roster to check for penalty rate issues',
    INITIAL_MESSAGE: 'I want to check roster compliance',
  },
  ASK_QUESTION: {
    TITLE: FAIRBOT_LABELS.ASK_QUESTION,
    DESCRIPTION: 'Get answers about awards, overtime, leave',
    INITIAL_MESSAGE: 'I have a Fair Work question',
  },
  GENERATE_CONTRACT: {
    TITLE: FAIRBOT_LABELS.GENERATE_CONTRACT,
    DESCRIPTION: 'Create a Fair Work compliant contract',
    INITIAL_MESSAGE: 'I want to generate an employment contract',
  },
} as const

export const FAIRBOT_IDS = {
  FILE_INPUT: 'fairbot-file-input',
} as const

export const FAIRBOT_INPUT_TYPES = {
  FILE: 'file',
} as const

export const FAIRBOT_MOCK_DATA = {
  PAYROLL: {
    issuesFound: 2,
    totalRecords: 24,
    topIssues: [
      { id: 'underpayment', description: 'Potential underpayment detected' },
      { id: 'missing-allowance', description: 'Allowance missing for shift' },
    ],
  },
  ROSTER: {
    issuesFound: 1,
    shiftCount: 12,
    topIssues: [
      { id: 'insufficient-breaks', description: 'Insufficient breaks detected' },
    ],
  },
  EMPLOYEE: {
    employeesReviewed: 6,
    issuesFound: 0,
  },
  DOCUMENT: {
    documentsGenerated: 1,
    lastGeneratedAt: '2024-01-12T10:30:00.000Z',
  },
} as const

export const FAIRBOT_QUICK_ACTIONS = {
  COLORS: {
    BLUE: 'blue',
    GREEN: 'green',
    PURPLE: 'purple',
    ORANGE: 'orange',
  },
  ICONS: {
    PAYROLL: 'payroll',
    ROSTER: 'roster',
    QUESTION: 'question',
    DOCUMENT: 'document',
  },
  PERMISSIONS: {
    CHECK_PAYROLL: 'CheckPayrollCompliance',
    CHECK_ROSTER: 'CheckRosterCompliance',
    MANAGE_DOCUMENTS: 'ManageDocuments',
  },
  IDS: {
    CHECK_PAYROLL: 'check-payroll',
    CHECK_ROSTER: 'check-roster',
    ASK_QUESTION: 'ask-question',
    GENERATE_CONTRACT: 'generate-contract',
  },
} as const
