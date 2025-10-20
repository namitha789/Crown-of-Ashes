using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Unit), typeof(NavMeshAgent))]
public class SimpleEnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float _detectionRange = 10f;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private Transform _guardPosition;
    
    private Unit _unit;
    private NavMeshAgent _navAgent;
    private Unit _currentTarget;
    private float _searchTimer;
    
    private void Awake()
    {
        _unit = GetComponent<Unit>();
        _navAgent = GetComponent<NavMeshAgent>();
        
        if (_guardPosition == null)
        {
            _guardPosition = transform;
        }
    }
    
    private void Update()
    {
        _searchTimer += Time.deltaTime;
        
        if (_searchTimer >= 0.5f)
        {
            FindTarget();
            _searchTimer = 0f;
        }
        
        if (_currentTarget != null)
        {
            AttackTarget();
        }
        else
        {
            ReturnToGuardPosition();
        }
    }
    
    private void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRange);
        
        float closestDistance = float.MaxValue;
        Unit closestEnemy = null;
        
        foreach (Collider hit in hits)
        {
            Unit unit = hit.GetComponent<Unit>();
            if (unit != null && unit.IsPlayerUnit)
            {
                float distance = Vector3.Distance(transform.position, unit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = unit;
                }
            }
        }
        
        _currentTarget = closestEnemy;
    }
    
    private void AttackTarget()
    {
        if (_currentTarget == null)
        {
            return;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, _currentTarget.transform.position);
        
        if (distanceToTarget > _detectionRange)
        {
            _currentTarget = null;
            return;
        }
        
        _unit.SetTarget(_currentTarget);
    }
    
    private void ReturnToGuardPosition()
    {
        float distanceToGuard = Vector3.Distance(transform.position, _guardPosition.position);
        
        if (distanceToGuard > 2f)
        {
            _navAgent.SetDestination(_guardPosition.position);
        }
        else
        {
            _navAgent.SetDestination(transform.position);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}