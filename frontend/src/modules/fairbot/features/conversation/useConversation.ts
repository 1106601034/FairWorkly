import { useMemo } from 'react'
import { useFairBot } from '../../hooks/useFairBot'
import { useMessageStream } from '../../hooks/useMessageStream'
import type { FairBotMessage } from '../../types/fairbot.types'

interface UseConversationResult {
  messages: FairBotMessage[]
  isLoading: boolean
  isTyping: boolean
  errorMessage: string | null
  hasMessages: boolean
  sendMessage: (text: string, file?: File) => Promise<void>
}

export const useConversation = (): UseConversationResult => {
  const { messages, sendMessage, isLoading, error } = useFairBot()
  const { isTyping } = useMessageStream(isLoading)

  return useMemo(
    () => ({
      messages,
      isLoading,
      isTyping,
      errorMessage: error?.message ?? null,
      hasMessages: messages.length > 0,
      sendMessage,
    }),
    [error?.message, isLoading, isTyping, messages, sendMessage],
  )
}
