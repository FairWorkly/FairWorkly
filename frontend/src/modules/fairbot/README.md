# FairBot Module

## Purpose

FairBot provides a chat-style assistant UI for Fair Work compliance help. It lets users send messages, upload roster/payroll files, and view a quick summary of results in a side panel.

## Architecture Pattern: Thin Module with Shared Core

This module follows the "Thin Module with Shared Core" pattern:

- **Page components** (`pages/`) act as **orchestrators** - they manage state, navigation, and wire callbacks to shared components
- **UI components** live in `@/shared/chat` - generic, reusable chat primitives
- **FairBot-specific configuration** stays in this module (labels, mock data, result types)

For the shared chat components, see `src/shared/chat/README.md`.

## File Structure

```
src/modules/fairbot/
  constants/
    fairbot.constants.ts     # FairBot-specific labels, layout, mock data
  features/
    resultsPanel/
      PayrollSummary.tsx     # Payroll result card
      QuickSummary.tsx       # Result type router
      ResultsEmpty.tsx       # Empty state
      ResultsPanel.tsx       # Panel container
      RosterSummary.tsx      # Roster result card
  hooks/
    useConversation.ts       # View-model combining useFairBot + typing indicator
    useFairBot.ts            # Core state: messages, mock responses, persistence
    useResultsPanel.ts       # Results summary state + session persistence
    usePermissions.ts        # Permission checks (dev mode)
  pages/
    FairBotChat.tsx          # Orchestrator page (wires shared components)
  types/
    fairbot.types.ts         # FairBot-specific types (results, quick actions)
  ui/
    WelcomeMessage.tsx       # FairBot-specific welcome card
```

## Shared Components Used

From `@/shared/chat`:

| Component | Purpose |
|-----------|---------|
| `MessageList` | Renders chat messages with typing indicator |
| `MessageInput` | Text input with send/attach buttons |
| `MessageBubble` | Individual message display |
| `TypingIndicator` | Animated typing dots |
| `FileUploadZone` | Drag/drop file upload area |
| `useFileUpload` | File validation and upload state |
| `useMessageStream` | Typing indicator timing |

## Data Flow

```
FairBotChat (orchestrator page)
    ├── useConversation (view-model)
    │       └── useFairBot (state + mock responses)
    │               └── useResultsPanel (results summary)
    └── useFileUpload (from @/shared/chat)
            └── calls conversation.sendMessage on file accept
```

## Configuration via Props

The page passes FairBot-specific configuration to shared components:

```typescript
// Labels for message display
const CHAT_LABELS = {
  userLabel: FAIRBOT_LABELS.USER_LABEL,
  assistantLabel: FAIRBOT_LABELS.ASSISTANT_LABEL,
  loadingMessage: FAIRBOT_LABELS.LOADING_MESSAGE,
  // ...
}

// Usage in orchestrator
<MessageList
  messages={conversation.messages}
  isTyping={conversation.isTyping}
  labels={CHAT_LABELS}
/>
```

## Entry Point and Routing

- Route: `src/app/routes/fairbot.routes.tsx` maps `/fairbot` to the page
- Page component: `src/modules/fairbot/pages/FairBotChat.tsx`
- Access control: Wrap with `ProtectedRoute` when auth is wired

## Extending the Module

### Add a New Result Type

1. Add interface to `types/fairbot.types.ts` and extend `FairBotResult` union
2. Add to `FAIRBOT_RESULTS.TYPES` and `FAIRBOT_MOCK_DATA` in constants
3. Add rendering branch in `features/resultsPanel/QuickSummary.tsx`
4. Add route to `FAIRBOT_ROUTES`

### Connect Real Agent Service

Replace in `hooks/useFairBot.ts`:

```typescript
// Current mock:
const response = buildMockResponse(trimmedText, file)

// Replace with:
const response = await agentApi.chat({ text: trimmedText, file })
```

## Testing Notes

- No tests exist yet
- Add colocated `*.test.tsx` for key flows:
  - Message sending, file validation, summary rendering

## Current Limitations

- Responses and summaries use mock data
- Permission checks use dev-mode defaults (`window.switchRole()`)
- Files are not persisted to session storage
