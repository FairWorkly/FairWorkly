import { styled } from '@/styles/styled'
import { Box, Typography, Container, Stack, Divider } from '@mui/material'
import {
  MessageList,
  MessageInput,
  FileUploadZone,
  useFileUpload,
} from '@/modules/fairbot/features/conversation'
import {
  FAIRBOT_ARIA,
  FAIRBOT_LABELS,
  FAIRBOT_LAYOUT,
  FAIRBOT_IDS,
} from '../constants/fairbot.constants'
import { useConversation } from '../hooks/useConversation'
import { ResultsPanel } from '../features/resultsPanel/ResultsPanel'

// FairBot-specific labels for chat components.
const CHAT_LABELS = {
  userLabel: FAIRBOT_LABELS.USER_LABEL,
  assistantLabel: FAIRBOT_LABELS.ASSISTANT_LABEL,
  messageTimePrefix: FAIRBOT_LABELS.MESSAGE_TIME_PREFIX,
  attachmentLabel: FAIRBOT_LABELS.ATTACHMENT_LABEL,
  messageListHeading: FAIRBOT_LABELS.MESSAGE_LIST_HEADING,
  loadingMessage: FAIRBOT_LABELS.LOADING_MESSAGE,
}

const INPUT_LABELS = {
  inputPlaceholder: FAIRBOT_LABELS.INPUT_PLACEHOLDER,
  sendButtonLabel: FAIRBOT_LABELS.SEND_BUTTON_LABEL,
  attachButtonLabel: FAIRBOT_LABELS.ATTACH_BUTTON_LABEL,
  messageInputLabel: FAIRBOT_LABELS.MESSAGE_INPUT_LABEL,
}

// FairBot chat page wires conversation and upload flows into a two-column layout.
// Sidebar is provided by MainLayout, so this page only has Chat + Results columns.
const PageContainer = styled(Box)({
  display: 'grid',
  gridTemplateColumns: FAIRBOT_LAYOUT.GRID_TEMPLATE_COLUMNS,
  minHeight: '100vh',
  minWidth: '100vh',
  overflow: 'hidden',
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    gridTemplateColumns: '1fr',
    height: 'auto',
    overflow: 'auto',
  },
})

// Main column that holds the conversational UI stack.
const ChatColumn = styled(Box)({
  display: 'flex',
  flexDirection: 'column',
  height: '100%',
  overflow: 'hidden',
})

// Page header for title/subtitle within the chat column.
const ChatHeader = styled(Container)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  height: `${FAIRBOT_LAYOUT.CHAT_HEADER_HEIGHT_PX}px`,
  margin: theme.spacing(2),
  gap: theme.spacing(1),
}))

// Scrollable wrapper around message list to keep input/footer in view.
const ScrollArea = styled(Container)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2),
  flex: 1,
  minHeight: 0,
  overflowY: 'auto',
  margin: theme.spacing(2),
}))

// Right column that hosts the summary panel (stacks below on mobile).
const ResultsColumn = styled(Box)(({ theme }) => ({
  backgroundColor: theme.palette.background.paper,
  height: '100%',
  overflow: 'auto',
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    order: 2,
  },
}))

const ResultsPanelWrapper = styled(Container)({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  height: '100%',
  width: '100%',
})

const MessageComposer = styled(Stack)(({ theme }) => ({
  margin: theme.spacing(2),
}))

const MessageInputWrapper = styled(Container)({
  width: '100%',
  '& > *': {
    width: '100%',
  },
})

export const FairBotChat = () => {
  const conversation = useConversation()

  // Treat file uploads as messages to keep the chat flow consistent.
  const { inputRef, controls: uploadControls } = useFileUpload({
    onFileAccepted: async file => {
      await conversation.sendMessage(FAIRBOT_LABELS.FILE_UPLOAD_PROMPT, file)
    },
  })

  return (
    <PageContainer>
      {/* Chat column: greeting, quick actions, message list, input. */}
      <ChatColumn aria-label={FAIRBOT_ARIA.CHAT_AREA}>
        <ChatHeader>
          <Typography variant="h5">{FAIRBOT_LABELS.TITLE}</Typography>
          <Typography variant="body2" color="text.secondary">
            {FAIRBOT_LABELS.SUBTITLE}
          </Typography>
        </ChatHeader>
        <Divider />
        <ScrollArea>
          <MessageList
            messages={conversation.messages}
            isTyping={conversation.isTyping}
            labels={CHAT_LABELS}
          />
        </ScrollArea>
        <Divider />
        <MessageComposer spacing={1.5}>
          <FileUploadZone
            controls={uploadControls}
            inputRef={inputRef}
            helperText={FAIRBOT_LABELS.UPLOAD_TIP}
            inputId={FAIRBOT_IDS.FILE_INPUT}
          >
            <MessageInputWrapper>
              <MessageInput
                controls={{ openFileDialog: uploadControls.openFileDialog }}
                onSendMessage={conversation.sendMessage}
                labels={INPUT_LABELS}
                inputId={FAIRBOT_IDS.MESSAGE_INPUT}
              />
            </MessageInputWrapper>
          </FileUploadZone>
        </MessageComposer>
      </ChatColumn>
      {/* Results column: mirrors the latest summary for faster navigation. */}
      <ResultsColumn>
        <ResultsPanelWrapper>
          <ResultsPanel />
        </ResultsPanelWrapper>
      </ResultsColumn>
    </PageContainer>
  )
}
