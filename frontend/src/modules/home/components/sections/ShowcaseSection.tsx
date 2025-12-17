import { styled } from "@mui/material/styles";
import { CheckCircleOutline } from '@mui/icons-material';
import Box, { type BoxProps } from '@mui/material/Box';
import { SectionContainer, ContentWrapper } from './SectionComponents';
import { tokens } from "@/app/providers/ThemeProvider";


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
    title: "Hospitality Award",
    description: "— Overtime, split shifts covered",
  },
  {
    id: "clerks",
    title: "Clerk Award",
    description: "— Office entitlements made clear",
  }
];


const ContentGrid = styled(Box)<BoxProps>(({ theme }) => ({
  display: "grid",
  gridTemplateColumns: "1fr",
  gap: theme.spacing(6),
  alignItems: "center",
  [theme.breakpoints.up("md")]: {
    gridTemplateColumns: "1fr 1fr",
    gap: theme.spacing(8),
  },
}));

const ImageColumn = styled(Box)<BoxProps>({
  position: "relative",
  borderRadius: tokens.borderRadius.large,
  overflow: "hidden",
  boxShadow: tokens.imageShadow,
});

const StyledImage = styled("img")({
  width: "100%",
  height: "auto",
  display: "block",
  objectFit: "cover",
});

const TextColumn = styled(Box)<BoxProps>(({ theme }) => ({
  display: "flex",
  flexDirection: "column",
  gap: theme.spacing(3),
}));

const Heading = styled("h2")(({ theme }) => ({
  fontSize: "1.5rem",
  fontWeight: 700,
  color: tokens.colors.gray900,
  lineHeight: 1.2,
  margin: 0,
  [theme.breakpoints.up("md")]: {
    fontSize: "2rem",
  },
}));

const Paragraph = styled("p")(({ theme }) => ({
  fontSize: "0.875rem",
  lineHeight: 1.6,
  color: tokens.colors.gray500,
  margin: 0,

  [theme.breakpoints.up("md")]: {
    fontSize: "1.125rem",
  }
}));

const AwardList = styled("ul")(({ theme }) => ({
  listStyle: "none",
  padding: 0,
  margin: 0,
  display: "flex",
  flexDirection: "column",
  gap: theme.spacing(2),
}));

const AwardItem = styled("li")(({ theme }) => ({
  display: "flex",
  alignItems: "flex-start",
  gap: theme.spacing(1.5),
}));

const AwardContent = styled(Box)<BoxProps>({
  display: "flex",
  flexWrap: "wrap",
});

const AwardTitle = styled("span")({
  fontSize: "1rem",
  fontWeight: 700,
  color: tokens.colors.gray900,
});

const AwardDescription = styled("span")(({ theme }) => ({
  fontSize: "1rem",
  fontWeight: 400,
  marginLeft: theme.spacing(0.625),
  color: tokens.colors.gray500,
}));

const CheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
  fontSize: "1.25rem",
  color: theme.palette.success.main,
  flexShrink: 0,
}));


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
                  <CheckIcon aria-hidden="true" />
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
