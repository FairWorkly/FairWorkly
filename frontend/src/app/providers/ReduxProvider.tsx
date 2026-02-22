import React, { useEffect, useRef } from 'react'
import { Provider } from 'react-redux'
import { store } from '../../store'
import { setupInterceptors } from '../../services/setupInterceptors'
import { setAuthData, setInitialized } from '../../slices/auth/authSlice'
import axios from 'axios'

interface ReduxProviderProps {
  children: React.ReactNode
}

export const ReduxProvider: React.FC<ReduxProviderProps> = ({ children }) => {
  const initializedRef = useRef(false)

  useEffect(() => {
    if (initializedRef.current) return
    setupInterceptors(store)

    // Try to refresh token on app startup, then fetch real user data
    const initializeAuth = async () => {
      try {
        const baseURL = import.meta.env.VITE_API_BASE_URL ?? '/api'
        const refreshRes = await axios.post(`${baseURL}/auth/refresh`, null, {
          withCredentials: true,
        })

        const refreshData = refreshRes.data?.data ?? refreshRes.data
        const accessToken = refreshData?.accessToken
        if (!accessToken) return

        // Fetch real user data using the new access token
        const meRes = await axios.get(`${baseURL}/auth/me`, {
          headers: { Authorization: `Bearer ${accessToken}` },
        })

        const u = meRes.data?.data ?? meRes.data
        const role = typeof u?.role === 'string' ? u.role.toLowerCase() : undefined
        if (!u?.id || !u?.email || !role) {
          throw new Error('/auth/me returned incomplete user data')
        }
        store.dispatch(setAuthData({
          user: {
            id: u.id,
            email: u.email,
            name: [u.firstName, u.lastName].filter(Boolean).join(' ') || u.email,
            role,
          },
          accessToken,
        }))
      } catch {
        // No valid refresh token, user needs to login
      } finally {
        store.dispatch(setInitialized())
      }
    }

    initializeAuth()
    initializedRef.current = true
  }, [])

  return <Provider store={store}>{children}</Provider>
}
