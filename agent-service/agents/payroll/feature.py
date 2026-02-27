from typing import Dict, Any
from master_agent.feature_registry import FeatureBase, feature_response


class DemoPayrollFeature(FeatureBase):
    """Stub payroll feature — returns a placeholder response.

    TODO: Replace with real payroll verification logic once payroll agent is implemented.
    """

    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        file_name = payload.get('file_name', 'unknown')

        return feature_response(
            type="payroll_verify",
            message=f"Payroll file '{file_name}' verification is not yet available.",
            note="PAYROLL_NOT_IMPLEMENTED",
        )
