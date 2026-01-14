import React from 'react'
import ReactDOM from 'react-dom/client'
import { AppProviders } from './app/providers/AppProviders'
import { App } from './app/App'
import '@/styles/theme/augment'

const rootEl = document.getElementById('root')
if (!rootEl) throw new Error('Root element #root not found')

ReactDOM.createRoot(rootEl).render(
  <React.StrictMode>
    <AppProviders>
      <App />
    </AppProviders>
  </React.StrictMode>
)
