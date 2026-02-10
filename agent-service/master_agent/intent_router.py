class IntentRouter:
    """
    Simple Router - Determines destination based on message content
    """
    
    def route(self, message: str, file_name: str = None) -> str:
        """
       Returns which function to use
        """
       # Rule 1: If the filename contains payroll -> payroll feature
        if file_name and 'payroll' in file_name.lower():
            return "payroll_verify"
        
        # Rule 2: If the filename contains roster -> roster feature
        if file_name and 'roster' in file_name.lower():
            return "roster"
        
        # Rule 3: If the message contains keywords -> compliance Q&A
        keywords = ['penalty rate', 'overtime', 'award', 'break']
        if any(kw in message.lower() for kw in keywords):
            return "compliance_qa"
        
        # Default: compliance Q&A
        return "compliance_qa"