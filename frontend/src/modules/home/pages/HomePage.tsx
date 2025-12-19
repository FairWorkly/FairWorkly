import Navbar from "@/modules/home/components/Navbar";
import HeroSection from "@/modules/home/components/HeroSection";
import TrustBar from "@/modules/home/components/TrustBar";
import { styled } from "@mui/material/styles";

const PageRoot = styled("main")(({ theme }) => ({
  minHeight: "100vh",
  backgroundColor: theme.palette.background.default,
}));

const Content = styled("div")(() => ({
  width: "100%",
}));

export default function HomePage() {
  return (
    <PageRoot>
      <Navbar />
      <Content>
        <HeroSection />
        <TrustBar />
      </Content>
    </PageRoot>
  );
}
