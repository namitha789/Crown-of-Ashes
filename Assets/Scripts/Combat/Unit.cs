using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [Header("Unit Properties")]
    [SerializeField] private string _unitName = "Unit";
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _attackDamage = 10;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private bool _isPlayerUnit = true;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject _selectionIndicator;
    
    private NavMeshAgent _navAgent;
    private int _currentHealth;
    private bool _isSelected;
    private Unit _currentTarget;
    private float _attackTimer;
    
    public bool IsSelected => _isSelected;
    public string UnitName => _unitName;
    public bool IsPlayerUnit => _isPlayerUnit;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    
    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _currentHealth = _maxHealth;
        
        if (_selectionIndicator != null)
        {
            _selectionIndicator.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (_currentTarget != null)
        {
            AttackTarget();
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
        _currentTarget = null;
        EventManager.TriggerEvent("UnitMoved", position);
    }
    
    public void AttackMove(Vector3 position)
    {
        _navAgent.SetDestination(position);
    }
    
    public void SetTarget(Unit target)
    {
        if (target == null || target == this) return;
        
        _currentTarget = target;
        _navAgent.SetDestination(target.transform.position);
    }
    
    private void AttackTarget()
    {
        if (_currentTarget == null)
        {
            return;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, _currentTarget.transform.position);
        
        if (distanceToTarget <= _attackRange)
        {
            _navAgent.SetDestination(transform.position);
            transform.LookAt(_currentTarget.transform);
            
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _attackSpeed)
            {
                CombatManager.DealDamage(this, _currentTarget, _attackDamage);
                _attackTimer = 0f;
            }
        }
        else
        {
            _navAgent.SetDestination(_currentTarget.transform.position);
        }
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        
        EventManager.TriggerEvent("UnitHealthChanged", this);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        EventManager.TriggerEvent("UnitDied", this);
        Destroy(gameObject);
    }
}