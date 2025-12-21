import { styled } from '@mui/material/styles'
import Typography from '@mui/material/Typography'
import Stack from '@mui/material/Stack'
import Divider from '@mui/material/Divider'
import {
  FAIRBOT_ARIA,
  FAIRBOT_LABELS,
  FAIRBOT_LAYOUT,
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

const PageContainer = styled('div')({
  display: 'grid',
  gridTemplateColumns: `minmax(0, 1fr) ${FAIRBOT_LAYOUT.RESULTS_PANEL_WIDTH}px`,
  gap: `${FAIRBOT_LAYOUT.CONTENT_GAP}px`,
  alignItems: 'start',
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    gridTemplateColumns: FAIRBOT_TEXT.SINGLE_COLUMN,
  },
})

const ChatColumn = styled('section')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.CONTENT_GAP}px`,
})

const ChatHeader = styled('header')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_STACK_GAP}px`,
})

const ScrollArea = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  gap: `${FAIRBOT_LAYOUT.MESSAGE_LIST_GAP}px`,
  maxHeight: `${FAIRBOT_LAYOUT.CHAT_SCROLL_HEIGHT}px`,
  overflowY: 'auto',
  paddingRight: `${FAIRBOT_LAYOUT.MESSAGE_SECTION_GAP}px`,
})

const ResultsColumn = styled('aside')({
  [`@media (max-width: ${FAIRBOT_LAYOUT.MOBILE_BREAKPOINT}px)`]: {
    order: 2,
  },
})

export const FairBotChat = () => {
  const conversation = useConversation()
  const upload = useFileUpload({
    onFileAccepted: async (file) => {
      await conversation.sendMessage(FAIRBOT_LABELS.FILE_UPLOAD_PROMPT, file)
    },
  })

  return (
    <PageContainer>
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
          <FileUploadZone upload={upload} helperText={FAIRBOT_LABELS.UPLOAD_TIP}>
            <Typography variant="body2">{FAIRBOT_LABELS.FILE_UPLOAD_PROMPT}</Typography>
          </FileUploadZone>
          <Divider />
          <MessageInput upload={upload} onSendMessage={conversation.sendMessage} />
        </Stack>
      </ChatColumn>
      <ResultsColumn>
        <ResultsPanel />
      </ResultsColumn>
    </PageContainer>
  )
}
