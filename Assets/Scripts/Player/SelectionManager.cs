using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private LayerMask _selectableLayer;
    
    private List<Unit> _selectedUnits = new List<Unit>();
    private Camera _mainCamera;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    
    private void Update()
    {
        HandleSelection();
    }
    
    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _selectableLayer))
            {
                Unit unit = hit.collider.GetComponent<Unit>();
                
                if (unit != null)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        // Add to selection
                        if (!_selectedUnits.Contains(unit))
                        {
                            SelectUnit(unit);
                        }
                    }
                    else
                    {
                        // Replace selection
                        DeselectAll();
                        SelectUnit(unit);
                    }
                }
                else
                {
                    // Clicked on ground
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectAll();
                    }
                }
            }
        }
        
        // Deselect all with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectAll();
        }
    }
    
    private void SelectUnit(Unit unit)
    {
        if (!_selectedUnits.Contains(unit))
        {
            _selectedUnits.Add(unit);
            unit.Select();
            Debug.Log($"Selected: {unit.UnitName}");
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