using System.Collections.Generic;
   using UnityEngine;

   public class SelectionManager : MonoBehaviour
   {
       [SerializeField] private RectTransform _selectionBox;
       [SerializeField] private LayerMask _selectableLayer;
       
       private List<Unit> _selectedUnits = new List<Unit>();
       private Vector3 _selectionBoxStart;
       private bool _isSelecting;
       
       private Camera _mainCamera;
       
       private void Awake()
       {
           _mainCamera = Camera.main;
           _selectionBox.gameObject.SetActive(false);
       }
       
       private void Update()
       {
           if (Input.GetMouseButtonDown(0))
           {
               _selectionBoxStart = Input.mousePosition;
               _isSelecting = true;
           }
           
           if (_isSelecting)
           {
               Vector3 currentMousePosition = Input.mousePosition;
               
               // Update selection box visual
               Vector3 center = (_selectionBoxStart + currentMousePosition) / 2f;
               _selectionBox.position = center;
               
               float sizeX = Mathf.Abs(currentMousePosition.x - _selectionBoxStart.x);
               float sizeY = Mathf.Abs(currentMousePosition.y - _selectionBoxStart.y);
               _selectionBox.sizeDelta = new Vector2(sizeX, sizeY);
               
               if (!_selectionBox.gameObject.activeInHierarchy)
               {
                   _selectionBox.gameObject.SetActive(true);
               }
           }
           
           if (Input.GetMouseButtonUp(0))
           {
               _isSelecting = false;
               _selectionBox.gameObject.SetActive(false);
               
               // Handle selection based on box or click
               if (Vector3.Distance(Input.mousePosition, _selectionBoxStart) < 10f)
               {
                   // Single selection
                   HandleSingleSelection();
               }
               else
               {
                   // Box selection
                   HandleBoxSelection();
               }
           }
           
           // Deselect all units when escape is pressed
           if (Input.GetKeyDown(KeyCode.Escape))
           {
               DeselectAll();
           }
       }
       
       private void HandleSingleSelection()
       {
           Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
           
           if (Physics.Raycast(ray, out RaycastHit hit, 100f, _selectableLayer))
           {
               if (hit.collider.TryGetComponent<Unit>(out Unit unit))
               {
                   // If shift isn't held down, deselect previous units
                   if (!Input.GetKey(KeyCode.LeftShift))
                   {
                       DeselectAll();
                   }
                   
                   SelectUnit(unit);
               }
               else
               {
                   if (!Input.GetKey(KeyCode.LeftShift))
                   {
                       DeselectAll();
                   }
               }
           }
           else
           {
               if (!Input.GetKey(KeyCode.LeftShift))
               {
                   DeselectAll();
               }
           }
       }
       
       private void HandleBoxSelection()
       {
           // Convert selection box to viewport space
           Vector2 min = new Vector2(
               Mathf.Min(_selectionBoxStart.x, Input.mousePosition.x),
               Mathf.Min(_selectionBoxStart.y, Input.mousePosition.y)
           );
           
           Vector2 max = new Vector2(
               Mathf.Max(_selectionBoxStart.x, Input.mousePosition.x),
               Mathf.Max(_selectionBoxStart.y, Input.mousePosition.y)
           );
           
           if (!Input.GetKey(KeyCode.LeftShift))
           {
               DeselectAll();
           }
           
           // Find all units within selection box
           foreach (Unit unit in FindObjectsOfType<Unit>())
           {
               Vector3 screenPos = _mainCamera.WorldToScreenPoint(unit.transform.position);
               
               if (screenPos.x > min.x && screenPos.x < max.x &&
                   screenPos.y > min.y && screenPos.y < max.y)
               {
                   SelectUnit(unit);
               }
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