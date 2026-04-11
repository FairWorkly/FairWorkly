import { useState } from 'react'
import Button from '@mui/material/Button'
import TextField from '@mui/material/TextField'
import Typography from '@mui/material/Typography'
import PlayArrowIcon from '@mui/icons-material/PlayArrow'
import { styled } from '@/styles/styled'
import type { ShiftScenario } from '../../types/debate.types'

interface DebateScenarioFormProps {
  onSubmit: (scenario: ShiftScenario) => void
  isLoading: boolean
}

const FormGrid = styled('form')(({ theme }) => ({
  display: 'grid',
  gridTemplateColumns: '1fr 1fr',
  gap: theme.spacing(2),
  [theme.breakpoints.down('sm')]: {
    gridTemplateColumns: '1fr',
  },
}))

const FullWidth = styled('div')({
  gridColumn: '1 / -1',
})

const DEFAULT_SCENARIO: ShiftScenario = {
  employee_name: 'Alice',
  shift_date: '2024-03-16 (Saturday)',
  shift_hours: 10,
  week_hours_before_shift: 38,
  award_name: 'Hospitality Industry (General) Award 2020',
  extra_context: 'Full-time employee',
}

export const DebateScenarioForm = ({
  onSubmit,
  isLoading,
}: DebateScenarioFormProps) => {
  const [form, setForm] = useState<ShiftScenario>(DEFAULT_SCENARIO)

  const update = (field: keyof ShiftScenario, value: string | number) =>
    setForm(prev => ({ ...prev, [field]: value }))

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onSubmit(form)
  }

  return (
    <FormGrid onSubmit={handleSubmit}>
      <TextField
        label="Employee Name"
        size="small"
        value={form.employee_name}
        onChange={e => update('employee_name', e.target.value)}
        required
      />
      <TextField
        label="Shift Date"
        size="small"
        value={form.shift_date}
        onChange={e => update('shift_date', e.target.value)}
        required
        helperText="e.g. 2024-03-16 (Saturday)"
      />
      <TextField
        label="Shift Hours"
        size="small"
        type="number"
        value={form.shift_hours}
        onChange={e => update('shift_hours', Number(e.target.value))}
        required
        inputProps={{ min: 0, max: 24, step: 0.5 }}
      />
      <TextField
        label="Week Hours Before Shift"
        size="small"
        type="number"
        value={form.week_hours_before_shift}
        onChange={e =>
          update('week_hours_before_shift', Number(e.target.value))
        }
        required
        inputProps={{ min: 0, max: 100, step: 0.5 }}
        helperText="Hours already worked Mon-Fri"
      />
      <FullWidth>
        <TextField
          label="Applicable Award"
          size="small"
          fullWidth
          value={form.award_name}
          onChange={e => update('award_name', e.target.value)}
          required
        />
      </FullWidth>
      <FullWidth>
        <TextField
          label="Additional Context (optional)"
          size="small"
          fullWidth
          value={form.extra_context ?? ''}
          onChange={e => update('extra_context', e.target.value)}
        />
      </FullWidth>
      <FullWidth>
        <Button
          type="submit"
          variant="contained"
          disabled={isLoading}
          startIcon={<PlayArrowIcon />}
          sx={{ mt: 1 }}
        >
          {isLoading ? 'Agents are debating...' : 'Start Agent Debate'}
        </Button>
        {isLoading && (
          <Typography variant="caption" color="text.secondary" sx={{ ml: 2 }}>
            Three agents are reviewing the scenario — this may take 15-30
            seconds.
          </Typography>
        )}
      </FullWidth>
    </FormGrid>
  )
}
