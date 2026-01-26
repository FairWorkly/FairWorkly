# Shared Chat Module

## Purpose

The `src/shared/chat` module provides reusable chat UI components, hooks, types, and utilities that can be used by any chat-like feature in the application.

This module follows the "Shared Core" pattern - providing generic, configurable components that feature modules can use while passing their own configuration and labels.

## Structure

```
src/shared/chat/
├── components/
│   ├── MessageBubble.tsx      # Single message display with sender/time
│   ├── MessageList.tsx        # List of messages with typing indicator
│   ├── MessageInput.tsx       # Text input with send/attach buttons
│   ├── TypingIndicator.tsx    # Animated typing dots
│   └── FileUploadZone.tsx     # Drag/drop file upload area
├── hooks/
│   ├── useMessageStream.ts    # Typing indicator timing logic
│   └── useFileUpload.ts       # File validation and upload state
├── types/
│   └── chat.types.ts          # Chat-related TypeScript interfaces
├── constants/
│   └── chat.constants.ts      # Default labels, UI constants
├── utils/
│   └── formatters.ts          # Timestamp and file size formatters
└── index.ts                   # Barrel export
```

## Usage

### Basic Import

```typescript
import {
  MessageList,
  MessageInput,
  FileUploadZone,
  useFileUpload,
  useMessageStream,
  type ChatMessage,
} from '@/shared/chat'
```

### Example: Building a Chat Page

```typescript
const MyChatPage = () => {
  const [messages, setMessages] = useState<ChatMessage[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const { isTyping } = useMessageStream(isLoading)

  const { inputRef, controls } = useFileUpload({
    onFileAccepted: async (file) => {
      await sendMessage('File uploaded', file)
    },
  })

  const sendMessage = async (text: string) => {
    // Your message sending logic
  }

  // Custom labels for your feature
  const labels = {
    userLabel: 'Customer',
    assistantLabel: 'Support Bot',
    loadingMessage: 'Thinking...',
  }

  return (
    <div>
      <MessageList
        messages={messages}
        isTyping={isTyping}
        labels={labels}
      />
      <FileUploadZone
        controls={controls}
        inputRef={inputRef}
        helperText="Drop files here"
      >
        <MessageInput
          controls={{ openFileDialog: controls.openFileDialog }}
          onSendMessage={sendMessage}
          labels={labels}
        />
      </FileUploadZone>
    </div>
  )
}
```

## Components

### MessageList

Renders a list of chat messages with a typing indicator.

```typescript
interface MessageListProps {
  messages: ChatMessage[]
  isTyping: boolean
  labels?: MessageListLabels
}
```

### MessageBubble

Renders a single message with sender label, timestamp, and optional file attachment.

```typescript
interface MessageBubbleProps {
  message: ChatMessage
  labels?: MessageBubbleLabels
}
```

### MessageInput

Text input with send and attach buttons.

```typescript
interface MessageInputProps {
  onSendMessage: (message: string) => Promise<void>
  controls: MessageInputControls
  disabled?: boolean
  labels?: MessageInputLabels
}
```

### TypingIndicator

Animated dots shown when the assistant is "typing".

```typescript
interface TypingIndicatorProps {
  isVisible: boolean
  loadingMessage?: string
}
```

### FileUploadZone

Drag-and-drop file upload area.

```typescript
interface FileUploadZoneProps {
  controls: FileUploadControls
  inputRef: RefObject<HTMLInputElement | null>
  children: ReactNode
  helperText?: string
  disabled?: boolean
}
```

## Hooks

### useMessageStream

Controls typing indicator timing to prevent flicker.

```typescript
const { isTyping } = useMessageStream(isLoading)
```

### useFileUpload

Manages file validation, drag/drop state, and file selection.

```typescript
const { inputRef, controls } = useFileUpload({
  onFileAccepted: (file) => handleFile(file),
  fileConfig: {
    acceptedTypes: ['pdf', 'doc'],
    maxSizeBytes: 10 * 1024 * 1024,
  },
  errorMessages: {
    invalidFileType: 'Only PDF and DOC files allowed',
  },
})
```

## Types

### ChatMessage

```typescript
interface ChatMessage {
  id: string
  role: 'user' | 'assistant'
  text: string
  timestamp: string
  file?: File
  fileMeta?: ChatFileMeta
}
```

### ChatError

```typescript
interface ChatError {
  message: string
  code?: string
}
```

## Customization

All components accept `labels` props for customization:

```typescript
const customLabels = {
  userLabel: 'You',
  assistantLabel: 'FairBot',
  messageTimePrefix: 'Sent',
  inputPlaceholder: 'Type a message...',
  loadingMessage: 'FairBot is thinking...',
}
```

## Modules Using This

- `modules/fairbot/` - FairBot conversational AI interface

## Adding New Chat Features

When building a new chat-like feature:

1. Import components from `@/shared/chat`
2. Create your own hook for conversation state (like `useFairBot`)
3. Pass your custom labels and configuration to components
4. Keep feature-specific logic in your module
