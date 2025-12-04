import React from 'react'

const ComplianceQA = () => {
    return (
        <div>
            <div>
                <h1>What can I help with?</h1>
                <h3>Description</h3>
                <TextField
                    id="standard-multiline-flexible"
                    label="Multiline"
                    multiline
                    maxRows={4}
                    variant="standard"
                />
                <TextField
                    id="standard-textarea"
                    label="Multiline Placeholder"
                    placeholder="Placeholder"
                    multiline
                    variant="standard"
                />
                <TextField
                    id="standard-multiline-static"
                    label="Multiline"
                    multiline
                    rows={4}
                    defaultValue="Default Value"
                    variant="standard"
                />
            </div>
        </div>
    )
}

export default ComplianceQA

