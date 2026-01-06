import React from "react";
import { styled, alpha } from "@mui/material/styles";
import type { Theme } from "@mui/material/styles";

import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";

import AutoAwesomeIcon from "@mui/icons-material/AutoAwesome";
import {
  AccountBalanceWalletOutlined,
  ArticleOutlined,
  CheckCircleOutline,
  ShieldOutlined,
  SupportAgent,
} from "@mui/icons-material";
import type { SvgIconProps } from "@mui/material/SvgIcon";
import { Card } from "@mui/material";


type PaletteKey = keyof Theme["palette"];
type Tone = Extract<
  PaletteKey,
  "primary" | "warning" | "info" | "success"    //取出palette定义好的样式
>;


const WholeSection = styled("section")(({ theme }) => ({
  backgroundColor: theme.palette.background.paper,
  padding: theme.spacing(12, 0),
  borderTop: `1px solid ${theme.palette.divider}`,
  borderBottom: `1px solid ${theme.palette.divider}`,
}));

const WholeContentWrapper = styled("div")(({ theme }) => ({
  maxWidth: 1280,   //theme里没有找到参数
  margin: "0 auto",
  padding: theme.spacing(0, 4),
}));

const SectionHeader = styled("header")(({ theme }) => ({
  textAlign: "center",
  marginBottom: theme.spacing(8),
}));

const SectionLabel = styled("div")(({ theme }) => ({
  display: "inline-flex",
  alignItems: "center",
  gap: theme.spacing(1),
  padding: theme.spacing(0.75, 2),
  backgroundColor: alpha(theme.palette.primary.main, 0.12),
  color: theme.palette.primary.main,
  borderRadius: theme.shape.borderRadius,
  fontSize: "0.8125rem",      //hardcode: theme里没有找到匹配样式
  fontWeight: 600,
  textTransform: "uppercase",
  letterSpacing: "0.5px",
  marginBottom: theme.spacing(2),
}));

const LabelIcon = styled(Box)({
  fontSize: "inherit",
  verticalAlign: "middle",
  lineHeight: 0.2,   //hardcode: theme里没有找到参数
});

const HeaderContainer = styled("header")(({ theme }) => ({
  marginTop: theme.spacing(3),
}));

const Title = styled(Typography)(({ theme }) => ({
  marginBottom: theme.spacing(2),
}));

const SubTitle = styled(Typography)(({ theme }) => ({
  margin: "0 auto",
  color: theme.palette.text.secondary,
}));



const CardsLayout = styled(Box)(({ theme }) => ({
  display: "grid",
  gridTemplateColumns: "1fr",
  gap: theme.spacing(4),
  [theme.breakpoints.up("sm")]: {
    gridTemplateColumns: "repeat(2, 1fr)",
  },
}));

const FeatureCardContainer = styled(Card)(({ theme }) => ({
  backgroundColor: theme.palette.background.default,
  padding: theme.spacing(4),
  border: `1px solid ${theme.palette.divider}`,
  transition: "all 0.3s ease",

  "&:hover": {
    boxShadow: theme.shadows[4],
    borderColor: theme.palette.primary.main,
  },
}));

const CardHeader = styled(Box)(({ theme }) => ({
  display: "flex",
  alignItems: "flex-start",
  gap: theme.spacing(2),
  marginBottom: theme.spacing(3),
}));

const CardIconWrapper = styled(Box)<{ tone: Tone }>(({ theme, tone }) => ({
  width: theme.spacing(7),
  height: theme.spacing(7),
  borderRadius: theme.spacing(1.75),
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  flexShrink: 0,

  backgroundColor: alpha(theme.palette[tone].main, 0.12),
  color: theme.palette[tone].main,

  "& svg": {
    fontSize: "1.75rem",    //hardcode: icon大小theme里没有找到匹配
  },
}));

const CardHeaderContent = styled(Box)({
  display: "block",
});

const CardAgentTypeLabel = styled("span")<{ tone: Tone }>(({ theme, tone }) => ({
  fontSize: "0.75rem",
  fontWeight: 600,             //hardcode: theme里没有匹配样式
  letterSpacing: "0.05em",
  textTransform: "uppercase",
  color: theme.palette[tone].main,
}));

const CardTitle = styled(Typography)({
  fontWeight: 700,
})

const CardDescription = styled(Typography)(({ theme }) => ({
  color: theme.palette.text.secondary,
  marginBottom: theme.spacing(3),
}));


