import { Box, Card, styled, Typography, alpha } from "@mui/material";
import {
    HelpOutlineOutlined,
    ComputerOutlined,
    EventOutlined,
    VerifiedOutlined,
    ScheduleOutlined,
    DescriptionOutlined,
    BuildOutlined,
    GavelOutlined,
    CloudDownloadOutlined,
} from "@mui/icons-material";
import React from "react";



const PageSection = styled(Box)(({ theme }) => ({
    backgroundColor: theme.palette.background.default,
    padding: theme.spacing(12, 0),
}));

const ContentContainer = styled(Box)(({ theme }) => ({
    margin: '0 auto',
    padding: theme.spacing(0, 4),
}));

const HeaderContainer = styled(Box)(({ theme }) => ({
    textAlign: 'center',
    marginBottom: theme.spacing(8),
}));

const SectionLabel = styled(Box)(({ theme }) => ({
    display: 'inline-flex',
    alignItems: 'center',
    gap: theme.spacing(1),
    padding: theme.spacing(0.75, 2),
    backgroundColor: alpha(theme.palette.primary.main, 0.12),
    color: theme.palette.primary.main,
    borderRadius: theme.shape.borderRadius,
    marginBottom: theme.spacing(2),
    '& .MuiSvgIcon-root': {
        fontSize: theme.spacing(2),
    }
}));


const SectionTitle = styled(Typography)(({ theme }) => ({
    marginBottom: theme.spacing(2),
}));

const SectionSubTitle = styled(Typography)(({ theme }) => ({
    margin: '0 auto',
    color: theme.palette.text.secondary,
}));


const FaqLayout = styled(Box)(({ theme }) => ({
    display: 'grid',
    gridTemplateColumns: '1fr',
    gap: theme.spacing(4),

    [theme.breakpoints.up('md')]: {
        gridTemplateColumns: 'repeat(2, 1fr)',
    },
}));

const FaqCard = styled(Card)(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
    borderRadius: theme.fairworkly.radius.lg,
    padding: theme.spacing(4),
    border: `1px solid ${theme.palette.divider}`,
    transition: theme.transitions.create(['border-color', 'box-shadow'], {
        duration: theme.transitions.duration.standard,
    }),

    '&:hover': {
        borderColor: theme.palette.primary.main,
        boxShadow: theme.shadows[2],
    },
}));


const QuestionContainer = styled(Box)(({ theme }) => ({
    display: 'flex',
    alignItems: 'flex-start',
    gap: theme.spacing(1.5),
    marginBottom: theme.spacing(2),
}));

const QuestionIcon = styled(Box)(({ theme }) => ({
    color: theme.palette.primary.main,
    display: 'flex',
    alignItems: 'center',
    flexShrink: 0,

    '& svg': {
        fontSize: theme.spacing(3),
    },
}));

const QuestionText = styled(Typography)(({ theme }) => ({
    fontWeight: theme.typography.fontWeightSemiBold,
    color: theme.palette.text.primary,
}));


const AnswerText = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,

}));

const BoldText = styled('span')(({ theme }) => ({
    color: theme.palette.text.primary,
    fontWeight: theme.typography.fontWeightSemiBold,
}));

const NoteBox = styled(Box)(({ theme }) => ({
    backgroundColor: alpha(theme.palette.primary.main, 0.05),
    padding: theme.spacing(2),
    borderRadius: theme.fairworkly.radius.md,
    borderLeft: `3px solid ${theme.palette.primary.main}`,
    marginTop: theme.spacing(2),
}));

const NoteText = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,
    fontSize: theme.typography.body2.fontSize,
}));



const ListContainer = styled(Box)(({ theme }) => ({
    marginTop: theme.spacing(1),
}));

const ListItemText = styled(AnswerText)(({ theme }) => ({
    marginTop: theme.spacing(0.5),
}));

const parseTextWithBold = (text: string): React.ReactNode => {
    const parts = text.split(/(\*\*.*?\*\*)/g);

    return parts.map((part, index) => {
        if (part.startsWith('**') && part.endsWith('**')) {
            return <BoldText key={index}>{part.slice(2, -2)}</BoldText>;
        }
        return <React.Fragment key={index}>{part}</React.Fragment>;
    });
};

interface FaqMessage {
    question: string;
    answer: string;
    note?: string;
    list?: string[];

}

