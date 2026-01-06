import React from "react";
import { styled } from "@mui/material/styles";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import { CheckCircleOutline } from "@mui/icons-material";



const WholeSection = styled("section")(({ theme }) => ({
  backgroundColor: theme.palette.background.default,
  padding: theme.spacing(10, 0),
}));

const WholeContentWrapper = styled(Box)(({ theme }) => ({
  maxWidth: 1280,   //theme里没有找到参数
  margin: "0 auto",
  padding: theme.spacing(0, 4),
}));

const ContentLayout = styled(Box)(({ theme }) => ({
  display: "grid",
  gridTemplateColumns: "1fr",
  gap: theme.spacing(4),
  alignItems: "center",

  [theme.breakpoints.up("md")]: {
    gridTemplateColumns: "1fr 1fr",
    gap: theme.spacing(8),
  },
}));


const StyledImage = styled("img")(({ theme }) => ({
  width: "100%",
  borderRadius: theme.shape.borderRadius,
  boxShadow: theme.shadows[3],
}));

const WordsWrapper = styled(Box)({
  margin: 0,
  padding: 0,
})

const SectionTitle = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(2)
}));

const SectionDescription = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(3)
}));


const AwardList = styled("ul")(({ theme }) => ({
  margin: 0,
  padding: 0,
  listStyle: "none",
  marginBottom: theme.spacing(4)
}));

const AwardItem = styled("li")(({ theme }) => ({
  display: "flex",
  alignItems: "flex-start",
  gap: theme.spacing(1.5),
  padding: theme.spacing(1.5, 0),
}));


const CheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
  color: theme.palette.success.main,
  marginTop: theme.spacing(0.25),
  verticalAlign: "middle",
  fontSize: "inherit"
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
    <WholeSection>
      <WholeContentWrapper>
        <ContentLayout>
          <StyledImage
            src="https://images.unsplash.com/photo-1600880292203-757bb62b4baf?w=800&q=80"
            alt="Australian small business owner"
          />

          <WordsWrapper>
            <SectionTitle variant="h3">
              Built for Australian Small Business Owners
            </SectionTitle>

            <SectionDescription variant="body1">
              Whether you run a café in Melbourne, a retail store in Sydney, or a
              hospitality venue in Brisbane — FairWorkly understands your
              specific award requirements.
            </SectionDescription>

            <AwardList>
              {awards.map((award) => (
                <AwardItem key={award.id}>
                  <CheckIcon aria-hidden="true" />
                  <Typography variant="h6" >{award.title}</Typography>
                  <Typography variant="body1">{award.description}</Typography>
                </AwardItem>
              ))}
            </AwardList>
          </WordsWrapper>
        </ContentLayout>
      </WholeContentWrapper>
    </WholeSection>
  );
};
