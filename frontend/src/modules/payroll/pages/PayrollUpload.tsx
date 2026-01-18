import React, { useState, useRef } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  TextField,
  MenuItem,
  CardContent,
  Link
} from '@mui/material';
import {
  CloudUpload,
  FolderOpen,
  Download,
  CheckCircle,
  Close,
  Delete,
  Tune,
  Storefront,
  Restaurant,
  BusinessCenter,
  ChevronRight
} from '@mui/icons-material';
import * as S from './PayrollUpload.styles';

interface UploadedFile {
  id: number;
  name: string;
  size: string;
  date: string;
  status: 'ready' | 'validating' | 'validated';
}

const awards = [
  { 
    id: 'retail', 
    icon: Storefront,
    title: 'Retail Award', 
    desc: 'General retail industry',
    color: '#6366f1'
  },
  { 
    id: 'hospitality', 
    icon: Restaurant,
    title: 'Hospitality Award', 
    desc: 'Cafes, restaurants, hotels',
    color: '#ec4899'
  },
  { 
    id: 'clerks', 
    icon: BusinessCenter,
    title: 'Clerks Award', 
    desc: 'Office & admin roles',
    color: '#06b6d4'
  }
];

const validationItems = [
  'Base rates & award classifications',
  'Penalty rates (weekends & public holidays)',
  'Casual loading (25%)',
  'Superannuation guarantee (12%)',
  'Single Touch Payroll (STP) compliance'
];

