import { Box } from '@mui/material'
import { Outlet } from 'react-router-dom'

// 这里的 PublicLayout 变成了一个单纯的“舞台”
// 具体的“布景”（背景色）由具体的页面（Page）自己决定
export const PublicLayout = () => {
  return (
    <Box component="main" sx={{ minHeight: '100vh' }}>
      <Outlet />
    </Box>
  )
}
