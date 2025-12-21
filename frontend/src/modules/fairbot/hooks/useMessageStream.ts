import { useEffect, useState } from 'react'
import { FAIRBOT_TIMING } from '../constants/fairbot.constants'

interface UseMessageStreamResult {
  isTyping: boolean
}

export const useMessageStream = (isLoading: boolean): UseMessageStreamResult => {
  const [isTyping, setIsTyping] = useState(false)

  useEffect(() => {
    if (!isLoading) {
      setIsTyping(false)
      return
    }

    const startDelay = window.setTimeout(() => {
      setIsTyping(true)
    }, FAIRBOT_TIMING.TYPING_INDICATOR_DELAY_MS)

    const minDisplay = window.setTimeout(() => {
      if (!isLoading) {
        setIsTyping(false)
      }
    }, FAIRBOT_TIMING.TYPING_INDICATOR_MIN_MS)

    return () => {
      window.clearTimeout(startDelay)
      window.clearTimeout(minDisplay)
    }
  }, [isLoading])

  return { isTyping }
}
