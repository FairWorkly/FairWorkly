import { styled } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import Stack from '@mui/material/Stack'
import Divider from '@mui/material/Divider'
import {
  FAIRBOT_ARIA,
  FAIRBOT_LABELS,
  FAIRBOT_LAYOUT,
  FAIRBOT_NUMBERS,
  FAIRBOT_TEXT,
} from '../constants/fairbot.constants'
import { useConversation } from '../features/conversation/useConversation'
import { useFileUpload } from '../hooks/useFileUpload'
import { WelcomeMessage } from '../ui/WelcomeMessage'
import { QuickActions } from '../features/quickActions/QuickActions'
import { MessageList } from '../features/conversation/MessageList'
import { FileUploadZone } from '../features/conversation/FileUploadZone'
import { MessageInput } from '../features/conversation/MessageInput'
import { ResultsPanel } from '../features/resultsPanel/ResultsPanel'
import { Sidebar } from '@/shared/components/layout/Sidebar'

// FairBot chat page wires conversation and upload flows into a three-column layout.
const PageContainer = styled('div')({
  display: 'grid',
  gridTemplateColumns: FAIRBOT_LAYOUT.GRID_TEMPLATE_COLUMNS,
  gap: `${FAIRBOT_LAYOUT.CONTENT_GAP}px`,
  alignItems: 'start',
  minHeight: FAIRBOT_LAYOUT.PAGE_MIN_HEIGHT,
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    gridTemplateColumns: FAIRBOT_TEXT.SINGLE_COLUMN,
  },
})

// Left column that hosts the shared sidebar navigation.
const SidebarColumn = styled('aside')({
  width: FAIRBOT_LAYOUT.SIDEBAR_COLUMN_WIDTH,
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    order: FAIRBOT_NUMBERS.ZERO,
  },
})

// Left column that holds the conversational UI stack.
const ChatColumn = styled('section')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.CONTENT_GAP}px`,
  width: FAIRBOT_LAYOUT.COLUMN_FULL_WIDTH,
})

// Page header for title/subtitle within the chat column.
const ChatHeader = styled('header')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_STACK_GAP}px`,
})

// Scrollable wrapper around message list to keep input/footer in view.
const ScrollArea = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_LIST_GAP}px`,
  maxHeight: `${FAIRBOT_LAYOUT.CHAT_SCROLL_HEIGHT}px`,
  overflowY: 'auto',
  paddingRight: `${FAIRBOT_LAYOUT.MESSAGE_SECTION_GAP}px`,
})

// Right column that hosts the summary panel (stacks below on mobile).
const ResultsColumn = styled('aside')({
  width: FAIRBOT_LAYOUT.RESULTS_COLUMN_WIDTH,
  alignSelf: FAIRBOT_LAYOUT.ALIGN_STRETCH,
  height: FAIRBOT_LAYOUT.COLUMN_FULL_HEIGHT,
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    order: FAIRBOT_NUMBERS.TWO,
  },
})

export const FairBotChat = () => {
  const conversation = useConversation()
  // Treat file uploads as messages to keep the chat flow consistent.
  const { inputRef, controls: upload } = useFileUpload({
    onFileAccepted: async (file) => {
      await conversation.sendMessage(FAIRBOT_LABELS.FILE_UPLOAD_PROMPT, file)
    },
  })

  return (
    <PageContainer>
      <SidebarColumn>
        <Sidebar width={FAIRBOT_LAYOUT.SIDEBAR_COLUMN_WIDTH} />
      </SidebarColumn>
      {/* Chat column: greeting, quick actions, message list, input. */}
      <ChatColumn aria-label={FAIRBOT_ARIA.CHAT_AREA}>
        <ChatHeader>
          <Typography variant="h5">{FAIRBOT_LABELS.TITLE}</Typography>
          <Typography variant="body2" color="text.secondary">
            {FAIRBOT_LABELS.SUBTITLE}
          </Typography>
        </ChatHeader>
        <WelcomeMessage />
        <QuickActions
          upload={upload}
          onSendMessage={conversation.sendMessage}
        />
        <ScrollArea>
          <MessageList
            messages={conversation.messages}
            isTyping={conversation.isTyping}
          />
        </ScrollArea>
        <Stack spacing={FAIRBOT_LAYOUT.MESSAGE_SECTION_GAP}>
          <Divider />
          <FileUploadZone
            upload={upload}
            inputRef={inputRef}
            helperText={FAIRBOT_LABELS.UPLOAD_TIP}
          >
            <MessageInput upload={upload} onSendMessage={conversation.sendMessage} />
          </FileUploadZone>
        </Stack>
      </ChatColumn>
      {/* Results column: mirrors the latest summary for faster navigation. */}
      <ResultsColumn>
        <ResultsPanel />
      </ResultsColumn>
    </PageContainer >
  )
}
