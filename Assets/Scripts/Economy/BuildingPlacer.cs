using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Material _validPlacementMaterial;
    [SerializeField] private Material _invalidPlacementMaterial;
    
    private GameObject _currentBuildingPreview;
    private bool _isPlacing;
    private Camera _mainCamera;
    private int _buildingCostGold;
    private int _buildingCostMaterials;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    
    private void Update()
    {
        if (_isPlacing && _currentBuildingPreview != null)
        {
            UpdateBuildingPreview();
            
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceBuilding();
            }
            else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
            }
        }
    }
    
    public void StartPlacingBuilding(GameObject buildingPrefab, int goldCost, int materialsCost)
    {
        if (_isPlacing)
        {
            CancelPlacement();
        }
        
        _currentBuildingPreview = Instantiate(buildingPrefab);
        _buildingCostGold = goldCost;
        _buildingCostMaterials = materialsCost;
        _isPlacing = true;
        
        // Disable colliders during preview
        foreach (Collider col in _currentBuildingPreview.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        
        Debug.Log($"Placing building: {buildingPrefab.name}");
    }
    
    private void UpdateBuildingPreview()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _groundLayer))
        {
            _currentBuildingPreview.transform.position = hit.point;
            
            // Check if placement is valid
            bool isValid = IsValidPlacement();
            
            // Update material based on validity
            Renderer[] renderers = _currentBuildingPreview.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = isValid ? _validPlacementMaterial : _invalidPlacementMaterial;
            }
        }
    }
    
    private bool IsValidPlacement()
    {
        // Simple check - can be expanded
        return true; // For now, always valid
    }
    
    private void TryPlaceBuilding()
    {
        if (IsValidPlacement())
        {
            if (ResourceManager.Instance.SpendResources(_buildingCostGold, _buildingCostMaterials, 0))
            {
                // Enable colliders
                foreach (Collider col in _currentBuildingPreview.GetComponentsInChildren<Collider>())
                {
                    col.enabled = true;
                }
                
                // Restore original material (create a proper building material)
                Renderer[] renderers = _currentBuildingPreview.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    // You'll need to assign proper materials here
                    renderer.material.color = Color.white;
                }
                
                Debug.Log("Building placed successfully!");
                _currentBuildingPreview = null;
                _isPlacing = false;
            }
        }
    }
    
    private void CancelPlacement()
    {
        if (_currentBuildingPreview != null)
        {
            Destroy(_currentBuildingPreview);
        }
        
        _isPlacing = false;
        Debug.Log("Building placement cancelled");
    }
}