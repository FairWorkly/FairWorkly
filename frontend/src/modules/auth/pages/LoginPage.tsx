import { useState } from 'react'
import { useSearchParams, useNavigate } from 'react-router-dom'
import { LoginForm, SignupForm, ForgotPasswordModal } from '../features'
import type { LoginFormData, SignupFormData } from '../types'
import {
  AuthHeader,
  AuthTitle,
  AuthSubtitle,
  AuthTabList,
  AuthTabButton,
} from '../ui'

type TabType = 'login' | 'signup'
const DEV_USER_NAME_STORAGE_KEY = 'dev:user-name'
const AUTH_SIMULATED_DELAY_MS = 900

export function LoginPage() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const initialTab = searchParams.get('signup') === 'true' ? 'signup' : 'login'
  const [activeTab, setActiveTab] = useState<TabType>(initialTab)
  const [forgotModalOpen, setForgotModalOpen] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [isGoogleLoading, setIsGoogleLoading] = useState(false)

  const simulateAuth = (options: { name: string; provider: 'email' | 'google' }) => {
    if (isSubmitting || isGoogleLoading) return
    const setLoading = options.provider === 'google' ? setIsGoogleLoading : setIsSubmitting
    setLoading(true)

    setTimeout(() => {
      if (typeof window !== 'undefined') {
        localStorage.setItem(DEV_USER_NAME_STORAGE_KEY, options.name)
      }
      setLoading(false)
      navigate('/fairbot')
    }, AUTH_SIMULATED_DELAY_MS)
  }

  const handleLogin = (values: LoginFormData) => {
    // TODO: Implement actual login logic
    const name = values.email ? values.email.split('@')[0] : 'Demo User'
    simulateAuth({ name, provider: 'email' })
  }

  const handleSignup = (values: SignupFormData) => {
    // TODO: Implement actual signup logic
    const name = values.firstName || values.email || 'New User'
    simulateAuth({ name, provider: 'email' })
  }

  const handleGoogleLogin = () => {
    // TODO: Backend-driven Google OAuth (redirect to server auth endpoint).
    simulateAuth({ name: 'Google User', provider: 'google' })
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
