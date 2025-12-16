import Box, { type BoxProps } from "@mui/material/Box";
import { styled } from "@mui/material/styles";
import Typography, { type TypographyProps } from "@mui/material/Typography";
import WarningAmberIcon from '@mui/icons-material/WarningAmber';
import {
    PaymentsOutlined,
    DescriptionOutlined,
    SentimentDissatisfied,
    Gavel,
} from "@mui/icons-material";
import { ContentWrapper, SectionContainer, SectionHeader, SectionLabel } from "./SectionComponents";
import type { SvgIconProps } from "@mui/material/SvgIcon";

interface ProblemCardData {
    id: string;
    icon: React.ComponentType<SvgIconProps>;
    value: string;
    label: string;
    description: string;
    iconBgColor: string;
    valueColor: string;
}

const PROBLEM_CARDS: ProblemCardData[] = [
    {
        id: "wage-underpayments",
        icon: PaymentsOutlined,
        value: "$1.35B",
        label: "Wage Underpayments",
        description: "Recovered by Fair Work in 2023-24",
        iconBgColor: "#EF44441A",
        valueColor: "#DC2626",
    },
    {
        id: "modern-awards",
        icon: DescriptionOutlined,
        value: "122",
        label: "Modern Awards",
        description: "Complex rules and variations",
        iconBgColor: "#F973161A",
        valueColor: "#D97706",
    },
    {
        id: "smes-not-confident",
        icon: SentimentDissatisfied,
        value: "73%",
        label: "SMEs Not Confident",
        description: "Too complex to DIY",
        iconBgColor: "#E0E7FF",
        valueColor: "#6366F1",
    },
    {
        id: "fwo-recovery",
        icon: Gavel,
        value: "$532M",
        label: "FWO Recovery",
        description: "Enforcement intensifying",
        iconBgColor: "#06B6D41A",
        valueColor: "#0891B2",
    },
];


const CardsGrid = styled(Box)(({ theme }) => ({
    display: "grid",
    alignItems: "stretch",
    gridTemplateColumns: "1fr",
    gap: "24px",
    [theme.breakpoints.up("sm")]: {
        gridTemplateColumns: "repeat(2, minmax(180px,1fr))",
    },
    [theme.breakpoints.up("md")]: {
        gridTemplateColumns: "repeat(4, minmax(180px,1fr))",
        gap: "20px",
    },
}));

const ProblemCard = styled(Box)<BoxProps>(({ theme }) => ({
    backgroundColor: "#FFFFFF",
    borderRadius: "16px",
    border: "1px solid #F3F4F6",
    padding: "32px 24px",
    textAlign: "center",
    boxShadow: "0 1px 3px rgba(0, 0, 0, 0.05)",
    transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
    height: "100%",
    display: "flex",
    flexDirection: "column",


    "&:hover": {
        transform: "translateY(-4px)",
        boxShadow: "0 12px 24px rgba(0, 0, 0, 0.1)",
        borderColor: "#E5E7EB"
    },

    [theme.breakpoints.down("sm")]: {
        padding: "24px 20px",
    },
}));


const IconContainer = styled(Box, {
    shouldForwardProp: (prop) => prop !== "bgColor",
})<BoxProps & { bgColor: string }>(({ bgColor }) => ({
    width: "64px",
    height: "64px",
    borderRadius: "50%",
    backgroundColor: bgColor,
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    margin: "0 auto 20px",
}));

const StyledIcon = styled(Box, {
    shouldForwardProp: (prop) => prop !== "iconColor",
})<BoxProps & { iconColor: string }>(({ iconColor }) => ({
    color: iconColor,
    fontSize: "32px",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
}));


const ValueText = styled(Typography, {
    shouldForwardProp: (prop) => prop !== "valueColor",
})<TypographyProps & { valueColor: string }>(({ valueColor }) => ({
    fontSize: "40px",
    fontWeight: 700,
    color: valueColor,
    lineHeight: 1,
    marginBottom: "12px",
}));

const LabelText = styled(Typography)<TypographyProps>({
    fontSize: "16px",
    fontWeight: 600,
    color: "#111827",
    marginBottom: "8px",
    lineHeight: 1.3,

});

const DescriptionText = styled(Typography)<TypographyProps>({
    fontSize: "14px",
    color: "#6B7280",
    lineHeight: 1.5,

});


interface CardProps {
    data: ProblemCardData
}

const Card: React.FC<CardProps> = ({ data }) => {
    const Icon = data.icon;
    return (
        <ProblemCard>
            <IconContainer bgColor={data.iconBgColor}>
                <StyledIcon iconColor={data.valueColor} component={Icon} />
            </IconContainer>

            <ValueText valueColor={data.valueColor}>{data.value}</ValueText>
            <LabelText>{data.label}</LabelText>
            <DescriptionText>{data.description}</DescriptionText>
        </ProblemCard>
    )
}

export const ProblemSection: React.FC = () => {
    return (
        <SectionContainer>
            <ContentWrapper>
                <SectionLabel icon={<WarningAmberIcon />}>THE PROBLEM</SectionLabel>
                <SectionHeader
                    title="Australian SMEs Face Real Compliance Risks"
                    subtitle="Complex award systems and changing regulations make compliance a nightmare"
                />
                <CardsGrid>
                    {PROBLEM_CARDS.map((card) => (
                        <Card key={card.id} data={card} />
                    ))}
                </CardsGrid>
            </ContentWrapper>
        </SectionContainer>
    )
}