
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
      aria-label="Primary navigation"
      width={width}
    >
      {/* 顶部留出和 AppBar 对齐的空间 */}
      <Toolbar component="div" />
      <Divider />

      {/* 语义：列表 = <ul> / <li> */}
      <List component="ul">
        <ListItem disablePadding component="li">
          <ListItemButton component={RouterLink} to="/">
            <ListItemText primary="Home" />
          </ListItemButton>
        </ListItem>

        <ListItem disablePadding component="li">
          <ListItemButton component={RouterLink} to="/compliance">
            <ListItemText primary="Compliance" />
          </ListItemButton>
        </ListItem>

        <ListItem disablePadding component="li">
          <ListItemButton component={RouterLink} to="/payroll">
            <ListItemText primary="Payroll" />
          </ListItemButton>
        </ListItem>

        <ListItem disablePadding component="li">
          <ListItemButton component={RouterLink} to="/documents">
            <ListItemText primary="Documents" />
          </ListItemButton>
        </ListItem>

        <ListItem disablePadding component="li">
          <ListItemButton component={RouterLink} to="/employee">
            <ListItemText primary="Employees" />
          </ListItemButton>
        </ListItem>
      </List>
    </NavigationDrawer>
  );
};

