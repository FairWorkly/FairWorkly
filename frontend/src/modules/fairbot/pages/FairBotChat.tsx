import { styled } from '@/styles/styled'
import Typography from '@mui/material/Typography'
import Stack from '@mui/material/Stack'
import Divider from '@mui/material/Divider'
import {
  FAIRBOT_ARIA,
  FAIRBOT_LABELS,
  FAIRBOT_LAYOUT,
} from '../constants/fairbot.constants'
import { useConversation } from '../features/conversation/useConversation'
import { useFileUpload } from '../hooks/useFileUpload'
import { MessageList } from '../features/conversation/MessageList'
import { FileUploadZone } from '../features/conversation/FileUploadZone'
import { MessageInput } from '../features/conversation/MessageInput'
import { ResultsPanel } from '../features/resultsPanel/ResultsPanel'

// FairBot chat page wires conversation and upload flows into a two-column layout.
// Sidebar is provided by MainLayout, so this page only has Chat + Results columns.
const PageContainer = styled('div')({
  display: 'grid',
  gridTemplateColumns: FAIRBOT_LAYOUT.GRID_TEMPLATE_COLUMNS,
  height: '100%',
  width: '100%',
  overflow: 'hidden',
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    gridTemplateColumns: '1fr',
    height: 'auto',
    overflow: 'auto',
  },
})

// Main column that holds the conversational UI stack.
const ChatColumn = styled('section')({
  display: 'flex',
  flexDirection: 'column',
  height: '100%',
  overflow: 'hidden',
})

// Page header for title/subtitle within the chat column.
const ChatHeader = styled('header')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  height: `${FAIRBOT_LAYOUT.CHAT_HEADER_HEIGHT_PX}px`,
  margin: theme.spacing(2),
  gap: theme.spacing(1),
}))

// Scrollable wrapper around message list to keep input/footer in view.
const ScrollArea = styled('div')(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(2),
  flex: 1,
  minHeight: 0,
  overflowY: 'auto',
  margin: theme.spacing(2),
}))

// Right column that hosts the summary panel (stacks below on mobile).
const ResultsColumn = styled('aside')(({ theme }) => ({
  backgroundColor: theme.palette.background.paper,
  height: '100%',
  overflow: 'auto',
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    order: 2,
  },
}))

const ResultsPanelWrapper = styled('div')({
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

const MessageInputWrapper = styled('div')({
  width: '100%',
  '& > *': {
    width: '100%',
  },
})

export const FairBotChat = () => {
  const conversation = useConversation()
  // Treat file uploads as messages to keep the chat flow consistent.
  const { inputRef, controls: upload } = useFileUpload({
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
          />
        </ScrollArea>
        <Divider />
        <MessageComposer spacing={1.5}>
          <FileUploadZone
            upload={upload}
            inputRef={inputRef}
            helperText={FAIRBOT_LABELS.UPLOAD_TIP}
          >
            <MessageInputWrapper>
              <MessageInput
                upload={upload}
                onSendMessage={conversation.sendMessage}
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
