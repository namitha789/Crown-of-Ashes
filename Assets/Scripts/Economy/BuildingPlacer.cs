using UnityEngine;

   public class BuildingPlacer : MonoBehaviour
   {
       [SerializeField] private LayerMask _groundLayer;
       [SerializeField] private Material _validPlacementMaterial;
       [SerializeField] private Material _invalidPlacementMaterial;
       
       private GameObject _currentBuilding;
       private BuildingMenu.BuildingType _currentType;
       private int _goldCost;
       private int _materialsCost;
       private bool _isPlacing;
       private Camera _mainCamera;
       private Renderer[] _buildingRenderers;
       private Material[] _originalMaterials;
       
       private void Awake()
       {
           _mainCamera = Camera.main;
       }
       
       private void Update()
       {
           if (_isPlacing && _currentBuilding != null)
           {
               UpdateBuildingPosition();
               
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
       
       public void StartPlacingBuilding(GameObject buildingPrefab, BuildingMenu.BuildingType type, int goldCost, int materialsCost)
       {
           if (_isPlacing)
           {
               CancelPlacement();
           }
           
           _currentBuilding = Instantiate(buildingPrefab);
           _currentType = type;
           _goldCost = goldCost;
           _materialsCost = materialsCost;
           _isPlacing = true;
           
           // Store renderers and original materials
           _buildingRenderers = _currentBuilding.GetComponentsInChildren<Renderer>();
           _originalMaterials = new Material[_buildingRenderers.Length];
           
           for (int i = 0; i < _buildingRenderers.Length; i++)
           {
               _originalMaterials[i] = _buildingRenderers[i].material;
           }
           
           // Disable colliders during placement
           foreach (Collider collider in _currentBuilding.GetComponentsInChildren<Collider>())
           {
               collider.enabled = false;
           }
       }
       
       private void UpdateBuildingPosition()
       {
           Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
           
           if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _groundLayer))
           {
               _currentBuilding.transform.position = hit.point;
               
               // Check if placement is valid
               bool isValid = IsValidPlacement();
               
               // Update materials based on validity
               Material placementMaterial = isValid ? _validPlacementMaterial : _invalidPlacementMaterial;
               
               foreach (Renderer renderer in _buildingRenderers)
               {
                   renderer.material = placementMaterial;
               }
           }
       }
       
       private bool IsValidPlacement()
       {
           // Simple placement validation - no overlapping colliders
           Collider[] colliders = _currentBuilding.GetComponentsInChildren<Collider>();
           
           foreach (Collider collider in colliders)
           {
               // Temporarily enable the collider for checking
               collider.enabled = true;
               
               // Check for overlapping colliders
               Collider[] overlappingColliders = Physics.OverlapBox(
                   collider.bounds.center,
                   collider.bounds.extents,
                   _currentBuilding.transform.rotation
               );
               
               // Disable it again
               collider.enabled = false;
               
               // If we found any colliders (other than our own building colliders)
               if (overlappingColliders.Length > colliders.Length)
               {
                   return false;
               }
           }
           
           return true;
       }
       
       private void TryPlaceBuilding()
       {
           if (IsValidPlacement())
           {
               if (ResourceManager.Instance.SpendResources(_goldCost, _materialsCost, 0))
               {
                   // Enable colliders
                   foreach (Collider collider in _currentBuilding.GetComponentsInChildren<Collider>())
                   {
                       collider.enabled = true;
                   }
                   
                   // Restore original materials
                   for (int i = 0; i < _buildingRenderers.Length; i++)
                   {
                       _buildingRenderers[i].material = _originalMaterials[i];
                   }
                   
                   // Add appropriate building component
                   AddBuildingComponent(_currentType);
                   
                   // Building placed successfully
                   _currentBuilding = null;
                   _isPlacing = false;
               }
               else
               {
                   Debug.Log("Not enough resources!");
                   CancelPlacement();
               }
           }
       }
       
       private void AddBuildingComponent(BuildingMenu.BuildingType type)
       {
           switch (type)
           {
               case BuildingMenu.BuildingType.CommandCenter:
                   _currentBuilding.AddComponent<CommandCenter>();
                   break;
               case BuildingMenu.BuildingType.Barracks:
                   _currentBuilding.AddComponent<Barracks>();
                   break;
               case BuildingMenu.BuildingType.ResourceCollector:
                   _currentBuilding.AddComponent<ResourceCollector>();
                   break;
               case BuildingMenu.BuildingType.DefenseTower:
                   _currentBuilding.AddComponent<DefenseTower>();
                   break;
           }
       }
       
       private void CancelPlacement()
       {
           if (_currentBuilding != null)
           {
               Destroy(_currentBuilding);
               _currentBuilding = null;
           }
           
           _isPlacing = false;
       }
   }
