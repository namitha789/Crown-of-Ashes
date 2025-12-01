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

    [Header("Audio")]
    [SerializeField] private AudioClip _buildingPlacedSound;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _placementSoundVolume = 1f;

    private GameObject _currentBuildingPreview;
    private bool _isPlacing;
    private Camera _mainCamera;
    private int _buildingCostGold;
    private int _buildingCostMaterials;
    private GameObject _buildingPrefab;
    private Material[] _originalMaterials; // Store original materials
    
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
    _buildingCostGold = goldCost;
    _buildingCostMaterials = materialsCost;

    // Instantiate with INACTIVE state to prevent Start() from running
    _currentBuildingPreview = Instantiate(buildingPrefab);
    _currentBuildingPreview.SetActive(false);

    _isPlacing = true;

    // Store original materials BEFORE any modifications (including inactive renderers)
    Renderer[] renderers = _currentBuildingPreview.GetComponentsInChildren<Renderer>(true);
    System.Collections.Generic.List<Material> originalMats = new System.Collections.Generic.List<Material>();

    foreach (Renderer renderer in renderers)
    {
        if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
        {
            // Store all materials from all slots
            foreach (Material mat in renderer.sharedMaterials)
            {
                originalMats.Add(mat);
            }
        }
    }

    _originalMaterials = originalMats.ToArray();

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

    // Re-activate the preview (Start() won't run because component is disabled)
    _currentBuildingPreview.SetActive(true);

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
            Material placementMaterial = isValid ? _validPlacementMaterial : _invalidPlacementMaterial;

            // Apply to ALL renderers including inactive ones
            Renderer[] renderers = _currentBuildingPreview.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                // Apply to MeshRenderer and SkinnedMeshRenderer (ignore particles, UI, etc.)
                if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
                {
                    // Apply to all material slots
                    Material[] materials = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i] = placementMaterial;
                    }
                    renderer.sharedMaterials = materials;
                }
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
            // Re-enable colliders
            foreach (Collider col in _currentBuildingPreview.GetComponentsInChildren<Collider>())
            {
                col.enabled = true;
            }

            // Restore original materials
            Renderer[] renderers = _currentBuildingPreview.GetComponentsInChildren<Renderer>(true);
            int materialIndex = 0;

            foreach (Renderer renderer in renderers)
            {
                if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
                {
                    Material[] restoredMaterials = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < restoredMaterials.Length && materialIndex < _originalMaterials.Length; i++)
                    {
                        restoredMaterials[i] = _originalMaterials[materialIndex];
                        materialIndex++;
                    }
                    renderer.sharedMaterials = restoredMaterials;
                }
            }

            // Re-enable Building component LAST (this triggers Start())
            Building buildingComponent = _currentBuildingPreview.GetComponent<Building>();
            if (buildingComponent != null)
            {
                buildingComponent.enabled = true;  // ← This triggers Start() and begins construction
            }

            // Play building placed sound
            PlayBuildingPlacedSound();

            Debug.Log("Building placed successfully!");
            _currentBuildingPreview = null;
            _isPlacing = false;
        }
        else
        {
            Debug.LogWarning("Not enough resources to place building!");
        }
    }
}

    private void PlayBuildingPlacedSound()
    {
        if (_buildingPlacedSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_buildingPlacedSound, _placementSoundVolume);
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