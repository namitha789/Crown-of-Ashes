using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    public enum CombatState
    {
        Idle,
        Moving,
        Chasing,
        Attacking,
        AttackingOutpost,
        Dead
    }

    [Header("Unit Properties")]
    [SerializeField] private string _unitName = "Unit";
    [SerializeField] private bool _isPlayerUnit = true;
    
    [Header("Combat Stats")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackSpeed = 1.5f;
    [SerializeField] private float _detectionRange = 10f;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject _selectionIndicator;

    [Header("Enemy Detection")]
    [SerializeField] private LayerMask _enemyLayer;
    
    private NavMeshAgent _navAgent;
    private int _currentHealth;
    private bool _isSelected;
    private Unit _currentTarget;
    private SimpleOutpost _currentOutpostTarget;
    private float _attackTimer;
    private bool _isDead = false;
    private float _lastAttackTime = 0f;
    private CombatState _currentState = CombatState.Idle;

    private Animator _animator;
    
    // Public Properties
    public bool IsSelected => _isSelected;
    public string UnitName => _unitName;
    public bool IsPlayerUnit => _isPlayerUnit;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public bool IsDead => _isDead;
    public int AttackDamage => _attackDamage;
    public float AttackRange => _attackRange;
    
    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _currentHealth = _maxHealth;

        if (_selectionIndicator != null)
        {
            _selectionIndicator.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (_isDead) return;
        UpdateAnimation();
        
        switch (_currentState)
        {
            case CombatState.Idle:
                HandleIdleState();
                break;
            case CombatState.Moving:
                HandleMovingState();
                break;
            case CombatState.Chasing:
                HandleChasingState();
                break;
            case CombatState.Attacking:
                HandleAttackingState();
                break;
            case CombatState.AttackingOutpost:
                HandleAttackingOutpostState();
                break;
        }
    }

    private void HandleIdleState()
    {
        // Look for enemies
        Unit enemy = FindNearestEnemy();
        if (enemy != null)
        {
            _currentTarget = enemy;
            _currentOutpostTarget = null;
            ChangeState(CombatState.Chasing);
        }
    }

    private void HandleMovingState()
    {
        // Check if reached destination
        if (!_navAgent.pathPending && _navAgent.remainingDistance <= _navAgent.stoppingDistance)
        {
            ChangeState(CombatState.Idle);
        }
        
        // Check for enemies while moving
        Unit enemy = FindNearestEnemy();
        if (enemy != null)
        {
            _currentTarget = enemy;
            _currentOutpostTarget = null;
            ChangeState(CombatState.Chasing);
        }
    }

    private void HandleChasingState()
    {
        if (_currentTarget == null || _currentTarget.IsDead)
        {
            _navAgent.ResetPath();
            ChangeState(CombatState.Idle);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _currentTarget.transform.position);

        if (distanceToTarget <= _attackRange)
        {
            ChangeState(CombatState.Attacking);
        }
        else
        {
            // Chase target
            _navAgent.SetDestination(_currentTarget.transform.position);
            _navAgent.isStopped = false;
        }
    }

    private void HandleAttackingState()
    {
        if (_currentTarget == null || _currentTarget.IsDead)
        {
            _navAgent.ResetPath();
            ChangeState(CombatState.Idle);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _currentTarget.transform.position);

        if (distanceToTarget > _attackRange)
        {
            // Target moved out of range
            ChangeState(CombatState.Chasing);
        }
        else
        {
            // Stay in place and attack
            _navAgent.isStopped = true;
            transform.LookAt(_currentTarget.transform);
            AttackTarget(_currentTarget);
        }
    }

    private void HandleAttackingOutpostState()
    {
        if (_currentOutpostTarget == null || _currentOutpostTarget.IsDestroyed)
        {
            Debug.Log($"{gameObject.name}: Outpost target is null or destroyed, returning to Idle");
            _currentOutpostTarget = null;
            _navAgent.ResetPath();
            ChangeState(CombatState.Idle);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _currentOutpostTarget.transform.position);

        Debug.Log($"{gameObject.name}: Moving to outpost {_currentOutpostTarget.gameObject.name} at {_currentOutpostTarget.transform.position}, distance: {distanceToTarget:F2}");

        if (distanceToTarget > _attackRange)
        {
            // Move closer to outpost
            Vector3 outpostPosition = _currentOutpostTarget.transform.position;
            Debug.Log($"{gameObject.name}: Setting NavAgent destination to {outpostPosition} for outpost {_currentOutpostTarget.gameObject.name}");
            Debug.Log($"{gameObject.name}: Current NavAgent destination: {_navAgent.destination}");
            _navAgent.SetDestination(outpostPosition);
            _navAgent.isStopped = false;
            Debug.Log($"{gameObject.name}: NavAgent destination after setting: {_navAgent.destination}");
        }
        else
        {
            // Stay in place and attack
            _navAgent.isStopped = true;
            transform.LookAt(_currentOutpostTarget.transform);
            AttackOutpost(_currentOutpostTarget);
        }
    }

    private void ChangeState(CombatState newState)
    {
        // Reset state-specific settings when leaving a state
        if (_currentState == CombatState.Attacking || _currentState == CombatState.AttackingOutpost)
        {
            _navAgent.isStopped = false;
        }

        // Immediately stop movement when entering Attacking state
        if (newState == CombatState.Attacking || newState == CombatState.AttackingOutpost)
        {
            _navAgent.isStopped = true;
            _navAgent.velocity = Vector3.zero; // Kill all momentum immediately
        }

        // Clear path when entering Idle to stop all movement
        if (newState == CombatState.Idle)
        {
            _navAgent.ResetPath();
            _navAgent.isStopped = false;
        }

        _currentState = newState;
        Debug.Log($"{gameObject.name} changed state to {newState}");
    }
    
    private void AttackTarget(Unit target)
    {
        if (target == null || target.IsDead) return;

        // Check if enough time has passed since last attack
        if (Time.time < _lastAttackTime + _attackSpeed) return;

        // Check if target is in range
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget > _attackRange) return;

        // Perform attack
        _lastAttackTime = Time.time;
        target.TakeDamage(_attackDamage);

        // Face the target
        transform.LookAt(target.transform);

        // Trigger attack animation
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }

        // Trigger attack event for animation/VFX
        EventManager.TriggerEvent("UnitAttacked", new CombatData(this, target, _attackDamage, target.transform.position));

        Debug.Log($"{gameObject.name} attacked {target.gameObject.name} for {_attackDamage} damage");
    }
    
    private void AttackOutpost(SimpleOutpost outpost)
    {
        if (outpost == null || outpost.IsDestroyed) return;

        // Check if enough time has passed since last attack
        if (Time.time < _lastAttackTime + _attackSpeed) return;

        // Check if target is in range
        float distanceToTarget = Vector3.Distance(transform.position, outpost.transform.position);
        if (distanceToTarget > _attackRange) return;

        // Perform attack
        _lastAttackTime = Time.time;
        outpost.TakeDamage(_attackDamage);

        // Face the target
        transform.LookAt(outpost.transform);

        // Trigger attack animation
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }

        Debug.Log($"{gameObject.name} attacked outpost for {_attackDamage} damage!");
    }
    
      public void TakeDamage(int damage)
      {
          if (_isDead) return;

          _currentHealth -= damage;
          _currentHealth = Mathf.Max(_currentHealth, 0);

          Debug.Log($"{gameObject.name} took {damage} damage! HP: {_currentHealth}/{_maxHealth}");

          // Trigger damage event
          EventManager.TriggerEvent("UnitDamaged", new CombatData
          {
              Attacker = null,
              Target = this,
              Damage = damage,
              HitPosition = transform.position
          });

          if (_currentHealth <= 0)
          {
              Die();
          }
      }
      private void PerformAttack(Unit target)
      {
          if (_animator != null)
          {
              _animator.SetTrigger("Attack"); // Trigger attack animation
          }

          // Deal damage
          target.TakeDamage(_attackDamage);
      }    
    private void Die()
    {
        if (_isDead) return;
        
        _isDead = true;
        if (_animator != null)
            {
                _animator.SetTrigger("Die");
            }
        
        // Trigger death event
        EventManager.TriggerEvent("UnitDied", transform.position);
        
        Debug.Log($"{gameObject.name} has died!");
        
        // Disable components
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Collider>().enabled = false;
        
        // Trigger death effect if available
        UnitDeathEffect deathEffect = GetComponent<UnitDeathEffect>();
        if (deathEffect != null)
        {
            deathEffect.TriggerDeath(); // Will destroy after fade
        }
        else
        {
            Destroy(gameObject, 1f); // Fallback
        }
    }


    private Unit FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, _detectionRange, _enemyLayer);
        
        Unit nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (Collider enemyCollider in enemies)
        {
            Unit enemy = enemyCollider.GetComponent<Unit>();
            if (enemy != null && !enemy.IsDead && enemy.IsPlayerUnit != this.IsPlayerUnit)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }
        
        return nearestEnemy;
    }

    public void ApplyAreaDamage(int damage, Vector3 sourcePosition)
    {
        TakeDamage(damage);
        
        // Visual feedback - push unit back slightly
        Vector3 pushDirection = (transform.position - sourcePosition).normalized;
        transform.position += pushDirection * 0.5f;
    }

    public static List<Unit> GetUnitsInRadius(Vector3 center, float radius, LayerMask targetLayer)
    {
        List<Unit> units = new List<Unit>();
        Collider[] hits = Physics.OverlapSphere(center, radius, targetLayer);
        
        foreach (Collider hit in hits)
        {
            Unit unit = hit.GetComponent<Unit>();
            if (unit != null && !unit.IsDead)
            {
                units.Add(unit);
            }
        }
        
        return units;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (_isDead) return;

        Debug.LogWarning($"{gameObject.name}: MoveTo() called, CLEARING outpost target! Moving to {targetPosition}");
        _currentTarget = null;
        _currentOutpostTarget = null;
        _navAgent.SetDestination(targetPosition);
        _navAgent.isStopped = false;

        ChangeState(CombatState.Moving);

        EventManager.TriggerEvent("UnitMoved", targetPosition);
    }

    public void SetTarget(Unit target)
    {
        if (_isDead) return;

        Debug.LogWarning($"{gameObject.name}: SetTarget() called, CLEARING outpost target! Targeting unit {target.gameObject.name}");
        _currentTarget = target;
        _currentOutpostTarget = null;
        ChangeState(CombatState.Chasing);
    }
    
    public void SetOutpostTarget(SimpleOutpost outpost)
    {
        if (_isDead || outpost == null) return;

        _currentTarget = null;
        _currentOutpostTarget = outpost;
        ChangeState(CombatState.AttackingOutpost);

        Debug.Log($"{gameObject.name} targeting outpost: {outpost.gameObject.name} at {outpost.transform.position}");
    }
    
    public void ClearTarget()
    {
        _currentTarget = null;
        _currentOutpostTarget = null;
        ChangeState(CombatState.Idle);
    }
    
    public void AttackMove(Vector3 position)
    {
        MoveTo(position);
        // The unit will automatically engage enemies it encounters
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
    
    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    private void UpdateAnimation()
    {
        if (_animator == null) return;

        // Set speed parameter based on NavMeshAgent velocity
        float speed = _navAgent.velocity.magnitude;
        _animator.SetFloat("Speed", speed);
    }

}































