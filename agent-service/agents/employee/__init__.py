"""
Employee agent package.

Provides routing to employee-facing features:
- General chat
- Payslip viewing
- Roster viewing
- Leave requests
"""

from .employee_feature import EmployeeFeature
from .chat_handler import ChatHandler
from .payslip_handler import PayslipHandler
from .roster_handler import RosterHandler
from .leave_handler import LeaveHandler

__all__ = [
    "EmployeeFeature",
    "ChatHandler",
    "PayslipHandler",
    "RosterHandler",
    "LeaveHandler",
]
