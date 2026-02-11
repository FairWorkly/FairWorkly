import { useState } from 'react'
import {
    Dialog,
    DialogContent,
    TextField,
} from '@mui/material'
import { AwardSelector, type AwardType } from '@/shared/compliance-check/components/AwardSelector'
import { DialogHeader, CardTitle, CardDescription, DialogBody, FieldBlock, DialogFooter, CancelButton, SaveButton } from './CompanyProfile.styles'

interface AddAwardDialogProps {
    open: boolean
    onClose: () => void
    onAdd: (awardType: AwardType, employeeCount: number) => void
}

export function AddAwardDialog({ open, onClose, onAdd }: AddAwardDialogProps) {
    const [selectedAward, setSelectedAward] = useState<AwardType>('retail')
    const [employeeCount, setEmployeeCount] = useState<string>('')
    const [error, setError] = useState<string>('')


    const validateForm = (): boolean => {
        if (!employeeCount) {
            setError('Employee count is required')
            return false
        }

        const count = parseInt(employeeCount)
        if (isNaN(count) || count < 1) {
            setError('Employee count must be a positive number')
            return false
        }

        setError('')
        return true
    }


    const handleAdd = () => {
        if (validateForm()) {
            onAdd(selectedAward, parseInt(employeeCount))
            handleClose()
        }
    }

    const handleClose = () => {
        setSelectedAward('retail')
        setEmployeeCount('')
        setError('')
        onClose()
    }

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            maxWidth="md"
            fullWidth
        >
            <DialogHeader>
                <CardTitle>Add Award</CardTitle>
                <CardDescription>
                    Select an award type and specify how many employees it applies to
                </CardDescription>
            </DialogHeader>

            <DialogContent>
                <DialogBody>
                    <AwardSelector
                        selectedAward={selectedAward}
                        onAwardChange={setSelectedAward}
                    />

                    <FieldBlock>
                        <CardDescription>
                            Number of Employees
                        </CardDescription>
                        <TextField
                            fullWidth
                            type="number"
                            placeholder="e.g., 25"
                            value={employeeCount}
                            onChange={(e) => {
                                setEmployeeCount(e.target.value)
                                setError('')
                            }}
                            error={!!error}
                            helperText={error || 'How many employees does this award apply to?'}
                            slotProps={{ htmlInput: { min: 1 } }}
                        />
                    </FieldBlock>
                </DialogBody>
            </DialogContent>

            <DialogFooter>
                <CancelButton onClick={handleClose} variant="outlined">
                    Cancel
                </CancelButton>
                <SaveButton onClick={handleAdd} variant="contained">
                    Add Award
                </SaveButton>
            </DialogFooter>
        </Dialog>
    )
}