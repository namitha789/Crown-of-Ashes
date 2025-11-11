using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    [Header("Selection Settings")]
    [SerializeField] private LayerMask _selectableLayer;           // must include "Selectable"
    [SerializeField] private LayerMask _enemyLayer;

    [Header("Input/UX")]
    [SerializeField] private bool _ignoreClicksOverUI = true;       // toggle if your canvas steals focus
    [SerializeField] private BuildingInfoUI _buildingUI;            // assign in Inspector

    private readonly List<Unit> _selectedUnits = new List<Unit>();
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null) Debug.LogError("Main Camera not found in SelectionManager!");

        // Fallback to Find if not wired in Inspector
        if (_buildingUI == null) _buildingUI = FindObjectOfType<BuildingInfoUI>(true);
#if CHK
        DebugCheckpoint.Hit($"SelectionManager.Start - UI wired: {_buildingUI != null}");
#endif
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
            // Optional: block world clicks when pointer is over UI
            if (_ignoreClicksOverUI && EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
#if CHK
                DebugCheckpoint.Hit("Blocked click (pointer over UI)");
#endif
                return;
            }

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            // Primary: only Selectable layer (DO NOT ignore triggers)
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _selectableLayer))
            {
                if (TryHandleHit(hit)) return;
            }

            // Fallback: search everything, nearest first (covers mis-layered children)
            var hits = Physics.RaycastAll(ray, 1000f);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var h in hits)
            {
                if (TryHandleHit(h))
                {
#if CHK
                    DebugCheckpoint.Hit($"FallbackRaycast used -> {h.collider.name}");
#endif
                    return;
                }
            }

            // Empty click: deselect & hide
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                if (_buildingUI != null) _buildingUI.HidePanel();
                DeselectAll();
#if CHK
                DebugCheckpoint.Hit("Empty click -> DeselectAll + HidePanel");
#endif
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_buildingUI != null) _buildingUI.HidePanel();
            DeselectAll();
#if CHK
            DebugCheckpoint.Hit("Escape -> DeselectAll + HidePanel");
#endif
        }
    }

    private bool TryHandleHit(RaycastHit hit)
    {
        // Unit?
        var unit = hit.collider.GetComponent<Unit>();
        if (unit != null && unit.IsPlayerUnit)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!_selectedUnits.Contains(unit)) SelectUnit(unit);
            }
            else
            {
                DeselectAll();
                SelectUnit(unit);
            }
#if CHK
            DebugCheckpoint.Hit($"Clicked Unit: {unit.name}");
#endif
            return true;
        }

        // Building?
        var building = hit.collider.GetComponentInParent<Building>();
        if (building != null)
        {
            if (_buildingUI == null) _buildingUI = FindObjectOfType<BuildingInfoUI>(true);
            if (_buildingUI != null) _buildingUI.ShowBuilding(building);
#if CHK
            DebugCheckpoint.Hit($"Clicked Building: {building.name}");
#endif
            return true;
        }

        return false;
    }

    private void HandleCommands()
    {
        if (Input.GetMouseButtonDown(1) && _selectedUnits.Count > 0)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                // Attack outpost
                SimpleOutpost outpost = hit.collider.GetComponent<SimpleOutpost>();
                if (outpost != null && !outpost.IsDestroyed)
                {
                    foreach (Unit unit in _selectedUnits) unit.SetOutpostTarget(outpost);
#if CHK
                    DebugCheckpoint.Hit("RightClick: Attack Outpost");
#endif
                    return;
                }

                // Attack enemy unit
                Unit enemyUnit = hit.collider.GetComponent<Unit>();
                if (enemyUnit != null && !enemyUnit.IsPlayerUnit)
                {
                    foreach (Unit unit in _selectedUnits) unit.SetTarget(enemyUnit);
#if CHK
                    DebugCheckpoint.Hit("RightClick: Attack Enemy Unit");
#endif
                    return;
                }

                // Move
                Vector3 targetPosition = hit.point;
                if (_selectedUnits.Count == 1) _selectedUnits[0].MoveTo(targetPosition);
                else ApplyFormationMovement(_selectedUnits, targetPosition);
#if CHK
                DebugCheckpoint.Hit("RightClick: Move/Formation");
#endif
            }
        }

        // Commander ability hotkey
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                CommanderManager.Instance.UseAbility(hit.point);
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
        foreach (Unit unit in _selectedUnits) unit.Deselect();
        _selectedUnits.Clear();
    }

    public List<Unit> GetSelectedUnits() => _selectedUnits;
}