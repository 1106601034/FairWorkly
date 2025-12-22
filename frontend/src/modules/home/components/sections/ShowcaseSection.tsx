import React from "react";
import { styled } from "@mui/material/styles";
import Box, { type BoxProps } from "@mui/material/Box";
import Typography, { type TypographyProps } from "@mui/material/Typography";
import { CheckCircleOutline } from "@mui/icons-material";



const ShowcaseSectionWrapper = styled("section")(({ theme }) => ({
  backgroundColor: theme.palette.background.default,
  padding: theme.spacing(10, 0),
}));

const ShowcaseContentWrapper = styled("div")(({ theme }) => ({
  maxWidth: 1280,
  margin: "0 auto",
  padding: theme.spacing(0, 3),
}));

const ContentGrid = styled("div")(({ theme }) => ({
  display: "grid",
  gridTemplateColumns: "1fr",
  gap: theme.spacing(6),
  alignItems: "center",

  [theme.breakpoints.up("md")]: {
    gridTemplateColumns: "1fr 1fr",
    gap: theme.spacing(8),
  },
}));


const ImageColumn = styled(Box)<BoxProps>(({ theme }) => ({
  position: "relative",
  borderRadius: theme.shape.borderRadius,
  overflow: "hidden",
  boxShadow: theme.shadows[3],
}));

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

const Heading = styled(Typography)<TypographyProps>(({ theme }) => ({
  color: theme.palette.text.primary,
}));

const Paragraph = styled(Typography)<TypographyProps>(({ theme }) => ({
  color: theme.palette.text.secondary,
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

const AwardContent = styled("span")({
  display: "inline-flex",
  flexWrap: "wrap",
  alignItems: "baseline",
});

const AwardTitle = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.primary,
  display: "inline",
}));

const AwardDescription = styled(Typography)(({ theme }) => ({
  marginLeft: theme.spacing(0.75),
  color: theme.palette.text.secondary,
  display: "inline",
}));

const CheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
  color: theme.palette.success.main,
  flexShrink: 0,
}));



export const ShowcaseSection: React.FC = () => {

  const awards = [
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
    },
  ];

  return (
    <ShowcaseSectionWrapper>
      <ShowcaseContentWrapper>
        <ContentGrid>
          <ImageColumn>
            <StyledImage
              src="https://images.unsplash.com/photo-1600880292203-757bb62b4baf?w=800&q=80"
              alt="Australian small business owner"
            />
          </ImageColumn>

          <TextColumn>
            <Heading variant="h3" component="h2">
              Built for Australian Small Business Owners
            </Heading>

            <Paragraph variant="body1" component="p">
              Whether you run a café in Melbourne, a retail store in Sydney, or a
              hospitality venue in Brisbane — FairWorkly understands your
              specific award requirements.
            </Paragraph>

            <AwardList>
              {awards.map((award) => (
                <AwardItem key={award.id}>
                  <CheckIcon aria-hidden="true" />
                  <AwardContent>
                    <AwardTitle variant="subtitle1" >{award.title}</AwardTitle>
                    <AwardDescription >{award.description}</AwardDescription>
                  </AwardContent>
                </AwardItem>
              ))}
            </AwardList>
          </TextColumn>
        </ContentGrid>
      </ShowcaseContentWrapper>
    </ShowcaseSectionWrapper>
  );
};
