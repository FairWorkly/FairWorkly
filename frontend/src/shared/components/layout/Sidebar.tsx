
import React from "react";
import styled from "@emotion/styled"; // 或 "styled-components"
import Drawer from "@mui/material/Drawer";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemText from "@mui/material/ListItemText";
import Toolbar from "@mui/material/Toolbar";
import Divider from "@mui/material/Divider";
import { Link as RouterLink } from "react-router-dom";

// this sidebar now map over shared nav arrays
// to remove duplicated JSX
// and make ordering/labels a single-source-of-truth update. 
import {
  NAV_ARIA,
  SIDEBAR_NAV_ITEMS,
} from "@/shared/constants/navigation.constants";

interface SidebarProps {
  width: number;
}

// Drawer 本身作为 <nav>，语义是主导航区域
const NavigationDrawer = styled(Drawer, {
  shouldForwardProp: (prop) => prop !== "width",
})<{
  width: number;
}>(({ width }) => ({
  width,
  flexShrink: 0,
  "& .MuiDrawer-paper": {
    width,
    boxSizing: "border-box",
  },
}));

export const Sidebar: React.FC<SidebarProps> = ({ width }) => {
  return (
    <NavigationDrawer
      variant="permanent"
      component="nav"
      aria-label={NAV_ARIA.PRIMARY}
      width={width}
    >
      {/* 顶部留出和 AppBar 对齐的空间 */}
      <Toolbar component="div" />
      <Divider />

      {/* 语义：列表 = <ul> / <li> */}
      <List component="ul">
        {SIDEBAR_NAV_ITEMS.map((item) => (
          <ListItem disablePadding component="li" key={item.to}>
            <ListItemButton component={RouterLink} to={item.to}>
              <ListItemText primary={item.label} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </NavigationDrawer>
  );
};
