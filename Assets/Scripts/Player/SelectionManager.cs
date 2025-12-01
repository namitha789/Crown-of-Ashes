using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private LayerMask _selectableLayer;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("Building Selection")]
    [SerializeField] private LayerMask _buildingLayer;

    [Header("Drag Selection")]
    [SerializeField] private RectTransform _selectionBox;
    [SerializeField] private Canvas _canvas;

    private List<Unit> _selectedUnits = new List<Unit>();
    private Camera _mainCamera;
    private Vector3 _dragStartPosition;
    private bool _isDragging = false;
    
    private void Awake()
    {
    }

    private void Start()
    {
        _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found in SelectionManager!");
        }

        // Hide selection box initially
        if (_selectionBox != null)
        {
            _selectionBox.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        // Add null check for camera before doing anything
        if (_mainCamera == null)
        {
            return;
        }
        
        CleanupDestroyedUnits();
        HandleSelection();
        HandleCommands();
    }
    
    private void CleanupDestroyedUnits()
    {
        _selectedUnits.RemoveAll(unit => unit == null);
    }
    
    private void HandleSelection()
    {
        // Start drag selection
        if (Input.GetMouseButtonDown(0))
        {
            // Don't process clicks if mouse is over UI
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            _dragStartPosition = Input.mousePosition;
            _isDragging = true;
        }

        // Update drag selection box
        if (_isDragging)
        {
            UpdateSelectionBox();
        }

        // End drag selection
        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;

            if (_selectionBox != null)
            {
                _selectionBox.gameObject.SetActive(false);
            }

            // Check if it was a click or a drag
            float dragDistance = Vector3.Distance(_dragStartPosition, Input.mousePosition);

            if (dragDistance < 5f) // Small distance = click
            {
                HandleSingleClick();
            }
            else // Larger distance = drag selection
            {
                HandleDragSelection();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectAll();
            
            // Also hide building UI
            BuildingInfoUI buildingUI = FindFirstObjectByType<BuildingInfoUI>();
            if (buildingUI != null)
            {
                buildingUI.HidePanel();
            }
        }
    }

    private void UpdateSelectionBox()
    {
        if (_selectionBox == null || _canvas == null) return;

        _selectionBox.gameObject.SetActive(true);

        // Convert screen positions to canvas local positions
        Vector2 boxStart, boxEnd;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            _dragStartPosition,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCamera,
            out boxStart
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            Input.mousePosition,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCamera,
            out boxEnd
        );

        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        _selectionBox.anchoredPosition = boxCenter;

        Vector2 boxSize = new Vector2(
            Mathf.Abs(boxStart.x - boxEnd.x),
            Mathf.Abs(boxStart.y - boxEnd.y)
        );
        _selectionBox.sizeDelta = boxSize;
    }

    private void HandleSingleClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        bool clickedOnUnit = false;
        bool clickedOnBuilding = false;

        // First check for units
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _selectableLayer))
        {
            Unit unit = hit.collider.GetComponent<Unit>();

            if (unit != null && unit.IsPlayerUnit)
            {
                clickedOnUnit = true;

                // Hide building UI when selecting units
                BuildingInfoUI buildingUI = FindFirstObjectByType<BuildingInfoUI>();
                if (buildingUI != null)
                {
                    buildingUI.HidePanel();
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (!_selectedUnits.Contains(unit))
                    {
                        SelectUnit(unit);
                    }
                }
                else
                {
                    DeselectAll();
                    SelectUnit(unit);
                }
            }
        }

        // If didn't click a unit, check for buildings
        if (!clickedOnUnit)
        {
            if (Physics.Raycast(ray, out RaycastHit buildingHit, 1000f))
            {
                Building building = buildingHit.collider.GetComponent<Building>();
                if (building != null)
                {
                    clickedOnBuilding = true;
                    Debug.Log($"Clicked on building: {building.gameObject.name}");

                    // Show building UI
                    BuildingInfoUI buildingUI = FindFirstObjectByType<BuildingInfoUI>();
                    if (buildingUI != null)
                    {
                        buildingUI.ShowBuilding(building);
                    }
                }
            }
        }

        // Only deselect if clicked empty space
        if (!clickedOnUnit && !clickedOnBuilding && !Input.GetKey(KeyCode.LeftShift))
        {
            DeselectAll();

            // Also hide building UI when clicking empty space
            BuildingInfoUI buildingUI = FindFirstObjectByType<BuildingInfoUI>();
            if (buildingUI != null)
            {
                buildingUI.HidePanel();
            }
        }
    }

    private void HandleDragSelection()
    {
        // Calculate screen-space bounds
        Vector2 min = Vector2.Min(_dragStartPosition, Input.mousePosition);
        Vector2 max = Vector2.Max(_dragStartPosition, Input.mousePosition);
        Rect selectionRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

        // Find all player units in the scene
        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);

        // If not holding shift, deselect all first
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            DeselectAll();
        }

        // Select units whose screen position is within the selection box
        foreach (Unit unit in allUnits)
        {
            if (unit == null || !unit.IsPlayerUnit) continue;

            Vector3 screenPos = _mainCamera.WorldToScreenPoint(unit.transform.position);

            // Check if unit is in front of camera and within selection rect
            if (screenPos.z > 0 && selectionRect.Contains(screenPos))
            {
                SelectUnit(unit);
            }
        }

        // Hide building UI when selecting units
        if (_selectedUnits.Count > 0)
        {
            BuildingInfoUI buildingUI = FindFirstObjectByType<BuildingInfoUI>();
            if (buildingUI != null)
            {
                buildingUI.HidePanel();
            }
        }
    }

    private void HandleCommands()
    {
        if (Input.GetMouseButtonDown(1) && _selectedUnits.Count > 0)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                // Check if clicked on outpost
                SimpleOutpost outpost = hit.collider.GetComponent<SimpleOutpost>();
                if (outpost != null && !outpost.IsDestroyed)
                {
                    Debug.Log($"RIGHT-CLICK DETECTED OUTPOST: {outpost.gameObject.name} at position {outpost.transform.position}");

                    // Attack outpost command
                    foreach (Unit unit in _selectedUnits)
                    {
                        if (unit != null)
                        {
                            Debug.Log($"Commanding {unit.gameObject.name} to attack {outpost.gameObject.name}");
                            unit.SetOutpostTarget(outpost);
                        }
                    }
                    return;
                }
                
                // Check if clicked on enemy unit
                Unit enemyUnit = hit.collider.GetComponent<Unit>();
                if (enemyUnit != null && !enemyUnit.IsPlayerUnit)
                {
                    // Attack command
                    foreach (Unit unit in _selectedUnits)
                    {
                        if (unit != null)
                        {
                            unit.SetTarget(enemyUnit);
                        }
                    }
                    return;
                }
                
                // Move command
                Vector3 targetPosition = hit.point;
                
                if (_selectedUnits.Count == 1)
                {
                    if (_selectedUnits[0] != null)
                    {
                        _selectedUnits[0].MoveTo(targetPosition);
                    }
                }
                else
                {
                    ApplyFormationMovement(_selectedUnits, targetPosition);
                }
            }
        }
        
        // Commander ability hotkey
        if (Input.GetKeyDown(KeyCode.C))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                CommanderManager.Instance.UseAbility(hit.point);
            }
        }
    }
    
    private void ApplyFormationMovement(List<Unit> units, Vector3 centerPosition)
    {
        int unitCount = units.Count;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
        float spacing = 2f;
        
        for (int i = 0; i < unitCount; i++)
        {
            if (units[i] == null) continue;
            
            int row = i / columns;
            int col = i % columns;
            
            Vector3 offset = new Vector3(
                (col - columns / 2f) * spacing,
                0,
                (row - unitCount / columns / 2f) * spacing
            );
            
            units[i].MoveTo(centerPosition + offset);
        }
    }
    
    private void SelectUnit(Unit unit)
    {
        if (!_selectedUnits.Contains(unit))
        {
            _selectedUnits.Add(unit);
            unit.Select();
        }
    }
    
    public void DeselectUnit(Unit unit)
    {
        if (_selectedUnits.Contains(unit))
        {
            _selectedUnits.Remove(unit);
            unit.Deselect();
        }
    }
    
    public void DeselectAll()
    {
        foreach (Unit unit in _selectedUnits)
        {
            if (unit != null)
            {
                unit.Deselect();
            }
        }
        
        _selectedUnits.Clear();
    }
    
    public List<Unit> GetSelectedUnits()
    {
        return _selectedUnits;
    }
}