export const FAIRBOT_LABELS = {
  // Page title shown in the chat header.
  TITLE: 'FairBot AI Assistant',
  // Supporting subtitle under the title.
  SUBTITLE:
    'Ask questions, upload roster/ payroll files, and get help with Fair Work compliance',
  // Greeting line in the welcome card.
  WELCOME_TITLE:
    "Hi John! I'm FairBot, your AI-powered Fair Work assistant. I can help you with:",
  // Bullets shown in the welcome card.
  WELCOME_BULLETS: [
    'Roster compliance - Upload and check for penalty rate issues',
    'Payroll verification - Identify underpayment risks',
    'Fair Work questions',
  ],
  // Prompt shown after the welcome bullets.
  PROMPT_QUESTION: 'What would you like help with today?',
  // Placeholder for the message input field.
  INPUT_PLACEHOLDER: 'Type your message or upload a file...',
  // Empty-state title for results panel.
  EMPTY_RESULTS_TITLE: 'Results will appear here',
  // Empty-state helper text for results panel.
  EMPTY_RESULTS_SUBTITLE:
    "Upload a file or ask a question, and I'll display a quick summary with links to detailed reports.",
  // Helper tip under the file upload zone.
  UPLOAD_TIP: 'Tip: Upload roster or payroll files (.xlsx, .csv) for instant analysis',
  // Label for ask question quick action.
  ASK_QUESTION: 'Ask a Question',
  // Typing indicator message.
  LOADING_MESSAGE: 'FairBot is thinking...',
  // Generic error message for chat actions.
  ERROR_GENERIC: 'Something went wrong. Please try again.',
  // Attachment label in message metadata.
  ATTACHMENT_LABEL: 'Attachment',
  // Label for contract generation quick action.
  GENERATE_CONTRACT: 'Generate Employment Contract',
  // Label for payroll compliance quick action.
  CHECK_PAYROLL_COMPLIANCE: 'Check Payroll Compliance',
  // Label for roster compliance quick action.
  CHECK_ROSTER_COMPLIANCE: 'Check Roster Compliance',
  // Chat label for assistant messages.
  ASSISTANT_LABEL: 'FairBot',
  // Chat label for user messages.
  USER_LABEL: 'You',
  // Prefix before message timestamp.
  MESSAGE_TIME_PREFIX: 'Sent',
  // Accessible label for send button.
  SEND_BUTTON_LABEL: 'Send message',
  // Accessible label for attachment button.
  ATTACH_BUTTON_LABEL: 'Attach file',
  // Accessible label for message input.
  MESSAGE_INPUT_LABEL: 'Message',
  // Heading for message list section.
  MESSAGE_LIST_HEADING: 'Conversation',
  // Message text used when a file upload is sent.
  VIEW_DETAILED_REPORT: 'View Detailed Report',
  FILE_UPLOAD_PROMPT: 'Drop a file here or click to upload',
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
  // ARIA label for the main chat area.
  CHAT_AREA: 'FairBot chat area',
  // ARIA label for the results panel.
  RESULTS_PANEL: 'FairBot results panel',
  // ARIA label for the message list container.
  MESSAGE_LIST: 'FairBot conversation',
  // ARIA label for the message input form.
  MESSAGE_INPUT: 'FairBot message input',
  // ARIA label for file upload zone.
  FILE_UPLOAD: 'File upload',
  // ARIA label for quick actions section.
  QUICK_ACTIONS: 'FairBot quick actions',
} as const

export const FAIRBOT_TEXT = {
  // Reusable empty string literal.
  EMPTY: '',
  // CSS grid shorthand for spanning full width.
  FULL_SPAN: '1 / -1',
  // Single-column grid template for responsive layout.
  SINGLE_COLUMN: 'minmax(0, 1fr)',
} as const

export const FAIRBOT_ENV = {
  // Guard token for checking window availability.
  TYPEOF_UNDEFINED: 'undefined',
} as const

export const FAIRBOT_SESSION_KEYS = {
  // Session storage key for saved conversation messages.
  CONVERSATION: 'fairbot_conversation',
  // Session storage key for saved results summary.
  RESULTS: 'fairbot_results',
} as const