const CardFeaturesLayout = styled("ul")(({ theme }) => ({
  display: "grid",
  gridTemplateColumns: "1 fr",
  gap: theme.spacing(1.5),
  listStyle: "none",
  padding: 0,
  margin: 0,

  [theme.breakpoints.up("sm")]: {
    gridTemplateColumns: "repeat(2, 1fr)",
  },
}));

const CardFeatureItem = styled("li")(({ theme }) => ({
  display: "flex",
  alignItems: "center",
  gap: theme.spacing(1),
  ...theme.typography.body2,
  color: theme.palette.text.secondary,
}));

const CardCheckIcon = styled(CheckCircleOutline)(({ theme }) => ({
  color: theme.palette.success.main,
  fontSize: "1.125rem",         //icon大小theme里没有找到匹配参数
  flexShrink: 0,
}));



interface FeatureCardData {
  id: string;
  agentType: string;
  tone: Tone;
  icon: React.ComponentType<SvgIconProps>;
  title: string;
  description: string;
  features: string[];
}

function FeatureCard({ data }: { data: FeatureCardData }) {
  const { tone, icon: IconComponent, agentType, title, description, features } =
    data;

  return (
    <FeatureCardContainer elevation={0}>
      <CardHeader>
        <CardIconWrapper tone={tone} aria-hidden="true">
          <IconComponent />
        </CardIconWrapper>

        <CardHeaderContent>
          <CardAgentTypeLabel tone={tone}>{agentType}</CardAgentTypeLabel>
          <CardTitle variant="h5" >{title}</CardTitle>
        </CardHeaderContent>
      </CardHeader>

      <CardDescription variant="body2">{description}</CardDescription>

      <CardFeaturesLayout>
        {features.map((feature) => (
          <CardFeatureItem key={`${agentType}-${feature}`}>
            <CardCheckIcon aria-hidden="true" />
            <span>{feature}</span>
          </CardFeatureItem>
        ))}
      </CardFeaturesLayout>
    </FeatureCardContainer>
  );
}



export const FeaturesSection: React.FC = () => {

  const content = {
    label: "FEATURES",
    title: "Four AI Agents Working For You",
    subtitle: "Specialised AI that understands Australian workplace law",
  };

  const cards: FeatureCardData[] = [
    {
      id: "compliance-agent",
      agentType: "COMPLIANCE AGENT",
      tone: "primary",
      icon: ShieldOutlined,
      title: "Roster Compliance Checking",
      description:
        "Upload your roster and get instant compliance analysis against Modern Award requirements.",
      features: ["Hours validation", "Break requirements", "Penalty checks", "AI Q&A"],
    },
    {
      id: "payroll-agent",
      agentType: "PAYROLL AGENT",
      tone: "warning",
      icon: AccountBalanceWalletOutlined,
      title: "Pay Validation & Audit",
      description:
        "Validate your payroll against award rates before you process, not after.",
      features: ["Rate validation", "Underpayment detect", "Super checks", "STP Phase 2"],
    },
    {
      id: "document-agent",
      agentType: "DOCUMENT AGENT",
      tone: "info",
      icon: ArticleOutlined,
      title: "Compliant Documents",
      description:
        "Generate legally compliant HR documents in seconds with AI assistance.",
      features: ["Contracts", "Warning letters", "Termination", "PDF export"],
    },
    {
      id: "self-service-agent",
      agentType: "SELF-SERVICE AGENT",
      tone: "success",
      icon: SupportAgent,
      title: "Employee Portal",
      description:
        "Let employees answer their own questions about pay and leave entitlements.",
      features: ["Leave queries", "Pay questions", "Policy lookups", "24/7 available"],
    },
  ];

  return (
    <WholeSection>
      <WholeContentWrapper>
        <SectionHeader>
          <SectionLabel>
            <LabelIcon aria-hidden>
              <AutoAwesomeIcon fontSize="inherit" />
            </LabelIcon>
            {content.label}
          </SectionLabel>

          <HeaderContainer>
            <Title variant="h2">{content.title}</Title>
            <SubTitle variant="body1">{content.subtitle}</SubTitle>
          </HeaderContainer>
        </SectionHeader>


        <CardsLayout>
          {cards.map((card) => (
            <FeatureCard key={card.id} data={card} />
          ))}
        </CardsLayout>

      </WholeContentWrapper>
    </WholeSection>
  );
};
