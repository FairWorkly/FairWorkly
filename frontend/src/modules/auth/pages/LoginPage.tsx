import { useState } from 'react'
import { useSearchParams, useNavigate } from 'react-router-dom'
import { useDispatch } from 'react-redux'
import { LoginForm, SignupForm, ForgotPasswordModal } from '../features'
import type { LoginFormData, SignupFormData } from '../types'
import { authApi } from '@/services/authApi'
import { setAuthData } from '@/slices/auth/authSlice'
import {
  AuthErrorAlert,
  AuthHeader,
  AuthTitle,
  AuthSubtitle,
  AuthTabList,
  AuthTabButton,
} from '../ui'

type TabType = 'login' | 'signup'

const DEFAULT_ROUTES: Record<string, string> = {
  admin: '/fairbot',
  manager: '/roster/upload',
}

export function LoginPage() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const dispatch = useDispatch()
  const initialTab = searchParams.get('signup') === 'true' ? 'signup' : 'login'
  const [activeTab, setActiveTab] = useState<TabType>(initialTab)
  const [forgotModalOpen, setForgotModalOpen] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const isGoogleLoading = false
  const [error, setError] = useState<string | null>(null)

  const handleLogin = async (values: LoginFormData) => {
    if (isSubmitting) return

    setIsSubmitting(true)
    setError(null)

    try {
      const response = await authApi.login(values.email, values.password)

      // Store auth data in Redux
      dispatch(setAuthData({
        user: {
          id: response.user.id,
          email: response.user.email,
          name: [response.user.firstName, response.user.lastName].filter(Boolean).join(' ') || response.user.email,
          role: response.user.role,
        },
        accessToken: response.accessToken,
      }))

      // Navigate to role-appropriate default route
      navigate(DEFAULT_ROUTES[response.user.role.toLowerCase()] ?? '/403')
    } catch (err: unknown) {
      console.error('Login failed:', err)
      setError('Invalid email or password. Please try again.')
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleSignup = (values: SignupFormData) => {
    // TODO: Implement actual signup logic
    console.log('Signup not yet implemented:', values)
  }

  const handleGoogleLogin = () => {
    // TODO: Backend-driven Google OAuth (redirect to server auth endpoint).
    console.log('Google login not yet implemented')
  }

  return (
    <section>
      <AuthHeader>
        <AuthTitle>Welcome back</AuthTitle>
        <AuthSubtitle>
          {activeTab === 'login'
            ? 'Sign in to manage your compliance'
            : 'Create your account to get started'}
        </AuthSubtitle>
      </AuthHeader>

      <AuthTabList role="tablist">
        <AuthTabButton type="button" active={activeTab === 'login'} onClick={() => setActiveTab('login')}>
          Sign In
        </AuthTabButton>
        <AuthTabButton type="button" active={activeTab === 'signup'} onClick={() => setActiveTab('signup')}>
          Create Account
        </AuthTabButton>
      </AuthTabList>

      {error && (
        <AuthErrorAlert severity="error">
          {error}
        </AuthErrorAlert>
      )}

      {activeTab === 'login' ? (
        <LoginForm
          onSubmit={handleLogin}
          onGoogleLogin={handleGoogleLogin}
          onForgotPassword={() => setForgotModalOpen(true)}
          isSubmitting={isSubmitting}
          isGoogleLoading={isGoogleLoading}
        />
      ) : (
        <SignupForm
          onSubmit={handleSignup}
          onGoogleLogin={handleGoogleLogin}
          isSubmitting={isSubmitting}
          isGoogleLoading={isGoogleLoading}
        />
      )}

      <ForgotPasswordModal
        open={forgotModalOpen}
        onClose={() => setForgotModalOpen(false)}
      />
    </section>
  )
}
