using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private LayerMask _selectableLayer;
    [SerializeField] private LayerMask _enemyLayer;
    
    [Header("Building Selection")]
    [SerializeField] private LayerMask _buildingLayer; // Add this field
    
    private List<Unit> _selectedUnits = new List<Unit>();
    private Camera _mainCamera;
    
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
    }
    
    private void Update()
    {
        HandleSelection();
        HandleCommands();
    }
    
    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
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
            }
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
                    // Attack outpost command
                    foreach (Unit unit in _selectedUnits)
                    {
                        unit.SetOutpostTarget(outpost);
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
                        unit.SetTarget(enemyUnit);
                    }
                    return;
                }
                
                // Move command
                Vector3 targetPosition = hit.point;
                
                if (_selectedUnits.Count == 1)
                {
                    _selectedUnits[0].MoveTo(targetPosition);
                }
                else
                {
                    ApplyFormationMovement(_selectedUnits, targetPosition);
                }
            }
        }
        
        // Commander ability hotkey
        if (Input.GetKeyDown(KeyCode.Q))
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
            unit.Deselect();
        }
        
        _selectedUnits.Clear();
    }
    
    public List<Unit> GetSelectedUnits()
    {
        return _selectedUnits;
    }
}