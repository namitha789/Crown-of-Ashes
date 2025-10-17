using UnityEngine;
   using UnityEngine.AI;

   public class Unit : MonoBehaviour
   {
       [SerializeField] private GameObject _selectionIndicator;
       [SerializeField] private UnitData _unitData;
       
       private NavMeshAgent _navAgent;
       private bool _isSelected;
       
       private void Awake()
       {
           _navAgent = GetComponent<NavMeshAgent>();
           _selectionIndicator.SetActive(false);
       }
       
       public void Select()
       {
           _isSelected = true;
           _selectionIndicator.SetActive(true);
       }
       
       public void Deselect()
       {
           _isSelected = false;
           _selectionIndicator.SetActive(false);
       }
       
       public void MoveTo(Vector3 position)
       {
           _navAgent.SetDestination(position);
       }
       
       public void Attack(Unit target)
       {
           // To be implemented in Sprint 1
           Debug.Log($"{gameObject.name} attacking {target.gameObject.name}");
       }
   }