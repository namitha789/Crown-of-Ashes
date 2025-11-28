using UnityEngine;
using UnityEngine.AI;

public class AttackCommandCenterAI : MonoBehaviour
{
    private GameObject _targetCommandCenter;
    private NavMeshAgent _agent;
    private Unit _unitComponent;
    private bool _isInitialized = false;
    private float _attackCheckInterval = 0.5f; // Check every 0.5 seconds
    private float _nextAttackCheck;
    private float _lastAttackTime = 0f; // NEW: Track last attack time

    public void Initialize(GameObject commandCenter)
    {
        _targetCommandCenter = commandCenter;
        _agent = GetComponent<NavMeshAgent>();
        _unitComponent = GetComponent<Unit>();

        if (_agent == null)
        {
            Destroy(this);
            return;
        }

        if (_unitComponent == null)
        {
            Destroy(this);
            return;
        }

        _isInitialized = true;
        _nextAttackCheck = Time.time;
        _lastAttackTime = Time.time - 2f; // Allow immediate first attack

        // Set destination to Command Center
        if (_targetCommandCenter != null && _agent.isOnNavMesh)
        {
            _agent.SetDestination(_targetCommandCenter.transform.position);
        }
    }

    private void Update()
    {
        if (!_isInitialized || _unitComponent.IsDead)
        {
            // If dead, stop this AI
            if (_unitComponent.IsDead)
            {
                Destroy(this);
            }
            return;
        }

        // Check if Command Center still exists
        if (_targetCommandCenter == null)
        {
            SwitchToNormalAI();
            return;
        }

        // Periodic check (not every frame for performance)
        if (Time.time >= _nextAttackCheck)
        {
            _nextAttackCheck = Time.time + _attackCheckInterval;
            CheckAttackCommandCenter();
        }
    }

    private void CheckAttackCommandCenter()
    {
        if (_targetCommandCenter == null || !_agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(transform.position, _targetCommandCenter.transform.position);

        // If within attack range, try to attack
        if (distance <= _unitComponent.AttackRange)
        {
            _agent.isStopped = true; // Stop moving
            AttackCommandCenter();
        }
        else
        {
            // Keep moving toward Command Center
            _agent.isStopped = false;
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                _agent.SetDestination(_targetCommandCenter.transform.position);
            }
        }
    }

    private void AttackCommandCenter()
    {
        if (_targetCommandCenter == null)
            return;

        // NEW: Check attack cooldown based on unit's attack speed
        float attackCooldown = 1.5f;
        if (Time.time < _lastAttackTime + attackCooldown)
        {
            return; // Still on cooldown
        }

        // NEW: Verify we're in range
        float distance = Vector3.Distance(transform.position, _targetCommandCenter.transform.position);
        if (distance > _unitComponent.AttackRange)
        {
            return; // Too far away
        }

        CommandCenter cc = _targetCommandCenter.GetComponent<CommandCenter>();
        if (cc != null && !cc.IsDestroyed)
        {
            // Deal damage
            cc.TakeDamage(_unitComponent.AttackDamage);
            _lastAttackTime = Time.time; // NEW: Record attack time

            // Face the target
            transform.LookAt(_targetCommandCenter.transform);
        }
    }

    private void SwitchToNormalAI()
    {
        SimpleEnemyAI normalAI = gameObject.AddComponent<SimpleEnemyAI>();
        Destroy(this);
    }
}