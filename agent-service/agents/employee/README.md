# Employee Agent

Routes employee-facing queries to specialized handlers.

## Files
- employee_feature.py: Main entry point; routes to the correct handler.
- chat_handler.py: General chat and FAQs.
- payslip_handler.py: Payslip retrieval or summaries.
- roster_handler.py: Roster lookup for upcoming or past shifts.
- leave_handler.py: Leave balance checks and request submission.

## TODOs
- Implement real routing/intent detection instead of placeholder branching.
- Connect each handler to data sources and permission checks.
- Add tests once business logic is defined.