export const FAIRBOT_ROLES = {
  // Role label for user messages.
  USER: 'user',
  // Role label for assistant messages.
  ASSISTANT: 'assistant',
} as const

export const FAIRBOT_MESSAGES = {
  // Default assistant reply text.
  ASSISTANT_DEFAULT: 'Thanks! I can help with that.',
  // Assistant reply text when file is received.
  ASSISTANT_FILE_RECEIVED: 'Thanks for the file. I will review it now.',
} as const

export const FAIRBOT_NUMBERS = {
  // Common numeric constant for zero.
  ZERO: 0,
  // Common numeric constant for one.
  ONE: 1,
  // Common numeric constant for two.
  TWO: 2,
  // Common numeric constant for three.
  THREE: 3,
  // Common numeric constant for four.
  FOUR: 4,
  // Common numeric constant for five.
  FIVE: 5,
} as const

export const FAIRBOT_KEYWORDS = {
  // Keyword to detect payroll intent.
  PAYROLL: 'payroll',
  // Keyword to detect roster intent.
  ROSTER: 'roster',
  // Keyword to detect employee intent.
  EMPLOYEE: 'employee',
  // Keyword to detect document intent.
  DOCUMENT: 'document',
  // Keyword to detect contract intent.
  CONTRACT: 'contract',
} as const

const FAIRBOT_GRID_COLUMNS = {
  // Sidebar column width for the FairBot layout.
  SIDEBAR: '20%',
  // Chat column width definition for the grid.
  CHAT: 'minmax(0, 1fr)',
  // Results column width for the FairBot layout.
  RESULTS: '25%',
} as const

const FAIRBOT_GRID_TEMPLATE_COLUMNS =
  `${FAIRBOT_GRID_COLUMNS.SIDEBAR} ${FAIRBOT_GRID_COLUMNS.CHAT} ${FAIRBOT_GRID_COLUMNS.RESULTS}` as const

export const FAIRBOT_LAYOUT = {
  // Uniform margin for chat sections (header, scroll area, composer).
  CHAT_SECTION_MARGIN_PX: 25,
  // Fixed height for the chat header.
  CHAT_HEADER_HEIGHT_PX: 100,
  // Fixed width for the results panel.
  RESULTS_PANEL_WIDTH: 360,
  // Breakpoint for mobile layout adjustments.
  MOBILE_BREAKPOINT: 900,
  // Gap between quick action cards.
  QUICK_ACTIONS_GAP: 12,
  // Number of quick action columns on desktop.
  QUICK_ACTIONS_COLUMNS: 2,
  // Padding inside the results panel.
  RESULTS_PANEL_PADDING: 24,
  // Gap between elements inside results panel.
  RESULTS_PANEL_GAP: 16,
  // Gap between message list items.
  MESSAGE_LIST_GAP: 16,
  // Gap between stacked message elements.
  MESSAGE_STACK_GAP: 8,
  // Gap between message section blocks.
  MESSAGE_SECTION_GAP: 1.5,
  // Padding inside the message list container.
  MESSAGE_LIST_PADDING: 8,
  // Sidebar column width for the FairBot grid layout.
  SIDEBAR_COLUMN_WIDTH: FAIRBOT_GRID_COLUMNS.SIDEBAR,
  // Grid template columns for the FairBot page container.
  GRID_TEMPLATE_COLUMNS: FAIRBOT_GRID_TEMPLATE_COLUMNS,
  // Minimum height for the page-level grid container.
  PAGE_MIN_HEIGHT: '100vh',
  // Flex display value for layout containers.
  DISPLAY_FLEX: 'flex',
  // Column direction for flex layouts.
  FLEX_DIRECTION_COLUMN: 'column',
  // Center alignment value for flex layouts.
  ALIGN_CENTER: 'center',
  // Center justification value for flex layouts.
  JUSTIFY_CENTER: 'center',
  // Border-box sizing for panel width calculations.
  BOX_SIZING_BORDER_BOX: 'border-box',
  // Full-width value for grid children.
  COLUMN_FULL_WIDTH: '100%',
  // Full-height value for stretching columns/panels.
  COLUMN_FULL_HEIGHT: '100%',
  // Grid alignment value for stretching items.
  ALIGN_STRETCH: 'stretch',
} as const

