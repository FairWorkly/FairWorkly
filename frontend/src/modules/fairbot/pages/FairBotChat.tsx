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
  alignItems: 'start',
  minHeight: FAIRBOT_LAYOUT.PAGE_MIN_HEIGHT,
  width: FAIRBOT_LAYOUT.COLUMN_FULL_WIDTH,
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    gridTemplateColumns: FAIRBOT_TEXT.SINGLE_COLUMN,
  },
})

// Left column that hosts the shared sidebar navigation.
const SidebarColumn = styled('aside')({
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    order: FAIRBOT_NUMBERS.ZERO,
  },
})

// Left column that holds the conversational UI stack.
const ChatColumn = styled('section')({
  display: FAIRBOT_LAYOUT.DISPLAY_FLEX,
  flexDirection: FAIRBOT_LAYOUT.FLEX_DIRECTION_COLUMN,
  alignSelf: FAIRBOT_LAYOUT.ALIGN_STRETCH,
  height: FAIRBOT_LAYOUT.PAGE_MIN_HEIGHT,
  minHeight: FAIRBOT_NUMBERS.ZERO,
})

// Page header for title/subtitle within the chat column.
const ChatHeader = styled('header')({
  display: 'flex',
  flexDirection: 'column',
  height: `${FAIRBOT_LAYOUT.CHAT_HEADER_HEIGHT_PX}px`,
  margin: `${FAIRBOT_LAYOUT.CHAT_SECTION_MARGIN_PX}px`,
  gap: `${FAIRBOT_LAYOUT.MESSAGE_STACK_GAP}px`,
})

// Scrollable wrapper around message list to keep input/footer in view.
const ScrollArea = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_LIST_GAP}px`,
  flex: FAIRBOT_NUMBERS.ONE,
  minHeight: FAIRBOT_NUMBERS.ZERO,
  overflowY: 'auto',
  margin: `${FAIRBOT_LAYOUT.CHAT_SECTION_MARGIN_PX}px`,
})

// Right column that hosts the summary panel (stacks below on mobile).
const ResultsColumn = styled('aside')(({ theme }) => ({
  backgroundColor: theme.palette.background.paper,
  alignSelf: FAIRBOT_LAYOUT.ALIGN_STRETCH,
  height: FAIRBOT_LAYOUT.COLUMN_FULL_HEIGHT,
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    order: FAIRBOT_NUMBERS.TWO,
  },
}))

const ResultsPanelWrapper = styled('div')({
  display: FAIRBOT_LAYOUT.DISPLAY_FLEX,
  flexDirection: FAIRBOT_LAYOUT.FLEX_DIRECTION_COLUMN,
  alignItems: FAIRBOT_LAYOUT.ALIGN_CENTER,
  justifyContent: FAIRBOT_LAYOUT.JUSTIFY_CENTER,
  height: FAIRBOT_LAYOUT.COLUMN_FULL_HEIGHT,
  width: FAIRBOT_LAYOUT.COLUMN_FULL_WIDTH,
})

const MessageComposer = styled(Stack)({
  margin: `${FAIRBOT_LAYOUT.CHAT_SECTION_MARGIN_PX}px`,
})

const MessageInputWrapper = styled('div')({
  width: FAIRBOT_LAYOUT.COLUMN_FULL_WIDTH,
  '& > *': {
    width: FAIRBOT_LAYOUT.COLUMN_FULL_WIDTH,
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
        <Divider />
        <ScrollArea>
          <WelcomeMessage />
          <QuickActions
            upload={upload}
            onSendMessage={conversation.sendMessage}
          />
          <MessageList
            messages={conversation.messages}
            isTyping={conversation.isTyping}
          />
        </ScrollArea>
        <Divider />
        <MessageComposer spacing={FAIRBOT_LAYOUT.MESSAGE_SECTION_GAP}>
          <FileUploadZone
            upload={upload}
            inputRef={inputRef}
            helperText={FAIRBOT_LABELS.UPLOAD_TIP}
          >
            <MessageInputWrapper>
              <MessageInput upload={upload} onSendMessage={conversation.sendMessage} />
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
    </PageContainer >
  )
}
