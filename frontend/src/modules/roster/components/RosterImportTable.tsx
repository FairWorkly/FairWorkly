import React, { useMemo } from 'react'
import {
  Box,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
  useTheme,
} from '@mui/material'
import ReportProblemOutlinedIcon from '@mui/icons-material/ReportProblemOutlined'

export interface ImportIssue {
  severity: 'error' | 'warning'
  code: string
  message: string
  row: number
  column?: string | null
  value?: string | null
}

export type RawRow = Record<string, unknown>

interface RosterImportTableProps {
  rawRows: RawRow[]
  issues: ImportIssue[]
}

const EXCEL_ROW_KEYS = new Set(['excelRow', 'excel_row'])

const humanizeHeader = (key: string) =>
  key
    .replace(/_/g, ' ')
    .replace(/\b\w/g, letter => letter.toUpperCase())

export const RosterImportTable: React.FC<RosterImportTableProps> = ({
  rawRows,
  issues,
}) => {
  const theme = useTheme()

  const { columns, issueMap, rowIssueMap } = useMemo(() => {
    const firstRow = rawRows[0] ?? {}
    const columns = Object.keys(firstRow).filter(key => !EXCEL_ROW_KEYS.has(key))
    const issueMap = new Map<string, ImportIssue[]>()
    const rowIssueMap = new Map<string | number, ImportIssue[]>()
    for (const issue of issues) {
      if (!issue.row) continue
      if (!issue.column) {
        const list = rowIssueMap.get(issue.row) ?? []
        list.push(issue)
        rowIssueMap.set(issue.row, list)
        continue
      }
      const key = `${issue.row}:${issue.column}`
      const list = issueMap.get(key) ?? []
      list.push(issue)
      issueMap.set(key, list)
    }
    return { columns, issueMap, rowIssueMap }
  }, [rawRows, issues])

  if (!rawRows.length) {
    return (
      <Box>
        <Typography variant="h6" gutterBottom>
          Import Rows
        </Typography>
        <Typography color="text.secondary">No rows to display.</Typography>
      </Box>
    )
  }

  const getRowNumber = (row: RawRow) =>
    (row.excelRow ?? row.excel_row ?? '') as number | string

  const getIssueList = (rowNum: number | string, columnKey?: string) =>
    issueMap.get(`${rowNum}:${columnKey ?? ''}`) ?? []

  const getRowIssues = (rowNum: number | string) =>
    rowIssueMap.get(rowNum) ?? []

  const cellBg = (severity?: 'error' | 'warning') => {
    if (severity === 'error') return theme.palette.error.light
    if (severity === 'warning') return theme.palette.warning.light
    return 'transparent'
  }

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Import Rows
      </Typography>
      <TableContainer component={Paper} variant="outlined">
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell sx={{ fontWeight: 600 }}>Row</TableCell>
              {columns.map(column => (
                <TableCell key={column} sx={{ fontWeight: 600 }}>
                  {humanizeHeader(column)}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {rawRows.map((row, idx) => {
              const rowNum = getRowNumber(row) || idx + 2
              const rowIssues = getRowIssues(rowNum)
              const rowSeverity = rowIssues[0]?.severity
              const rowTooltip = rowIssues
                .map(issue => `[${issue.code}] ${issue.message}`)
                .join('\n')
              return (
                <TableRow key={`${rowNum}-${idx}`}>
                  <TableCell sx={{ whiteSpace: 'nowrap' }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <Box component="span">{rowNum}</Box>
                      {rowIssues.length > 0 && (
                        <Tooltip title={rowTooltip}>
                          <ReportProblemOutlinedIcon
                            fontSize="small"
                            color={rowSeverity === 'error' ? 'error' : 'warning'}
                          />
                        </Tooltip>
                      )}
                    </Box>
                  </TableCell>
                  {columns.map(column => {
                    const issuesForCell = getIssueList(rowNum, column)
                    const severity = issuesForCell[0]?.severity
                    const hasIssue = issuesForCell.length > 0
                    const content = row[column] ?? ''
                    const tooltipText = issuesForCell
                      .map(issue => `[${issue.code}] ${issue.message}`)
                      .join('\n')

                    return (
                      <TableCell
                        key={`${rowNum}-${column}`}
                        sx={{
                          backgroundColor: cellBg(severity),
                          whiteSpace: 'nowrap',
                        }}
                      >
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Box component="span">{String(content)}</Box>
                          {hasIssue && (
                            <Tooltip title={tooltipText}>
                              <ReportProblemOutlinedIcon
                                fontSize="small"
                                color={severity === 'error' ? 'error' : 'warning'}
                              />
                            </Tooltip>
                          )}
                        </Box>
                      </TableCell>
                    )
                  })}
                </TableRow>
              )
            })}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  )
}
