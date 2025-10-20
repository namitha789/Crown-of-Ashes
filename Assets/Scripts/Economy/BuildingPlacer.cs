using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Building Prefabs")]
    [SerializeField] private GameObject _barracksPrefab;
    [SerializeField] private GameObject _resourceCollectorPrefab;
    
    [Header("Costs")]
    [SerializeField] private int _barracksCostGold = 100;
    [SerializeField] private int _barracksCostMaterials = 50;
    [SerializeField] private int _collectorCostGold = 75;
    [SerializeField] private int _collectorCostMaterials = 25;
    
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
            _currentBuildingPreview.transform.position = hit.point;
            
            bool isValid = IsValidPlacement();
            
            Renderer[] renderers = _currentBuildingPreview.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material = isValid ? _validPlacementMaterial : _invalidPlacementMaterial;
            }
        }
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