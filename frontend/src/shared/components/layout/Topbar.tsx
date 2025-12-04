
import React from "react";
import styled from "@emotion/styled"; // 或 "styled-components"
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import { Link as RouterLink } from "react-router-dom";

interface TopbarProps {
  drawerWidth: number;
}

// 语义：整个顶部栏是 <header>，里面还是用 MUI AppBar
const HeaderBar = styled(AppBar, {
  shouldForwardProp: (prop) => prop !== "drawerWidth",
})<{
  drawerWidth: number;
}>(({ theme, drawerWidth }) => ({
  zIndex: theme.zIndex.drawer + 1,
  marginLeft: drawerWidth,
  width: `calc(100% - ${drawerWidth}px)`,
}));

// 顶部右侧导航区域，用 <nav>
const TopNav = styled.nav`
  display: flex;
  align-items: center;
  gap: 8px;
`;

export const Topbar: React.FC<TopbarProps> = ({ drawerWidth }) => {
  return (
    <HeaderBar
      position="fixed"
      component="header"
      role="banner"
      elevation={1}
      drawerWidth={drawerWidth}
    >
      <Toolbar component="div">
        <Typography
          component="h1"
          variant="h6"
          sx={{ flexGrow: 1 }}
        >
          FairWorkly
        </Typography>

        <TopNav aria-label="Top navigation">
          <Button
            color="inherit"
            component={RouterLink}
            to="/compliance/qa"
          >
            Compliance Q&A
          </Button>
          <Button color="inherit" component={RouterLink} to="/payroll">
            Payroll
          </Button>
          <Button color="inherit" component={RouterLink} to="/documents">
            Documents
          </Button>
          <Button color="inherit" component={RouterLink} to="/employee/help">
            Employee Help
          </Button>
        </TopNav>
      </Toolbar>
    </HeaderBar>
  );
};