export const FAIRBOT_UPLOAD = {
  // Border radius for upload drop zone.
  BORDER_RADIUS: 16,
  // Border override for upload drop zone.
  BORDER_NONE: 'none',
  // Gap between upload zone elements.
  GAP: 8,
  // Horizontal offset for helper text alignment.
  HELPER_TEXT_OFFSET_PX: 10,
  // Transition duration for upload hover states.
  TRANSITION_MS: 150,
  // Minimum height for upload zone.
  MIN_HEIGHT: 48,
  // Easing for upload zone transitions.
  TRANSITION_EASING: 'ease',
  // CSS properties animated on upload zone.
  TRANSITION_PROPERTIES: 'background-color',
} as const

export const FAIRBOT_QUICK_ACTIONS_UI = {
  // Padding inside quick action cards.
  CARD_PADDING: 12,
  // Border radius for quick action cards.
  CARD_RADIUS: 12,
  // Border width for quick action cards.
  CARD_BORDER_WIDTH: 1,
  // Minimum height for quick action cards.
  CARD_MIN_HEIGHT: 88,
  // Icon size for quick action badges.
  ICON_SIZE: 20,
  // Vertical gap in action card content.
  CONTENT_GAP: 4,
  // Transition duration for hover effects.
  TRANSITION_MS: 150,
  // Hover translation for card lift effect.
  HOVER_TRANSLATE_Y_PX: -1,
  // Palette mapping for quick action colors.
  COLOR_MAP: {
    // Blue action palette mapping.
    blue: {
      // MUI palette key for background color.
      background: 'info',
      // MUI palette key for border color.
      border: 'info',
    },
    // Green action palette mapping.
    green: {
      // MUI palette key for background color.
      background: 'success',
      // MUI palette key for border color.
      border: 'success',
    },
    // Purple action palette mapping.
    purple: {
      // MUI palette key for background color.
      background: 'secondary',
      // MUI palette key for border color.
      border: 'secondary',
    },
    // Orange action palette mapping.
    orange: {
      // MUI palette key for background color.
      background: 'warning',
      // MUI palette key for border color.
      border: 'warning',
    },
  },
} as const

export const FAIRBOT_RESULTS_UI = {
  // Border radius for results cards.
  CARD_RADIUS: 16,
  // Border override for results panel.
  PANEL_BORDER: 'none',
  // Padding inside results cards.
  CARD_PADDING: 16,
  // Radius for empty state icon container.
  EMPTY_ICON_RADIUS: 48,
  // Gap between stats in results cards.
  STAT_GAP: 12,
  // Minimum height for results panel.
  MIN_HEIGHT: 320,
  STATS_GRID_COLUMNS: 2,
  STACK_GAP: 16,
  LIST_GAP: 8,
} as const

export const FAIRBOT_MESSAGE_UI = {
  // Border radius for chat bubbles.
  BUBBLE_RADIUS: 16,
  // Padding inside chat bubbles.
  BUBBLE_PADDING: 12,
  // Max width for chat bubbles.
  BUBBLE_MAX_WIDTH: 520,
  // Border radius for file badge.
  FILE_BADGE_RADIUS: 12,
  // Horizontal padding for file badge.
  FILE_BADGE_PADDING_X: 8,
  // Vertical padding for file badge.
  FILE_BADGE_PADDING_Y: 4,
} as const

export const FAIRBOT_INPUT_UI = {
  // Border radius for input controls.
  BORDER_RADIUS: 16,
  // Gap between input controls.
  GAP: 12,
  // Size for icon buttons.
  BUTTON_SIZE: 44,
  // Border radius for the text field.
  FIELD_RADIUS: 12,
} as const

export const FAIRBOT_TYPING_UI = {
  // Diameter of each typing dot.
  DOT_SIZE: 6,
  // Gap between typing dots.
  DOT_GAP: 4,
  // Animation duration for typing dots.
  DOT_ANIMATION_MS: 1200,
  // Short delay between dots.
  DELAY_SHORT_MS: 120,
  // Long delay between dots.
  DELAY_LONG_MS: 240,
} as const

