import { styled } from '@/styles/styled'

export const PublicShell = styled('div')(({ theme }) => {
  const gridLine = theme.fairworkly.effect.gridLine

  return {
    minHeight: '100vh',
    backgroundColor: theme.palette.background.default,
    backgroundImage: `
      radial-gradient(ellipse 80% 50% at 20% 0%, rgba(99,102,241,0.08) 0%, transparent 50%),
      radial-gradient(ellipse 60% 40% at 80% 100%, rgba(236,72,153,0.06) 0%, transparent 50%),
      linear-gradient(${gridLine} 1px, transparent 1px),
      linear-gradient(90deg, ${gridLine} 1px, transparent 1px)
    `,
    backgroundSize: 'auto, auto, 60px 60px, 60px 60px',
    backgroundAttachment: 'fixed',
    overflowX: 'hidden',
  }
})


