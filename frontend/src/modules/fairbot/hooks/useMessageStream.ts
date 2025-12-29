import { useEffect, useState } from 'react'
import { FAIRBOT_NUMBERS, FAIRBOT_TIMING } from '../constants/fairbot.constants'

interface UseMessageStreamResult {
  isTyping: boolean
}

export const useMessageStream = (isLoading: boolean): UseMessageStreamResult => {
  const [isTyping, setIsTyping] = useState(false)

  useEffect(() => {
    if (!isLoading) {
      const stopDelay = window.setTimeout(() => {
        setIsTyping(false)
      }, FAIRBOT_NUMBERS.ZERO)

      return () => {
        window.clearTimeout(stopDelay)
      }
    }

    // Delay indicator start to avoid flicker on fast responses.
    const startDelay = window.setTimeout(() => {
      setIsTyping(true)
    }, FAIRBOT_TIMING.TYPING_INDICATOR_DELAY_MS)

    return () => {
      window.clearTimeout(startDelay)
    }
  }, [isLoading])

  return { isTyping }
}
