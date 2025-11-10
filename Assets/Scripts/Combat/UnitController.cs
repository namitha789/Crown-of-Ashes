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
            List<Unit> selectedUnits = _selectionManager.GetSelectedUnits();

            if (selectedUnits.Count > 0)
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                {
                    // Check if clicked on enemy unit
                    Unit targetUnit = hit.collider.GetComponent<Unit>();

                    if (targetUnit != null && !targetUnit.IsDead)
                    {
                        // Check if it's an enemy
                        if (targetUnit.IsPlayerUnit != selectedUnits[0].IsPlayerUnit)
                        {
                            // Attack command
                            foreach (Unit unit in selectedUnits)
                            {
                                unit.SetTarget(targetUnit);
                            }
                            Debug.Log($"Attack command to {targetUnit.UnitName}");
                        }
                    }
                    else if ((_groundLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                    {
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