export const FAIRBOT_TIME_FORMAT = {
  // Hour format for timestamps.
  HOUR: '2-digit',
  // Minute format for timestamps.
  MINUTE: '2-digit',
} as const

export const FAIRBOT_FILE_SIZE = {
  // Bytes per kilobyte threshold.
  KILO_THRESHOLD: 1024,
  // Bytes per megabyte threshold.
  MEGA_THRESHOLD: 1024 * 1024,
  // Decimal places when formatting megabytes.
  MEGA_DECIMALS: 1,
  // Suffix for kilobyte formatting.
  KILO_SUFFIX: 'KB',
  // Suffix for megabyte formatting.
  MEGA_SUFFIX: 'MB',
  // Fallback text for zero size.
  ZERO_KB: '0 KB',
} as const

export const FAIRBOT_TIMING = {
  // Delay before showing typing indicator to reduce flicker.
  TYPING_INDICATOR_DELAY_MS: 200,
} as const

export const FAIRBOT_FILE = {
  // File extensions accepted for uploads.
  ACCEPTED_TYPES: ['csv', 'xlsx'],
  // MIME types accepted for uploads.
  ACCEPTED_MIME: [
    'text/csv',
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  ],
  // Maximum allowed upload size in bytes.
  MAX_SIZE_BYTES: 5 * 1024 * 1024,
  // Human-readable max size label.
  MAX_SIZE_LABEL: '5MB',
  // Accept attribute value for file inputs.
  ACCEPT_ATTRIBUTE: '.csv,.xlsx',
  // Separator used when parsing file extensions.
  NAME_SEPARATOR: '.',
} as const

export const FAIRBOT_ERRORS = {
  // Error shown for unsupported file types.
  INVALID_FILE_TYPE: 'Unsupported file type. Please upload a CSV or XLSX file.',
  // Error shown when file exceeds size limits.
  FILE_TOO_LARGE: `File is too large. Max size is ${FAIRBOT_FILE.MAX_SIZE_LABEL}.`,
  // Error shown when no file is provided.
  FILE_REQUIRED: 'Please select a file to continue.',
} as const

export const FAIRBOT_RESULTS = {
  // Result types supported by FairBot summaries.
  TYPES: {
    // Payroll summary result type.
    PAYROLL: 'payroll',
    // Roster summary result type.
    ROSTER: 'roster',
    // Employee review result type.
    EMPLOYEE: 'employee',
    // Document generation result type.
    DOCUMENT: 'document',
  },
} as const

export const FAIRBOT_ROUTES = {
  // Route for payroll upload workflow.
  PAYROLL: '/payroll/upload',
  // Route for roster compliance upload workflow.
  ROSTER: '/compliance/upload',
  // Route for employee profile overview.
  EMPLOYEE: '/my-profile',
  // Route for generated documents.
  DOCUMENT: '/documents',
} as const

export const FAIRBOT_QUICK_ACTIONS_TEXT = {
  // Copy for payroll compliance quick action.
  CHECK_PAYROLL: {
    // Title shown on the quick action card.
    TITLE: FAIRBOT_LABELS.CHECK_PAYROLL_COMPLIANCE,
    // Description shown on the quick action card.
    DESCRIPTION: 'Upload payslip CSV to verify Fair Work compliance',
    // Initial message sent when action is clicked.
    INITIAL_MESSAGE: 'I want to check payroll compliance',
  },
  // Copy for roster compliance quick action.
  CHECK_ROSTER: {
    // Title shown on the quick action card.
    TITLE: FAIRBOT_LABELS.CHECK_ROSTER_COMPLIANCE,
    // Description shown on the quick action card.
    DESCRIPTION: 'Upload roster to check for penalty rate issues',
    // Initial message sent when action is clicked.
    INITIAL_MESSAGE: 'I want to check roster compliance',
  },
  // Copy for ask question quick action.
  ASK_QUESTION: {
    // Title shown on the quick action card.
    TITLE: FAIRBOT_LABELS.ASK_QUESTION,
    // Description shown on the quick action card.
    DESCRIPTION: 'Get answers about awards, overtime, leave',
    // Initial message sent when action is clicked.
    INITIAL_MESSAGE: 'I have a Fair Work question',
  },
  // Copy for contract generation quick action.
  GENERATE_CONTRACT: {
    // Title shown on the quick action card.
    TITLE: FAIRBOT_LABELS.GENERATE_CONTRACT,
    // Description shown on the quick action card.
    DESCRIPTION: 'Create a Fair Work compliant contract',
    // Initial message sent when action is clicked.
    INITIAL_MESSAGE: 'I want to generate an employment contract',
  },
} as const

