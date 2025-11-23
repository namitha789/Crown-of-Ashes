using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SelectionManager _selectionManager;

    [Header("Movement Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _formationSpacing = 2f;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
        
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found in UnitController!");
        }
    }

    private void Update()
    {
        HandleUnitCommands();
    }

   private void HandleUnitCommands()
{
    if (_mainCamera == null)
    {
        _mainCamera = Camera.main;
        return;
    }
    
    if (Input.GetMouseButtonDown(1)) // Right click
    {
        Debug.Log("=== RIGHT CLICK DETECTED ===");
        
        List<Unit> selectedUnits = _selectionManager.GetSelectedUnits();
        Debug.Log($"Selected units: {selectedUnits.Count}");

        if (selectedUnits.Count > 0)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Debug.Log($"Raycast hit: {hit.collider.gameObject.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                
                // Check if clicked on enemy unit
                Unit targetUnit = hit.collider.GetComponent<Unit>();
                Debug.Log($"Unit component found: {targetUnit != null}");

                if (targetUnit != null && !targetUnit.IsDead)
                {
                    // Check if it's an enemy
                    if (targetUnit.IsPlayerUnit != selectedUnits[0].IsPlayerUnit)
                    {
                        // Attack enemy unit
                        Debug.Log("✓ Attacking enemy unit!");
                        foreach (Unit unit in selectedUnits)
                        {
                            unit.SetTarget(targetUnit);
                        }
                        return;
                    }
                }

                // Check if clicked on outpost
                SimpleOutpost targetOutpost = hit.collider.GetComponent<SimpleOutpost>();
                Debug.Log($"SimpleOutpost on hit object: {targetOutpost != null}");
                
                if (targetOutpost == null)
                {
                    // Maybe it's a child collider, check parent
                    targetOutpost = hit.collider.GetComponentInParent<SimpleOutpost>();
                    Debug.Log($"SimpleOutpost on parent: {targetOutpost != null}");
                }

                if (targetOutpost != null && !targetOutpost.IsDestroyed)
                {
                    Debug.Log("✓✓✓ ATTACKING OUTPOST! ✓✓✓");
                    // Attack outpost
                    foreach (Unit unit in selectedUnits)
                    {
                        Debug.Log($"Setting outpost target for {unit.gameObject.name}");
                        unit.SetOutpostTarget(targetOutpost);
                    }
                    return;
                }

                Debug.Log($"Checking ground layer. Hit layer: {hit.collider.gameObject.layer}, Ground layer mask: {_groundLayer.value}");
                
                // If not an enemy or outpost, check for ground movement
                if ((_groundLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    Debug.Log("✓ Moving to ground position");
                    // Move command
                    Vector3 targetPosition = hit.point;

                    if (selectedUnits.Count == 1)
                    {
                        selectedUnits[0].MoveTo(targetPosition);
                    }
                    else
                    {
                        ApplyFormationMovement(selectedUnits, targetPosition);
                    }
                }
                else
                {
                    Debug.LogWarning("No valid command found for this target!");
                }
            }
            else
            {
                Debug.LogWarning("Raycast didn't hit anything!");
            }
        }
    }
}

    private void ApplyFormationMovement(List<Unit> units, Vector3 centerPosition)
    {
        int unitCount = units.Count;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(unitCount));

        for (int i = 0; i < unitCount; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Vector3 offset = new Vector3(
                (col - columns / 2f) * _formationSpacing,
                0,
                (row - unitCount / columns / 2f) * _formationSpacing
            );

            units[i].MoveTo(centerPosition + offset);
        }
    }
}