import { Box, Card, Button, styled, Typography, alpha } from "@mui/material";
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
    ArrowForward,
} from "@mui/icons-material";
import { useNavigate } from "react-router-dom";



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
    fontWeight: 600,   //hardcode 
    color: theme.palette.text.primary,
}));


const AnswerText = styled(Typography)(({ theme }) => ({
    color: theme.palette.text.secondary,

    '& strong': {
        color: theme.palette.text.primary,
        fontWeight: 600,   //hardcode
    },
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


const TemplateButton = styled(Button)(({ theme }) => ({
    marginTop: theme.spacing(3),
    padding: theme.spacing(1.5, 3),
    backgroundColor: alpha(theme.palette.primary.main, 0.12),
    color: theme.palette.primary.main,
    fontWeight: 600,
    borderRadius: theme.fairworkly.radius.md,
    textTransform: 'none',
    transition: theme.transitions.create(['all'], {
        duration: theme.transitions.duration.short,
    }),

    '&:hover': {
        backgroundColor: alpha(theme.palette.primary.main, 0.18),
        transform: 'translateY(-2px)',
    },

    '& .MuiButton-endIcon': {
        marginLeft: theme.spacing(1),
    },
}));



interface FaqItem {
    id: string;
    icon: React.ComponentType;
    question: string;
    answer: string | React.ReactNode;
    note?: string;
    hasButton?: boolean;
}


export const FaqSection: React.FC = () => {
    const navigate = useNavigate();

    const content = {
        label: 'FAQ',
        title: 'Frequently Asked Questions',
        subtitle: 'Everything you need to know about FairWorkly',
    };


    const faqs: FaqItem[] = [
        {
            id: 'systems',
            icon: ComputerOutlined,
            question: 'What systems does FairWorkly work with?',
            answer: (
                <>
                    <strong>FairWorkly doesn't require integration.</strong> We accept standard CSV files, which means you can use any payroll or rostering system that can export CSV — including Xero, MYOB, QuickBooks, Keypay, Deputy, Tanda, and many others.
                </>
            ),
            note: '💡 Simply export your data as CSV and upload. No API keys, no IT setup required.',
        },
        {
            id: 'rostering',
            icon: EventOutlined,
            question: 'Do I need rostering software?',
            answer: (
                <>
                    No! While we work great with Deputy and Tanda exports, you can also upload roster data from Excel. We provide a <strong>free template</strong> that makes it easy to convert your existing schedules to CSV format.
                </>
            ),
        },
        {
            id: 'awards',
            icon: VerifiedOutlined,
            question: 'Which Awards do you cover?',
            answer: (
                <>
                    We currently support the three most common Awards:
                    <br />• <strong>Hospitality Industry Award 2020</strong>
                    <br />• <strong>General Retail Industry Award</strong>
                    <br />• <strong>Clerks Private Sector Award</strong>
                </>
            ),
        },
        {
            id: 'validation-time',
            icon: ScheduleOutlined,
            question: 'How long does validation take?',
            answer: (
                <>
                    Most validations complete in <strong>2-3 minutes</strong>. Upload your CSV, grab a coffee, and your compliance report will be ready. Larger files (100+ employees) may take up to 5 minutes.
                </>
            ),
        },
        {
            id: 'contracts',
            icon: DescriptionOutlined,
            question: 'Do you generate employment contracts?',
            answer: (
                <>
                    No, we don't generate documents in the MVP. Our <strong>Document Agent</strong> helps you <strong>track mandatory documents</strong> like Fair Work Information Statement (FWIS), Separation Certificates, and Casual Conversion Notices — ensuring you don't miss critical deadlines.
                </>
            ),
        },
        {
            id: 'issues',
            icon: BuildOutlined,
            question: 'What if I find issues in my payroll?',
            answer: (
                <>
                    We provide <strong>detailed reports</strong> with expected vs actual values and step-by-step recommendations. You can then fix issues in your payroll system (Xero/MYOB/etc) and re-upload to verify.
                </>
            ),
            note: '📌 For complex issues, we recommend consulting the Fair Work Ombudsman or an employment lawyer.',
        },
        {
            id: 'legal',
            icon: GavelOutlined,
            question: 'Is this a substitute for legal advice?',
            answer: (
                <>
                    <strong>No.</strong> FairWorkly is a compliance validation tool, not legal advice. We check your payroll and rosters against Fair Work rules and highlight potential issues. For complex situations or disputes, consult the Fair Work Ombudsman or an employment lawyer.
                </>
            ),
        },
        {
            id: 'templates',
            icon: CloudDownloadOutlined,
            question: 'Do you provide CSV templates?',
            answer: (
                <>
                    Yes! We provide <strong>CSV templates</strong> for both payslip and roster data to help you get started with FairWorkly.
                </>
            ),
            hasButton: true,
        },
    ];

    const handleTemplateClick = () => {
        navigate('/templates');
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
                    {faqs.map((faq) => {
                        const IconComponent = faq.icon;
                        return (
                            <FaqCard key={faq.id} elevation={0}>
                                <QuestionContainer>
                                    <QuestionIcon aria-hidden="true">
                                        <IconComponent />
                                    </QuestionIcon>
                                    <QuestionText variant="h6">{faq.question}</QuestionText>
                                </QuestionContainer>

                                <AnswerText variant="body1">{faq.answer}</AnswerText>

                                {faq.note && (
                                    <NoteBox>
                                        <NoteText>{faq.note}</NoteText>
                                    </NoteBox>
                                )}

                                {faq.hasButton && (
                                    <TemplateButton
                                        fullWidth
                                        endIcon={<ArrowForward />}
                                        onClick={handleTemplateClick}
                                    >
                                        View All CSV Templates
                                    </TemplateButton>
                                )}
                            </FaqCard>
                        );
                    })}
                </FaqLayout>
            </ContentContainer>
        </PageSection>
    );
};