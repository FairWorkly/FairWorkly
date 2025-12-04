import {
    Box,
    Button,
    TextField,
} from '@mui/material'

const ComplianceQA = () => {
    return (
        <Box
            component="form"
            sx={{ '& .MuiTextField-root': { m: 1, width: '25ch' } }}
            noValidate
            autoComplete="off"
        >
            <h1>Hello</h1>
            <h5>I will be ready when you are.</h5>
            <TextField
                id="outlined-multiline-static"
                multiline
                rows={4}
                defaultValue="Ask anything"
            />
            <Button>Send</Button>
        </Box>
    )
}

export default ComplianceQA
