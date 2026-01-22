import React, { useEffect, useRef } from 'react'
import { Provider } from 'react-redux'
import { store } from '../../store'
import { setupInterceptors } from '../../services/setupInterceptors'

interface ReduxProviderProps {
  children: React.ReactNode
}

export const ReduxProvider: React.FC<ReduxProviderProps> = ({ children }) => {
  const initializedRef = useRef(false)

  useEffect(() => {
    if (initializedRef.current) return
    setupInterceptors(store)
    initializedRef.current = true
  }, [])

  return <Provider store={store}>{children}</Provider>
}