const FAQ_MESSAGES: Record<string, FaqMessage> = {
    systems: {
        question: 'What systems does FairWorkly work with?',
        answer: "**FairWorkly doesn't require integration.** We accept **CSV** for payroll and **XLSX** for rosters, which means you can use any payroll or rostering system that can export these formats — including Xero, MYOB, QuickBooks, Keypay, Deputy, Tanda, and many others.",
        note: '💡 Simply export your data as CSV or XLSX and upload. No API keys, no IT setup required.',
    },
    rostering: {
        question: 'Do I need rostering software?',
        answer: "No! While we work great with Deputy and Tanda exports, you can also upload roster data from Excel. We provide a **free template** that makes it easy to convert your existing schedules to XLSX format.",
    },
    awards: {
        question: 'Which Awards do you cover?',
        answer: 'We currently support the three most common Awards:',
        list: [
            'Hospitality Industry (General) Award 2020',
            'General Retail Industry Award 2020',
            'Clerks\u2014Private Sector Award 2020',
        ],
    },
    validationTime: {
        question: 'How long does validation take?',
        answer: "Most validations complete in **2-3 minutes**. Upload your file, grab a coffee, and your compliance report will be ready. Larger files (100+ employees) may take up to 5 minutes.",
    },
    contracts: {
        question: 'Do you generate employment contracts?',
        answer: "No, we don't generate documents in the MVP. Our **Document Agent** helps you **track mandatory documents** like Fair Work Information Statement (FWIS), Separation Certificates, and Casual Conversion Notices — ensuring you don't miss critical deadlines.",
    },
    issues: {
        question: 'What if I find issues in my payroll?',
        answer: 'We provide **detailed reports** with expected vs actual values and step-by-step recommendations. You can then fix issues in your payroll system (Xero/MYOB/etc) and re-upload to verify.',
        note: '📌 For complex issues, we recommend consulting the Fair Work Ombudsman or an employment lawyer.',
    },
    legal: {
        question: 'Is this a substitute for legal advice?',
        answer: '**No.** FairWorkly is a compliance validation tool, not legal advice. We check your payroll and rosters against Fair Work rules and highlight potential issues. For complex situations or disputes, consult the Fair Work Ombudsman or an employment lawyer.',
    },
    templates: {
        question: 'Do you provide file templates?',
        answer: 'Yes! We provide **CSV templates** for payslip data and **XLSX templates** for roster data to help you get started with FairWorkly.',
    },
};

interface FaqConfig {
    id: keyof typeof FAQ_MESSAGES;
    icon: React.ComponentType;
}

const FAQ_CONFIGS: FaqConfig[] = [
    { id: 'systems', icon: ComputerOutlined },
    { id: 'rostering', icon: EventOutlined },
    { id: 'awards', icon: VerifiedOutlined },
    { id: 'validationTime', icon: ScheduleOutlined },
    { id: 'contracts', icon: DescriptionOutlined },
    { id: 'issues', icon: BuildOutlined },
    { id: 'legal', icon: GavelOutlined },
    { id: 'templates', icon: CloudDownloadOutlined },
];



export const FaqSection: React.FC = () => {

    const content = {
        label: 'FAQ',
        title: 'Frequently Asked Questions',
        subtitle: 'Everything you need to know about FairWorkly',
    };


    return (
        <PageSection id="faq">
            <ContentContainer>
                <HeaderContainer>
                    <SectionLabel>
                        <HelpOutlineOutlined fontSize="inherit" />
                        {content.label}
                    </SectionLabel>
                    <SectionTitle variant="h2">{content.title}</SectionTitle>
                    <SectionSubTitle variant="h5">{content.subtitle}</SectionSubTitle>
                </HeaderContainer>

                <FaqLayout>
                    {FAQ_CONFIGS.map((config: FaqConfig) => {
                        const message = FAQ_MESSAGES[config.id];
                        const IconComponent = config.icon;
                        return (
                            <FaqCard key={config.id} elevation={0}>
                                <QuestionContainer>
                                    <QuestionIcon aria-hidden="true">
                                        <IconComponent />
                                    </QuestionIcon>
                                    <QuestionText variant="h6">{message.question}</QuestionText>
                                </QuestionContainer>
                                <AnswerText variant="body1">
                                    {parseTextWithBold(message.answer)}
                                </AnswerText>

                                {message.list && (
                                    <ListContainer>
                                        {message.list.map((item) => (
                                            <ListItemText key={item} variant="body1">
                                                • <BoldText>{item}</BoldText>
                                            </ListItemText>
                                        ))}
                                    </ListContainer>
                                )}
                                {message.note && (
                                    <NoteBox>
                                        <NoteText>{message.note}</NoteText>
                                    </NoteBox>
                                )}

                            </FaqCard>
                        );
                    })}
                </FaqLayout>
            </ContentContainer>
        </PageSection>
    );
};
