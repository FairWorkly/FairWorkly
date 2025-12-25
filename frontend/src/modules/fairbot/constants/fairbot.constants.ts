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
  // Label for roster quick action.
  CHECK_ROSTER: 'Check Roster',
  // Label for payroll quick action.
  VERIFY_PAYROLL: 'Verify Payroll',
  // Label for ask question quick action.
  ASK_QUESTION: 'Ask a Question',
  // Call-to-action label for detailed report links.
  VIEW_DETAILED_REPORT: 'View Detailed Report',
  // Typing indicator message.
  LOADING_MESSAGE: 'FairBot is thinking...',
  // Generic error message for chat actions.
  ERROR_GENERIC: 'Something went wrong. Please try again.',
  // Attachment label in message metadata.
  ATTACHMENT_LABEL: 'Attachment',
  // Section header for quick actions.
  QUICK_ACTIONS_HEADER: 'Quick Actions',
  // Label for contract generation quick action.
  GENERATE_CONTRACT: 'Generate Employment Contract',
  // Label for payroll compliance quick action.
  CHECK_PAYROLL_COMPLIANCE: 'Check Payroll Compliance',
  // Label for roster compliance quick action.
  CHECK_ROSTER_COMPLIANCE: 'Check Roster Compliance',
  // Title for results panel.
  RESULTS_PANEL_TITLE: 'Results Summary',
  // Subtitle for results panel.
  RESULTS_PANEL_SUBTITLE: 'Summary based on your latest request.',
  // Title for payroll summary card.
  PAYROLL_SUMMARY_TITLE: 'Payroll Check Complete',
  // Title for roster summary card.
  ROSTER_SUMMARY_TITLE: 'Roster Check Complete',
  // Title for employee summary placeholder.
  EMPLOYEE_SUMMARY_TITLE: 'Employee Review Complete',
  // Title for document summary placeholder.
  DOCUMENT_SUMMARY_TITLE: 'Document Generation Complete',
  // Label for total records stat.
  TOTAL_RECORDS_LABEL: 'Total Records',
  // Label for shift count stat.
  SHIFT_COUNT_LABEL: 'Shifts Reviewed',
  // Label for employees reviewed stat.
  EMPLOYEES_REVIEWED_LABEL: 'Employees Reviewed',
  // Label for documents generated stat.
  DOCUMENTS_GENERATED_LABEL: 'Documents Generated',
  // Label for issues found stat.
  ISSUES_FOUND_LABEL: 'Issues Found',
  // Label for top issues list.
  TOP_ISSUES_LABEL: 'Top Issues',
  // Success message when payroll has no issues.
  NO_PAYROLL_ISSUES: 'No payroll issues detected.',
  // Success message when roster has no issues.
  NO_ROSTER_ISSUES: 'No roster issues detected.',
  // Placeholder summary for employee result type.
  EMPLOYEE_ISSUES_SUMMARY: 'Employee review summary',
  // Placeholder summary for document result type.
  DOCUMENTS_SUMMARY: 'Document generation summary',
  // Tooltip label for view results action.
  VIEW_RESULTS_TITLE: 'View results',
  // Helper hint below results panel.
  RESULTS_PANEL_HINT: 'Open detailed reports to review findings.',
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
  // Prompt text inside upload zone.
  FILE_UPLOAD_PROMPT: 'Drop a file here or click to upload',
  // Hint when user cannot submit.
  SUBMIT_DISABLED_HINT: 'Please enter a message or attach a file.',
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

export const FAIRBOT_LAYOUT = {
  // Max width of the chat column content.
  CHAT_MAX_WIDTH: 760,
  // Fixed width for the results panel.
  RESULTS_PANEL_WIDTH: 360,
  // Reserved width for sidebar layouts.
  SIDEBAR_WIDTH: 220,
  // Base gap used between layout sections.
  CONTENT_GAP: 24,
  // Max height for the scrollable message list.
  CHAT_SCROLL_HEIGHT: 540,
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
  MESSAGE_SECTION_GAP: 12,
  // Padding inside the message list container.
  MESSAGE_LIST_PADDING: 8,
} as const

export const FAIRBOT_UPLOAD = {
  // Border radius for upload drop zone.
  BORDER_RADIUS: 16,
  // Border width for upload drop zone.
  BORDER_WIDTH: 1,
  // Padding inside the upload zone.
  PADDING: 12,
  // Gap between upload zone elements.
  GAP: 8,
  // Transition duration for upload hover states.
  TRANSITION_MS: 150,
  // Minimum height for upload zone.
  MIN_HEIGHT: 48,
  // Border style for upload zone.
  BORDER_STYLE: 'dashed',
  // Easing for upload zone transitions.
  TRANSITION_EASING: 'ease',
  // CSS properties animated on upload zone.
  TRANSITION_PROPERTIES: 'border-color, background-color',
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
  // Column span for full-width cards.
  FULL_SPAN_COLUMNS: 2,
} as const

export const FAIRBOT_RESULTS_UI = {
  // Icon size for results header.
  HEADER_ICON_SIZE: 56,
  // Border radius for results cards.
  CARD_RADIUS: 16,
  // Padding inside results cards.
  CARD_PADDING: 16,
  // Radius for empty state icon container.
  EMPTY_ICON_RADIUS: 48,
  // Icon size inside empty state circle.
  EMPTY_ICON_SIZE: 32,
  // Gap between stats in results cards.
  STAT_GAP: 12,
  // Gap between list items in summaries.
  LIST_GAP: 8,
  // Background token for empty icon container.
  EMPTY_ICON_BG: 'action.hover',
  // Column count for stats grid.
  STATS_GRID_COLUMNS: 2,
  // Gap between stacked summary sections.
  STACK_GAP: 16,
  // Minimum height for results panel.
  MIN_HEIGHT: 320,
  // Border width for results cards.
  CARD_BORDER_WIDTH: 1,
  // Gap between header title and subtitle.
  HEADER_GAP: 4,
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
  // Delay before showing typing indicator.
  TYPING_INDICATOR_DELAY_MS: 200,
  // Minimum time to keep typing indicator visible.
  TYPING_INDICATOR_MIN_MS: 800,
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
