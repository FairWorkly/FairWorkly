import { useState, useCallback } from 'react'

type Severity = 'success' | 'error' | 'warning' | 'info'

interface Notification {
  message: string
  severity: Severity
}

export function useNotification() {
  const [notification, setNotification] = useState<Notification | null>(null)

  const notify = useCallback(
    (message: string, severity: Severity = 'success') => {
      setNotification({ message, severity })
    },
    []
  )

  const clear = useCallback(() => {
    setNotification(null)
  }, [])

  return { notification, notify, clear }
}
