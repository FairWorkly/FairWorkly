import { useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { LoginForm, SignupForm, ForgotPasswordModal } from '../features'
import type { SignupFormData } from '../types'
import { useLogin } from '../hooks'
import {
  AuthErrorAlert,
  AuthHeader,
  AuthTitle,
  AuthSubtitle,
  AuthTabList,
  AuthTabButton,
} from '../ui'

type TabType = 'login' | 'signup'

export function LoginPage() {
  const [searchParams] = useSearchParams()
  const { login, isSubmitting: isLoginSubmitting, error: loginError } = useLogin()
  const initialTab = searchParams.get('signup') === 'true' ? 'signup' : 'login'
  const [activeTab, setActiveTab] = useState<TabType>(initialTab)
  const [forgotModalOpen, setForgotModalOpen] = useState(false)
  const isGoogleLoading = false

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

      {loginError && (
        <AuthErrorAlert severity="error">
          {loginError}
        </AuthErrorAlert>
      )}

      {activeTab === 'login' ? (
        <LoginForm
          onSubmit={login}
          onGoogleLogin={handleGoogleLogin}
          onForgotPassword={() => setForgotModalOpen(true)}
          isSubmitting={isLoginSubmitting}
          isGoogleLoading={isGoogleLoading}
        />
      ) : (
        <SignupForm
          onSubmit={handleSignup}
          onGoogleLogin={handleGoogleLogin}
          isSubmitting={isLoginSubmitting}
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
