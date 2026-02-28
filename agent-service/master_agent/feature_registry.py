from typing import Any, Dict, Optional


def feature_response(
    type: str,
    message: str,
    *,
    model: Optional[str] = None,
    sources: Optional[list] = None,
    note: Optional[str] = None,
    data: Optional[Dict[str, Any]] = None,
) -> Dict[str, Any]:
    """Standard feature response constructor — ensures consistent shape."""
    resp: Dict[str, Any] = {
        "type": type,
        "message": message,
        "model": model,
        "sources": sources or [],
        "note": note,
    }
    if data is not None:
        resp["data"] = data
    return resp


class FeatureBase:
    """
    Base class for all Features
    """
    async def process(self, payload: Dict[str, Any]) -> Dict[str, Any]:
        """
        Process request - all Features must implement this method
        """
        raise NotImplementedError("Feature must implement process method")


class FeatureRegistry:
    """
    Feature registry - manages all AI features
    """
    
    def __init__(self):
        self._features: Dict[str, FeatureBase] = {}
    
    def register(self, feature_type: str, feature: FeatureBase):
        """
        Register a Feature
        """
        self._features[feature_type] = feature
        print(f"[OK] Registered feature: {feature_type}")
    
    def get_feature(self, feature_type: str) -> FeatureBase:
        """
        Get a Feature
        """
        if feature_type not in self._features:
            raise ValueError(f"Unknown feature type: {feature_type}")
        
        return self._features[feature_type]
    
    def list_features(self) -> list:
        """
        List all registered Features
        """
        return list(self._features.keys())