export function PayrollUpload() {
  const [uploadedFiles, setUploadedFiles] = useState<UploadedFile[]>([]);
  const [selectedAward, setSelectedAward] = useState('retail');
  const [isDragging, setIsDragging] = useState(false);
  const [error, setError] = useState('');
  const fileInputRef = useRef<HTMLInputElement>(null);

  const formatFileSize = (bytes: number): string => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  };

  const validateFile = (file: File): string | null => {
    if (!file.name.toLowerCase().endsWith('.csv')) {
      return 'Only CSV files are supported';
    }
    
    const MAX_SIZE = 50 * 1024 * 1024;
    if (file.size > MAX_SIZE) {
      return 'File size must be less than 50MB';
    }
    
    if (file.size === 0) {
      return 'File is empty';
    }
    
    return null;
  };

  const handleFileUpload = (file: File) => {
    const validationError = validateFile(file);
    if (validationError) {
      setError(validationError);
      setTimeout(() => setError(''), 5000);
      return;
    }

    const fileData: UploadedFile = {
      id: Date.now(),
      name: file.name,
      size: formatFileSize(file.size),
      date: new Date().toLocaleString('en-AU', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      }),
      status: 'ready'
    };

    setUploadedFiles([...uploadedFiles, fileData]);
    setError('');
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(true);
  };

  const handleDragLeave = () => {
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
    
    const files = e.dataTransfer.files;
    if (files.length > 0) {
      handleFileUpload(files[0]);
    }
  };

  const deleteFile = (id: number) => {
    setUploadedFiles(uploadedFiles.filter(file => file.id !== id));
  };

  const downloadTemplate = () => {
    const csv = `Employee ID,Name,Hours Worked,Hourly Rate,Award Level\n001,John Smith,38,25.50,Level 3\n002,Jane Doe,40,28.00,Level 4`;
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'payroll_template.csv';
    a.click();
    URL.revokeObjectURL(url);
  };

  const handleValidate = () => {
    setUploadedFiles(uploadedFiles.map(file => ({ ...file, status: 'validating' })));
    setTimeout(() => {
      alert('Validation started!');
    }, 500);
  };

  return (
    <S.PageContainer>
      {/* Breadcrumbs */}
      <S.StyledBreadcrumbs separator={<ChevronRight fontSize="small" />}>
        <Link underline="hover" color="inherit" href="#">
          Home
        </Link>
        <Link underline="hover" color="text.primary" href="#">
          Verify Payroll
        </Link>
      </S.StyledBreadcrumbs>

      {/* Page title */}
      <S.PageTitle>Upload Payroll</S.PageTitle>

      {/* Error alert */}
      {error && (
        <S.StyledAlert severity="error" onClose={() => setError('')}>
          {error}
        </S.StyledAlert>
      )}

      {/* Upload zone */}
      <S.UploadZone
        elevation={0}
        $isDragging={isDragging}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        onClick={() => fileInputRef.current?.click()}
      >
        <S.UploadIconBox>
          <CloudUpload style={{ fontSize: 40, color: 'white' }} />
        </S.UploadIconBox>

        <S.UploadTitle>
          Drag and drop your payroll file
        </S.UploadTitle>
        
        <S.UploadDescription>
          or click to browse from your computer
        </S.UploadDescription>

        <S.UploadButton
          variant="contained"
          startIcon={<FolderOpen />}
        >
          Choose File
        </S.UploadButton>

        <S.FileTypeHint>
          Supported: CSV only · Max size: 50MB · Export from Xero or MYOB
        </S.FileTypeHint>

        <S.TemplateButton
          size="small"
          startIcon={<Download />}
          onClick={(e) => {
            e.stopPropagation();
            downloadTemplate();
          }}
        >
          Download CSV template
        </S.TemplateButton>

        <input
          ref={fileInputRef}
          type="file"
          accept=".csv"
          style={{ display: 'none' }}
          onChange={(e) => {
            if (e.target.files?.[0]) {
              handleFileUpload(e.target.files[0]);
            }
          }}
        />
      </S.UploadZone>

      {/* Configuration card */}
      <S.ConfigCard>
        <CardContent>
          <S.SectionTitle variant="h6">
            <Tune />
            Configuration
          </S.SectionTitle>

          <S.SectionSubtitle variant="subtitle2">
            Select Award
          </S.SectionSubtitle>
          
          <S.GridContainer>
            {awards.map((award) => (
              <S.AwardOption
                key={award.id}
                elevation={0}
                onClick={() => setSelectedAward(award.id)}
                $selected={selectedAward === award.id}
                $color={award.color}
              >
                <S.AwardIconBox
                  $selected={selectedAward === award.id}
                  $color={award.color}
                >
                  <award.icon style={{ fontSize: 40 }} />
                </S.AwardIconBox>
                <S.AwardTitle>
                  {award.title}
                </S.AwardTitle>
                <S.AwardDescription>
                  {award.desc}
                </S.AwardDescription>
              </S.AwardOption>
            ))}
          </S.GridContainer>

          <S.FormGrid>
            <TextField
              select
              fullWidth
              label="Pay Period"
              defaultValue="Weekly"
              size="small"
            >
              <MenuItem value="Weekly">Weekly</MenuItem>
              <MenuItem value="Fortnightly">Fortnightly</MenuItem>
              <MenuItem value="Monthly">Monthly</MenuItem>
            </TextField>
            
            <TextField
              fullWidth
              type="date"
              label="Week Starting"
              InputLabelProps={{ shrink: true }}
              size="small"
            />
            
            <TextField
              fullWidth
              type="date"
              label="Week Ending"
              InputLabelProps={{ shrink: true }}
              size="small"
            />
          </S.FormGrid>

          <S.SectionSubtitle variant="subtitle2">
            Validation Coverage
          </S.SectionSubtitle>
          
          <S.ValidationDescription>
            This validation will check all 5 compliance areas
          </S.ValidationDescription>

          <S.ValidationList>
            {validationItems.map((item, index) => (
              <S.ValidationItem key={index} elevation={0}>
                <CheckCircle style={{ color: '#10b981', fontSize: 24 }} />
                <S.ValidationItemText>
                  {item}
                </S.ValidationItemText>
              </S.ValidationItem>
            ))}
          </S.ValidationList>
        </CardContent>
      </S.ConfigCard>

      {/* Uploaded files list */}
      {uploadedFiles.length > 0 && (
        <S.FilesCard>
          <CardContent>
            <S.SectionTitle variant="h6">
              <FolderOpen />
              Uploaded Files
            </S.SectionTitle>

            <TableContainer>
              <Table>
                <TableHead>
                  <TableRow>
                    <S.StyledTableCell>File Name</S.StyledTableCell>
                    <S.StyledTableCell>Size</S.StyledTableCell>
                    <S.StyledTableCell>Upload Date</S.StyledTableCell>
                    <S.StyledTableCell>Status</S.StyledTableCell>
                    <S.StyledTableCell>Actions</S.StyledTableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {uploadedFiles.map((file) => (
                    <TableRow key={file.id} hover>
                      <TableCell>
                        <S.FileNameText>{file.name}</S.FileNameText>
                      </TableCell>
                      <TableCell>
                        <S.FileSizeText>{file.size}</S.FileSizeText>
                      </TableCell>
                      <TableCell>
                        <S.FileDateText>{file.date}</S.FileDateText>
                      </TableCell>
                      <TableCell>
                        <Chip
                          label={file.status.charAt(0).toUpperCase() + file.status.slice(1)}
                          size="small"
                          color={
                            file.status === 'ready' ? 'success' :
                            file.status === 'validating' ? 'warning' : 'primary'
                          }
                        />
                      </TableCell>
                      <TableCell>
                        <S.DeleteButton
                          size="small"
                          onClick={() => deleteFile(file.id)}
                        >
                          <Delete fontSize="small" />
                        </S.DeleteButton>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </CardContent>
        </S.FilesCard>
      )}

      {/* Action buttons */}
      <S.ActionsContainer>
        <S.CancelButton
          variant="outlined"
          size="large"
          startIcon={<Close />}
        >
          Cancel
        </S.CancelButton>
        <S.ValidateButton
          variant="contained"
          size="large"
          startIcon={<CheckCircle />}
          disabled={uploadedFiles.length === 0}
          onClick={handleValidate}
        >
          Validate Payroll
        </S.ValidateButton>
      </S.ActionsContainer>
    </S.PageContainer>
  );
}