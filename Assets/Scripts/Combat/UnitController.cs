using System.Collections.Generic;
   using UnityEngine;

   public class UnitController : MonoBehaviour
   {
       [SerializeField] private SelectionManager _selectionManager;
       [SerializeField] private LayerMask _groundLayer;
       
       private Camera _mainCamera;
       
       private void Awake()
       {
           _mainCamera = Camera.main;
       }
       
       private void Update()
       {
           HandleUnitCommands();
       }
       
       private void HandleUnitCommands()
       {
           if (Input.GetMouseButtonDown(1))
           {
               List<Unit> selectedUnits = _selectionManager.GetSelectedUnits();
               
               if (selectedUnits.Count > 0)
               {
                   Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                   
                   if (Physics.Raycast(ray, out RaycastHit hit))
                   {
                       if (hit.collider.TryGetComponent<Unit>(out Unit targetUnit))
                       {
                           // Attack command
                           foreach (Unit unit in selectedUnits)
                           {
                               unit.Attack(targetUnit);
                           }
                       }
                       else if ((_groundLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                       {
                           // Move command
                           Vector3 targetPosition = hit.point;
                           
                           if (selectedUnits.Count == 1)
                           {
                               // Single unit movement
                               selectedUnits[0].MoveTo(targetPosition);
                           }
                           else
                           {
                               // Formation movement
                               ApplyFormationMovement(selectedUnits, targetPosition);
                           }
                       }
                   }
               }
           }
       }
       
       private void ApplyFormationMovement(List<Unit> units, Vector3 centerPosition)
       {
           // Simple grid formation
           int unitCount = units.Count;
           int columns = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
           float spacing = 2.0f;
           
           for (int i = 0; i < unitCount; i++)
           {
               int row = i / columns;
               int col = i % columns;
               
               Vector3 offset = new Vector3(
                   (col - (columns / 2)) * spacing, 
                   0, 
                   (row - (unitCount / columns / 2)) * spacing
               );
               
               units[i].MoveTo(centerPosition + offset);
           }
       }
   }