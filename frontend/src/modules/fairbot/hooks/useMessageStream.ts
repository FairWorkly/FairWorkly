import { useEffect, useState } from 'react'

interface UseMessageStreamResult {
  isTyping: boolean
}

export const useMessageStream = (
  isLoading: boolean
): UseMessageStreamResult => {
  const [isTyping, setIsTyping] = useState(false)

  useEffect(() => {
    if (!isLoading) {
      const stopDelay = window.setTimeout(() => {
        setIsTyping(false)
      }, 0)

      return () => {
        window.clearTimeout(stopDelay)
      }
    }

    // Delay indicator start to avoid flicker on fast responses.
    const startDelay = window.setTimeout(() => {
      setIsTyping(true)
    }, 200)

    return () => {
      window.clearTimeout(startDelay)
    }
  }, [isLoading])

  return { isTyping }
}
