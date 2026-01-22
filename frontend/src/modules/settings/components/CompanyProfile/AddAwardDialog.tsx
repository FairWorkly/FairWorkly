// modules/settings/components/CompanyProfile/AddAwardDialog.tsx

import { useState } from 'react'
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    TextField,
    Box,
    Typography,
} from '@mui/material'
import { AwardSelector, type AwardType } from '@/shared/compliance-check/components/AwardSelector'

interface AddAwardDialogProps {
    open: boolean
    onClose: () => void
    onAdd: (awardType: AwardType, employeeCount: number) => void
}

/**
 * Add Award对话框
 * 
 * 功能：
 * 1. 使用共享的 AwardSelector 组件让用户选择 Award 类型
 * 2. 输入该 Award 覆盖的员工数
 * 3. MVP阶段：只是UI演示，不调用API
 * 
 * 未来：
 * - 调用 POST /settings/organization/awards
 * - 验证是否重复添加
 * - 添加成功后刷新列表
 */
export function AddAwardDialog({ open, onClose, onAdd }: AddAwardDialogProps) {
    // 选中的 Award 类型
    const [selectedAward, setSelectedAward] = useState<AwardType>('retail')

    // 员工数量
    const [employeeCount, setEmployeeCount] = useState<string>('')

    // 验证错误
    const [error, setError] = useState<string>('')

    /**
     * 验证表单
     */
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

    /**
     * 处理添加
     */
    const handleAdd = () => {
        if (validateForm()) {
            onAdd(selectedAward, parseInt(employeeCount))
            handleClose()
        }
    }

    /**
     * 关闭对话框并重置状态
     */
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
            <DialogTitle>
                <Typography variant="h6">Add Award</Typography>
                <Typography variant="body2" color="text.secondary">
                    Select an award type and specify how many employees it applies to
                </Typography>
            </DialogTitle>

            <DialogContent>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3, py: 2 }}>
                    {/* 使用共享的 AwardSelector 组件 */}
                    <AwardSelector
                        selectedAward={selectedAward}
                        onAwardChange={setSelectedAward}
                    />

                    {/* 员工数输入 */}
                    <Box>
                        <Typography variant="subtitle2" gutterBottom>
                            Number of Employees
                        </Typography>
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
                            inputProps={{ min: 1 }}
                        />
                    </Box>
                </Box>
            </DialogContent>

            <DialogActions sx={{ px: 3, pb: 3 }}>
                <Button onClick={handleClose} variant="outlined">
                    Cancel
                </Button>
                <Button onClick={handleAdd} variant="contained">
                    Add Award
                </Button>
            </DialogActions>
        </Dialog>
    )
}