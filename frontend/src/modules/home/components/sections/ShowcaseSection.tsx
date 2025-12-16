import { styled } from "@mui/material/styles";
import { CheckCircleOutline } from '@mui/icons-material';
import Box from '@mui/material/Box';
import { SectionContainer, ContentWrapper } from './SectionComponents';
import { tokens, customTheme } from "@/app/providers/ThemeProvider";


interface AwardItem {
  id: string;
  title: string;
  description: string;
};

const AWARDS: AwardItem[] = [
  {
    id: "retail",
    title: "Retail Award",
    description: "— Complex penalty rates simplified",
  },
  {
    id: "hospitality",
    title: "Hospotality Award",
    description: "— Overtime, split shifts covered",
  },
  {
    id: "clerks",
    title: "Clerk Award",
    description: "— Office entitlements made clear",
  }
];


const ContentGrid = styled(Box)(({ theme }) => ({
  display: "grid",
  gridTemplateColumns: "1fr",
  gap: "48px",
  alignItems: "center",
  [theme.breakpoints.up("md")]: {
    gridTemplateColumns: "1fr 1fr",
    gap: "64px",
  },
}));

const ImageColumn = styled(Box)({
  position: "relative",
  borderRadius: "16px",
  overflow: "hidden",
  boxShadow: "0 25px 50px -12px rgba(0, 0, 0, 0.15)",
});

const StyledImage = styled("img")({
  width: "100%",
  height: "auto",
  display: "block",
  objectFit: "cover",
});

const TextColumn = styled(Box)({
  display: "flex",
  flexDirection: "column",
  gap: "24px",
});

const Heading = styled("h2")({
  fontSize: "24px",
  fontWeight: 700,
  color: tokens.colors.gray900,
  lineHeight: 1.2,
  margin: 0,
  [customTheme.breakpoints.up("md")]: {
    fontSize: "32px",
  },
});

const Paragraph = styled("p")({
  fontSize: "14px",
  lineHeight: 1.6,
  color: tokens.colors.gray500,
  margin: 0,

  [customTheme.breakpoints.up("md")]: {
    fontSize: "18px",
  }
});

const AwardList = styled("ul")({
  listStyle: "none",
  padding: 0,
  margin: 0,
  display: "flex",
  flexDirection: "column",
  gap: "16px",
});

const AwardItem = styled("li")({
  display: "flex",
  alignItems: "flex-start",
  gap: "12px"
});

const AwardContent = styled("div")({
  display: "flex",
  flexWrap: "wrap"
});

const AwardTitle = styled("span")({
  fontSize: "16px",
  fontWeight: 700,
  color: tokens.colors.gray900,
});

const AwardDescription = styled("span")({
  fontSize: "16px",
  fontWeight: 400,
  marginLeft: "5px",
  color: tokens.colors.gray500,
});

const CheckIcon = styled(CheckCircleOutline)({
  fontSize: '1.25rem',
  color: customTheme.palette.success.main,
  flexShrink: 0,
});


export const ShowcaseSection = () => {
  return (
    <SectionContainer>
      <ContentWrapper>
        <ContentGrid>
          <ImageColumn>
            <StyledImage
              src="https://images.unsplash.com/photo-1600880292203-757bb62b4baf?w=800&q=80"
              alt="Australian small business owner"
            />
          </ImageColumn>
          <TextColumn>
            <Heading>Built for Australian Small Business Owners</Heading>
            <Paragraph>Whether you run a café in Melbourne, a retail store in Sydney, or a hospitality venue in Brisbane — FairWorkly understands your specific award requirements.</Paragraph>
            <AwardList>
              {AWARDS.map((award) => (
                <AwardItem key={award.id}>
                  <CheckIcon />
                  <AwardContent>
                    <AwardTitle>{award.title}</AwardTitle>
                    <AwardDescription>{award.description}</AwardDescription>
                  </AwardContent>
                </AwardItem>
              ))}
            </AwardList>
          </TextColumn>
        </ContentGrid>
      </ContentWrapper>
    </SectionContainer>
  )
}
