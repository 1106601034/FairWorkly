import React from "react";
import Typography from "@mui/material/Typography";
import { alpha, styled } from "@mui/material/styles";
import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import {
    PaymentsOutlined,
    DescriptionOutlined,
    SentimentDissatisfied,
    Gavel,
} from "@mui/icons-material";
import type { SvgIconProps } from "@mui/material/SvgIcon";
import type { Theme } from "@mui/material/styles";
import { Box, Card } from "@mui/material";


type PaletteKey = keyof Theme["palette"];
type Tone = Extract<
    PaletteKey,
    "primary" | "error" | "warning" | "info"   //取出palette定义好的样式
>;


const WholeSection = styled("section")(({ theme }) => ({
    backgroundColor: theme.palette.background.default,
    padding: theme.spacing(12, 0),
}));

const WholeContentWrapper = styled(Box)(({ theme }) => ({
    maxWidth: 1280,   //theme里没有找到参数
    margin: "0 auto",
    padding: theme.spacing(0, 4),
}));

const SectionHeader = styled("header")(({ theme }) => ({
    textAlign: "center",
    marginBottom: theme.spacing(8),
}));

const SectionLabel = styled(Box)(({ theme }) => ({
    display: "inline-flex",
    alignItems: "center",
    gap: theme.spacing(1),
    padding: theme.spacing(0.75, 2),
    backgroundColor: alpha(theme.palette.primary.main, 0.12),
    color: theme.palette.primary.main,
    borderRadius: theme.shape.borderRadius,
    fontSize: "0.8125rem",
    fontWeight: 600,            //hardcode: theme里没有找到匹配样式
    textTransform: "uppercase",
    letterSpacing: "0.5px",
    marginBottom: theme.spacing(2),
}));

const LabelIcon = styled(Box)({
    fontSize: "inherit",
    lineHeight: 0.2,              //hardcode: theme里没有找到匹配样式
    verticalAlign: "middle",
});

const HeaderContainer = styled("header")(({ theme }) => ({
    marginTop: theme.spacing(3),
}));

const SectionTitle = styled(Typography)(({ theme }) => ({
    marginBottom: theme.spacing(2),
}));

const SubTitle = styled(Typography)(({ theme }) => ({
    margin: "0 auto",
    color: theme.palette.text.secondary,
}));


const CardsLayout = styled(Box)(({ theme }) => ({
    display: "grid",
    gap: theme.spacing(3),
    [theme.breakpoints.up("sm")]: {
        gridTemplateColumns: "repeat(2,  1fr)",
    },
    [theme.breakpoints.up("md")]: {
        gridTemplateColumns: "repeat(4,  1fr)",
    },
}));


const ProblemCardContainer = styled(Card)(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
    border: `1px solid ${theme.palette.divider}`,
    padding: theme.spacing(4),
    transition: "all 0.3s ease",   //hardcode: theme里没有找到匹配的
    textAlign: "center",

    "&:hover": {
        transform: "translateY(-8px)",
        boxShadow: theme.shadows[4],
    },
}));

const CardIconContainer = styled(Box)<{ tone: Tone }>(({ theme, tone }) => ({
    width: theme.spacing(7),
    height: theme.spacing(7),
    borderRadius: theme.spacing(1.75),
    backgroundColor: alpha(theme.palette[tone].main, 0.1),
    color: theme.palette[tone].main,

    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    margin: `0 auto ${theme.spacing(2)}`,

    "& svg": {
        fontSize: "1.75rem",  //hardcode: icon大小theme里没有找到匹配
    },
}));

const CardValueTitle = styled(Typography)<{ tone: Tone }>(({ theme, tone }) => ({
    color: theme.palette[tone].main,
    marginBottom: theme.spacing(1),
}));

const CardTextTitle = styled(Typography)(({ theme }) => ({
    fontWeight: 600,              //hardcode: theme里没有找到匹配
    marginBottom: theme.spacing(1),
}));

const CardDescriptionText = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,
}));


interface CardData {
    id: string;
    icon: React.ComponentType<SvgIconProps>;
    value: string;
    label: string;
    description: string;
    tone: Tone;
}

function ProblemCard({ data }: { data: CardData }) {
    const { tone, icon: IconComponent, label, value, description } = data;
    return (
        <ProblemCardContainer>
            <CardIconContainer tone={tone} aria-hidden="true">
                <IconComponent />
            </CardIconContainer>

            <CardValueTitle variant="h2" tone={tone}>{value}</CardValueTitle>
            <CardTextTitle variant="body1" >{label}</CardTextTitle>
            <CardDescriptionText variant="body2">{description}</CardDescriptionText>
        </ProblemCardContainer>
    );
}



export const ProblemSection: React.FC = () => {

    const content = {
        label: "THE PROBLEM",
        title: "Australian SMEs Face Real Compliance Risks",
        subtitle:
            "Complex award systems and changing regulations make compliance a nightmare",
    };

    const cards: CardData[] = [
        {
            id: "wage-underpayments",
            icon: PaymentsOutlined,
            value: "$1.35B",
            label: "Wage Underpayments",
            description: "Recovered by Fair Work in 2023-24",
            tone: "error",
        },
        {
            id: "modern-awards",
            icon: DescriptionOutlined,
            value: "122",
            label: "Modern Awards",
            description: "Complex rules and variations",
            tone: "warning",
        },
        {
            id: "smes-not-confident",
            icon: SentimentDissatisfied,
            value: "73%",
            label: "SMEs Not Confident",
            description: "Too complex to DIY",
            tone: "primary",
        },
        {
            id: "fwo-recovery",
            icon: Gavel,
            value: "$532M",
            label: "FWO Recovery",
            description: "Enforcement intensifying",
            tone: "info",
        },
    ];

    return (
        <WholeSection>
            <WholeContentWrapper>
                <SectionHeader>
                    <SectionLabel>
                        <LabelIcon aria-hidden>
                            <WarningAmberIcon fontSize="inherit" />
                        </LabelIcon>
                        {content.label}
                    </SectionLabel>

                    <HeaderContainer>
                        <SectionTitle variant="h2" >{content.title}</SectionTitle>
                        <SubTitle variant="body1">{content.subtitle}</SubTitle>
                    </HeaderContainer>
                </SectionHeader>

                <CardsLayout>
                    {cards.map((card) => (
                        <ProblemCard key={card.id} data={card} />
                    ))}
                </CardsLayout>

            </WholeContentWrapper>
        </WholeSection>
    );
};
