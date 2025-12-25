
import React from "react";
import styled from "@emotion/styled"; // 或 "styled-components"
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import { Link as RouterLink } from "react-router-dom";

// this topbar now map over shared nav arrays
// to remove duplicated JSX
// and make ordering/labels a single-source-of-truth update. 
import {
  APP_LABELS,
  NAV_ARIA,
  NAV_LAYOUT,
  TOPBAR_NAV_ITEMS,
} from "@/shared/constants/navigation.constants";

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
  gap: ${NAV_LAYOUT.TOP_GAP_PX}px;
`;

const Title = styled(Typography)`
  flex-grow: ${NAV_LAYOUT.TITLE_FLEX};
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
        <Title component="h1" variant="h6">
          {APP_LABELS.BRAND}
        </Title>

        <TopNav aria-label={NAV_ARIA.TOP}>
          {TOPBAR_NAV_ITEMS.map((item) => (
            <Button
              key={item.to}
              color="inherit"
              component={RouterLink}
              to={item.to}
            >
              {item.label}
            </Button>
          ))}
        </TopNav>
      </Toolbar>
    </HeaderBar>
  );
};
