import { useEffect, useState } from 'react'
import { CHAT_NUMBERS, CHAT_TIMING } from '../constants/chat.constants'

interface UseMessageStreamResult {
  isTyping: boolean
}

// Controls typing indicator with delay to prevent flicker on fast responses.
export const useMessageStream = (isLoading: boolean): UseMessageStreamResult => {
  const [isTyping, setIsTyping] = useState(false)

  useEffect(() => {
    if (!isLoading) {
      const stopDelay = window.setTimeout(() => {
        setIsTyping(false)
      }, CHAT_NUMBERS.ZERO)

      return () => {
        window.clearTimeout(stopDelay)
      }
    }

    // Delay indicator start to avoid flicker on fast responses.
    const startDelay = window.setTimeout(() => {
      setIsTyping(true)
    }, CHAT_TIMING.TYPING_INDICATOR_DELAY_MS)

    return () => {
      window.clearTimeout(startDelay)
    }
  }, [isLoading])

  return { isTyping }
}
