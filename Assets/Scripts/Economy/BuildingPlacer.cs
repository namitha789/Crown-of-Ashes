using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Building Prefabs")]
    [SerializeField] private GameObject _barracksPrefab;
    [SerializeField] private GameObject _resourceCollectorPrefab;
    [SerializeField] private GameObject _towerPrefab;
    
    [Header("Costs")]
    [SerializeField] private int _barracksCostGold = 100;
    [SerializeField] private int _barracksCostMaterials = 50;
    [SerializeField] private int _collectorCostGold = 75;
    [SerializeField] private int _collectorCostMaterials = 25;
    [SerializeField] private int _towerCostGold = 150;
    [SerializeField] private int _towerCostMaterials = 75;
    
    [Header("Placement Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Material _validPlacementMaterial;
    [SerializeField] private Material _invalidPlacementMaterial;
    
    private GameObject _currentBuildingPreview;
    private bool _isPlacing;
    private Camera _mainCamera;
    private int _buildingCostGold;
    private int _buildingCostMaterials;
    private GameObject _buildingPrefab;
    
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
    
    public void StartPlacingBarracks()
    {
        StartPlacingBuilding(_barracksPrefab, _barracksCostGold, _barracksCostMaterials);
    }
    
    public void StartPlacingCollector()
    {
        StartPlacingBuilding(_resourceCollectorPrefab, _collectorCostGold, _collectorCostMaterials);
    }
    
    public void StartPlacingTower()
    {
        StartPlacingBuilding(_towerPrefab, _towerCostGold, _towerCostMaterials);
    }
    
    private void StartPlacingBuilding(GameObject buildingPrefab, int goldCost, int materialsCost)
    {
        if (_isPlacing)
        {
            CancelPlacement();
        }
        
        _buildingPrefab = buildingPrefab;
        _currentBuildingPreview = Instantiate(buildingPrefab);
        _buildingCostGold = goldCost;
        _buildingCostMaterials = materialsCost;
        _isPlacing = true;
        
        // Disable components during preview
        foreach (Collider col in _currentBuildingPreview.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        
        Building buildingComponent = _currentBuildingPreview.GetComponent<Building>();
        if (buildingComponent != null)
        {
            buildingComponent.enabled = false;
        }
        
        Debug.Log($"Placing building. Cost: {goldCost} Gold, {materialsCost} Materials");
    }
    
    private void UpdateBuildingPreview()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _groundLayer))
        {
            // Calculate the building's bounds to position it correctly on the ground
            Bounds buildingBounds = GetBuildingBounds(_currentBuildingPreview);
            float yOffset = buildingBounds.min.y - _currentBuildingPreview.transform.position.y;
            
            Vector3 position = hit.point;
            position.y -= yOffset; // Adjust Y so bottom of building sits on ground
            
            _currentBuildingPreview.transform.position = position;
            
            bool isValid = IsValidPlacement();
            
            Renderer[] renderers = _currentBuildingPreview.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = isValid ? _validPlacementMaterial : _invalidPlacementMaterial;
            }
        }
    }
    
    private Bounds GetBuildingBounds(GameObject building)
    {
        Bounds bounds = new Bounds(building.transform.position, Vector3.zero);
        bool hasBounds = false;
        
        // Use renderers for bounds (colliders are disabled during preview)
        Renderer[] renderers = building.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.enabled && renderer.bounds.size != Vector3.zero)
            {
                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }
        
        // If no renderers found, try colliders (even if disabled, they might have bounds)
        if (!hasBounds)
        {
            Collider[] colliders = building.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                if (col.bounds.size != Vector3.zero)
                {
                    if (!hasBounds)
                    {
                        bounds = col.bounds;
                        hasBounds = true;
                    }
                    else
                    {
                        bounds.Encapsulate(col.bounds);
                    }
                }
            }
        }
        
        return bounds;
    }
    
    private bool IsValidPlacement()
    {
        return true;
    }
    
    private void TryPlaceBuilding()
    {
        if (IsValidPlacement())
        {
            if (ResourceManager.Instance.SpendResources(_buildingCostGold, _buildingCostMaterials, 0))
            {
                // Re-enable components
                foreach (Collider col in _currentBuildingPreview.GetComponentsInChildren<Collider>())
                {
                    col.enabled = true;
                }
                
                Building buildingComponent = _currentBuildingPreview.GetComponent<Building>();
                if (buildingComponent != null)
                {
                    buildingComponent.enabled = true;
                }
                
                // Restore materials
                Renderer[] renderers = _currentBuildingPreview.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
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