import React from 'react'
import styled from '@emotion/styled'
import { Topbar } from './Topbar'
import { Sidebar } from './Sidebar'

interface MainLayoutProps {
  children: React.ReactNode
}

/**
 * 整个应用的外层容器（语义上就是一个布局 div）
 */
const AppShell = styled.div`
  display: flex;
  min-height: 100vh;
`

/**
 * 侧边栏区域，用 <aside> 语义化：辅助导航/信息
 */
const SidebarRegion = styled.aside`
  /* Sidebar 自己有宽度和样式，这里只负责语义标签 */
`

/**
 * 主内容区域，用 <main> 语义化：页面主内容
 */
const MainContent = styled.main<{ sidebarWidth: number }>`
  flex: 1;
  padding: 24px;
  margin-top: 64px; /* 顶部 Topbar 的高度，大概值，后面可以抽成常量 */
  margin-left: ${({ sidebarWidth }) => `${sidebarWidth}px`};

  @media (max-width: 900px) {
    margin-left: 0; /* 小屏时可以让主内容占满宽度，后续可以配合隐藏 Sidebar */
  }
`

export const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
  // 和 Topbar / Sidebar 约定好的侧边栏宽度
  const sidebarWidth = 220

  return (
    <AppShell>
      {/* 头部区域（Topbar 里用 MUI AppBar，后面可以把 component 改成 header） */}
      <Topbar drawerWidth={sidebarWidth} />

      {/* 侧边栏区域，用 aside 包一层，语义：导航/辅助内容 */}
      <SidebarRegion aria-label="Primary navigation">
        <Sidebar width={sidebarWidth} />
      </SidebarRegion>

      {/* 主内容区域，用 main，语义：当前页面的主要内容 */}
      <MainContent sidebarWidth={sidebarWidth} aria-label="Main content area">
        {children}
      </MainContent>
    </AppShell>
  )
}
