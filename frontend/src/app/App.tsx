import { useEffect, useRef } from 'react'
import { RouterProvider } from 'react-router-dom'
import { router } from './routes'
import httpClient from '../services/httpClient'
import { setAuthData, logout } from '../slices/auth'
import { useAppDispatch } from '../store/hooks'

export function App() {
  const dispatch = useAppDispatch()
  const initializedRef = useRef(false)

  useEffect(() => {
    if (initializedRef.current) return
    initializedRef.current = true

    const hydrateAuth = async () => {
      try {
        const refreshResponse = await httpClient.post('/auth/refresh')
        const accessToken = refreshResponse.data?.accessToken ?? refreshResponse.data?.token
        if (!accessToken) {
          throw new Error('Refresh succeeded without access token')
        }

        const meResponse = await httpClient.get('/auth/me', {
          headers: { Authorization: `Bearer ${accessToken}` },
        })
        const normalizedUser = {
          ...meResponse.data,
          role: typeof meResponse.data?.role === 'string'
            ? meResponse.data.role.toLowerCase()
            : meResponse.data?.role,
        }
        dispatch(setAuthData({ user: normalizedUser, accessToken }))
      } catch {
        dispatch(logout())
      }
    }

    hydrateAuth()
  }, [dispatch])

  return <RouterProvider router={router} />
}
