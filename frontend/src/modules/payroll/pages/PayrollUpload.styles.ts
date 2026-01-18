import { styled } from '@/styles/styled';
import { Box, Paper, Button, Card, TableCell, IconButton, Breadcrumbs, Alert, Typography } from '@mui/material';

// Page container
export const PageContainer = styled(Box)({
  minHeight: '100vh',
  backgroundColor: '#f8fafc',
  padding: '32px'
});

// Breadcrumbs
export const StyledBreadcrumbs = styled(Breadcrumbs)({
  marginBottom: '16px'
});

// Page title
export const PageTitle = styled(Box)({
  fontSize: '1.875rem',
  fontWeight: 700,
  marginBottom: '24px'
});

// Error alert
export const StyledAlert = styled(Alert)({
  marginBottom: '24px'
});

// Upload zone
export const UploadZone = styled(Paper)<{ $isDragging?: boolean }>(({ $isDragging }) => ({
  marginBottom: '24px',
  padding: '48px',
  border: '2px dashed',
  borderColor: $isDragging ? '#6366f1' : '#e2e8f0',
  borderRadius: '16px',
  backgroundColor: $isDragging ? 'rgba(99, 102, 241, 0.05)' : 'white',
  textAlign: 'center',
  cursor: 'pointer',
  transition: 'all 0.2s',
  '&:hover': {
    borderColor: '#6366f1',
    backgroundColor: 'rgba(99, 102, 241, 0.05)'
  }
}));

// Upload icon box
export const UploadIconBox = styled(Box)({
  width: '80px',
  height: '80px',
  margin: '0 auto 24px',
  borderRadius: '16px',
  background: 'linear-gradient(135deg, #7c3aed, #ec4899)',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center'
});

// Upload button
export const UploadButton = styled(Button)({
  marginBottom: '16px',
  background: 'linear-gradient(135deg, #7c3aed, #ec4899)',
  '&:hover': {
    background: 'linear-gradient(135deg, #6d28d9, #db2777)',
  }
});

// Download template button
export const TemplateButton = styled(Button)({
  color: '#6366f1'
});

// Configuration card
export const ConfigCard = styled(Card)({
  marginBottom: '24px'
});

// Award option
export const AwardOption = styled(Paper)<{ $selected?: boolean; $color?: string }>(
  ({ $selected, $color = '#6366f1' }) => ({
    padding: '24px',
    textAlign: 'center',
    cursor: 'pointer',
    border: '2px solid',
    borderColor: $selected ? $color : '#e2e8f0',
    backgroundColor: $selected ? `${$color}15` : 'white',
    transition: 'all 0.2s',
    '&:hover': {
      borderColor: $color,
      backgroundColor: `${$color}15`
    }
  })
);

// Award icon box
export const AwardIconBox = styled(Box)<{ $selected?: boolean; $color?: string }>(
  ({ $selected, $color = '#6366f1' }) => ({
    width: '60px',
    height: '60px',
    margin: '0 auto 16px',
    borderRadius: '8px',
    backgroundColor: $selected ? $color : `${$color}20`,
    color: $selected ? 'white' : $color,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center'
  })
);

// Validation item
export const ValidationItem = styled(Paper)({
  padding: '16px',
  display: 'flex',
  alignItems: 'center',
  gap: '16px',
  backgroundColor: '#f8fafc',
  border: '1px solid #e2e8f0'
});

// Files list card
export const FilesCard = styled(Card)({
  marginBottom: '24px'
});

// Table header cell
export const StyledTableCell = styled(TableCell)({
  fontWeight: 600,
  backgroundColor: '#f8fafc'
});

// Delete button
export const DeleteButton = styled(IconButton)({
  backgroundColor: 'rgba(239, 68, 68, 0.1)',
  color: '#ef4444',
  '&:hover': {
    backgroundColor: 'rgba(239, 68, 68, 0.2)'
  }
});

// Actions container
export const ActionsContainer = styled(Box)({
  display: 'flex',
  justifyContent: 'center',
  gap: '16px',
  marginTop: '32px'
});

// Cancel button
export const CancelButton = styled(Button)({
  padding: '12px 32px'
});

// Validate button
export const ValidateButton = styled(Button)({
  padding: '12px 32px',
  background: 'linear-gradient(135deg, #6366f1, #ec4899)',
  '&:hover': {
    background: 'linear-gradient(135deg, #4f46e5, #db2777)',
  }
});

// Section title (with icon)
export const SectionTitle = styled(Typography)({
  display: 'flex',
  alignItems: 'center',
  gap: '8px',
  fontWeight: 700,
  marginBottom: '24px'
});

// Section subtitle
export const SectionSubtitle = styled(Typography)({
  fontWeight: 600,
  marginBottom: '16px'
});

// Grid container (replaces MUI Grid)
export const GridContainer = styled(Box)({
  display: 'grid',
  gridTemplateColumns: 'repeat(3, 1fr)',
  gap: '16px',
  marginBottom: '24px'
});

// Form grid (3 columns)
export const FormGrid = styled(Box)({
  display: 'grid',
  gridTemplateColumns: 'repeat(3, 1fr)',
  gap: '16px',
  marginBottom: '24px'
});

// Validation list container
export const ValidationList = styled(Box)({
  display: 'flex',
  flexDirection: 'column',
  gap: '12px'
});

// Typography variants
export const UploadTitle = styled(Typography)({
  fontSize: '1.125rem',
  fontWeight: 600,
  marginBottom: '8px'
});

export const UploadDescription = styled(Typography)({
  color: '#64748b',
  marginBottom: '24px'
});

export const FileTypeHint = styled(Typography)({
  fontSize: '0.875rem',
  color: '#64748b',
  marginTop: '16px',
  marginBottom: '8px'
});

export const ValidationDescription = styled(Typography)({
  fontSize: '0.875rem',
  color: '#64748b',
  marginBottom: '16px'
});

export const ValidationItemText = styled(Typography)({
  fontSize: '0.875rem',
  fontWeight: 500
});

export const FileNameText = styled(Typography)({
  fontWeight: 600
});

export const FileSizeText = styled(Typography)({
  fontSize: '0.875rem',
  color: '#64748b'
});

export const FileDateText = styled(Typography)({
  fontSize: '0.875rem',
  color: '#64748b'
});

export const AwardTitle = styled(Typography)({
  fontWeight: 600,
  fontSize: '0.875rem'
});

export const AwardDescription = styled(Typography)({
  fontSize: '0.75rem',
  color: '#64748b'
});

// Generic icon wrapper
export const StyledIcon = styled(Box)({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  fontSize: '40px',
  color: 'inherit'
});