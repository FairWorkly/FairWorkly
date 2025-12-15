
import styled from '@emotion/styled';
import { CheckCircleOutline } from '@mui/icons-material';
import Box from '@mui/material/Box';
import React from 'react'


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

const SectionContainer = styled("section")(({ theme }) => ({
  backgroundColor: "#F9FAFB",
  padding: "80px 24px",
  [theme.breakpoints.up("md")]: {
    padding: "100px 48px",
  }
}));

const ContentWrapper = styled(Box)({
  maxWidth: "1280px",
  margin: "0 auto",

});

const ContentGrid = styled(Box)(({ theme }) => ({
  display: "grid",
  gridTemplateColumns: "1fr",
  gap: theme.spacing(6),
  alignItems: "center",
  [theme.breakpoints.up("md")]: {
    gridTemplateColumns: "1fr 1fr",
    gap: theme.spacing(8),
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

const TextColumn = styled(Box)(({ theme }) => ({
  display: "flex",
  flexDirection: "column",
  gap: theme.spacing(3),
}));

const Heading = styled("h2")(({ theme }) => ({
  fontSize: "24px",
  fontWeight: 700,
  color: theme.palette.text.primary,
  lineHeight: 1.2,
  margin: 0,
  [theme.breakpoints.up("md")]: {
    fontSize: "32px",
  },
}));

const Paragraph = styled("p")(({ theme }) => ({
  fontSize: "14px",
  lineHeight: 1.6,
  color: theme.palette.text.secondary,
  margin: 0,

  [theme.breakpoints.up("md")]: {
    fontSize: "18px",
  }
}))

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
  gap: theme.spacing(1.5)
}));

const AwardContent = styled("div")({
  display: "flex",
  gap: "2px,"
});

const AwardTitle = styled("span")(({ theme }) => ({
  fontSize: "16px",
  fontWeight: 700,
  color: theme.palette.text.primary,
}));

const AwardDescription = styled("span")(({ theme }) => ({
  fontSize: "16px",
  fontWeight: 400,
  marginLeft: "5px",
  color: theme.palette.text.secondary,
}))

const CheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
  fontSize: '1.25rem',
  color: theme.palette.success.main,
  flexShrink: 0,
}));


export const ShowcaseSection = () => {
  return (
    <SectionContainer>
      <ContentWrapper>
        <ContentGrid>
          <ImageColumn>
            <StyledImage src="https://images.unsplash.com/photo-1600880292203-757bb62b4baf?w=800&q=80" />
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
