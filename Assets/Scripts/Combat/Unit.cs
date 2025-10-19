using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [Header("Unit Properties")]
    [SerializeField] private string _unitName = "Unit";
    [SerializeField] private int _maxHealth = 100;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject _selectionIndicator;
    
    private NavMeshAgent _navAgent;
    private int _currentHealth;
    private bool _isSelected;
    
    public bool IsSelected => _isSelected;
    public string UnitName => _unitName;
    
    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _currentHealth = _maxHealth;
        
        if (_selectionIndicator != null)
        {
            _selectionIndicator.SetActive(false);
        }
    }
    
    public void Select()
    {
        _isSelected = true;
        if (_selectionIndicator != null)
        {
            _selectionIndicator.SetActive(true);
        }
    }
    
    public void Deselect()
    {
        _isSelected = false;
        if (_selectionIndicator != null)
        {
            _selectionIndicator.SetActive(false);
        }
    }
    
    public void MoveTo(Vector3 position)
    {
        _navAgent.SetDestination(position);
        EventManager.TriggerEvent("UnitMoved", position);
    }
    
    public void Stop()
    {
        _navAgent.SetDestination(transform.position);
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        Destroy(gameObject);
    }
}