export const FAIRBOT_IDS = {
  // DOM id for the hidden file input.
  FILE_INPUT: 'fairbot-file-input',
  // DOM id for the text message input.
  MESSAGE_INPUT: 'fairbot-message-input',
} as const

export const FAIRBOT_INPUT_TYPES = {
  // Input type for file inputs.
  FILE: 'file',
  // Input type for non-submit buttons.
  BUTTON: 'button',
  // Input type for submit buttons.
  SUBMIT: 'submit',
} as const

export const FAIRBOT_MOCK_DATA = {
  // Mock summary data for payroll results.
  PAYROLL: {
    // Number of issues flagged in payroll review.
    issuesFound: 2,
    // Total records reviewed in payroll summary.
    totalRecords: 24,
    // Top payroll issues surfaced to the user.
    topIssues: [
      {
        // Issue identifier.
        id: 'underpayment',
        // Issue description text.
        description: 'Potential underpayment detected',
      },
      {
        // Issue identifier.
        id: 'missing-allowance',
        // Issue description text.
        description: 'Allowance missing for shift',
      },
    ],
  },
  // Mock summary data for roster results.
  ROSTER: {
    // Number of issues flagged in roster review.
    issuesFound: 1,
    // Total shifts reviewed in roster summary.
    shiftCount: 12,
    // Top roster issues surfaced to the user.
    topIssues: [
      {
        // Issue identifier.
        id: 'insufficient-breaks',
        // Issue description text.
        description: 'Insufficient breaks detected',
      },
    ],
  },
  // Mock summary data for employee results.
  EMPLOYEE: {
    // Number of employees reviewed.
    employeesReviewed: 6,
    // Number of issues found in employee review.
    issuesFound: 0,
  },
  // Mock summary data for document results.
  DOCUMENT: {
    // Number of documents generated.
    documentsGenerated: 1,
    // Timestamp for the last generated document.
    lastGeneratedAt: '2024-01-12T10:30:00.000Z',
  },
} as const

export const FAIRBOT_QUICK_ACTIONS = {
  // Color identifiers for quick action cards.
  COLORS: {
    // Blue action color.
    BLUE: 'blue',
    // Green action color.
    GREEN: 'green',
    // Purple action color.
    PURPLE: 'purple',
    // Orange action color.
    ORANGE: 'orange',
  },
  // Icon identifiers for quick action cards.
  ICONS: {
    // Icon for payroll action.
    PAYROLL: 'payroll',
    // Icon for roster action.
    ROSTER: 'roster',
    // Icon for question action.
    QUESTION: 'question',
    // Icon for document action.
    DOCUMENT: 'document',
  },
  // Permission tokens required for actions.
  PERMISSIONS: {
    // Permission for payroll compliance action.
    CHECK_PAYROLL: 'CheckPayrollCompliance',
    // Permission for roster compliance action.
    CHECK_ROSTER: 'CheckRosterCompliance',
    // Permission for contract/document actions.
    MANAGE_DOCUMENTS: 'ManageDocuments',
  },
  // Stable identifiers for quick action config.
  IDS: {
    // Id for payroll action.
    CHECK_PAYROLL: 'check-payroll',
    // Id for roster action.
    CHECK_ROSTER: 'check-roster',
    // Id for ask question action.
    ASK_QUESTION: 'ask-question',
    // Id for contract generation action.
    GENERATE_CONTRACT: 'generate-contract',
  },
} as const
