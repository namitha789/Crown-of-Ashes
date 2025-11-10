using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private float _chaseRadius = 15f;
    [SerializeField] private float _returnDistance = 20f;
    [SerializeField] private LayerMask _playerLayer;
    
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _chaseSpeed = 4f;
    [SerializeField] private float _waypointReachDistance = 1f;
    
    private NavMeshAgent _agent;
    private Unit _unitComponent;
    private Vector3 _guardPosition;
    private Unit _currentTarget;
    private int _currentPatrolIndex = 0;
    private AIState _currentState = AIState.Patrolling;
    
    private enum AIState
    {
        Patrolling,
        Chasing,
        Returning,
        Guarding
    }
    
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _unitComponent = GetComponent<Unit>();
        _guardPosition = transform.position;
        
        _agent.speed = _patrolSpeed;
        
        // Auto-find patrol points if array is empty or all null
        if (_patrolPoints == null || _patrolPoints.Length == 0 || AllPatrolPointsNull())
        {
            FindPatrolPointsInChildren();
        }
        
        if (_patrolPoints != null && _patrolPoints.Length > 0 && !AllPatrolPointsNull())
        {
            GoToNextPatrolPoint();
        }
    }
    
    private bool AllPatrolPointsNull()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0) return true;
        
        foreach (Transform point in _patrolPoints)
        {
            if (point != null) return false;
        }
        return true;
    }
    
    private void FindPatrolPointsInChildren()
    {
        // Look for child objects named "PatrolPoint" or with "PatrolPoint" in the name
        List<Transform> foundPoints = new List<Transform>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.Contains("PatrolPoint") || child.name.Contains("Patrol Point"))
            {
                foundPoints.Add(child);
            }
        }
        
        // Also check for a parent "PatrolPoints" container
        Transform parent = transform.parent;
        if (parent != null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform sibling = parent.GetChild(i);
                if (sibling != transform && (sibling.name.Contains("PatrolPoint") || sibling.name.Contains("Patrol Point")))
                {
                    if (!foundPoints.Contains(sibling))
                    {
                        foundPoints.Add(sibling);
                    }
                }
            }
        }
        
        if (foundPoints.Count > 0)
        {
            _patrolPoints = foundPoints.ToArray();
            Debug.Log($"{gameObject.name}: Auto-found {_patrolPoints.Length} patrol points");
        }
    }
    
    private void Update()
    {
        if (_unitComponent == null || _unitComponent.CurrentHealth <= 0) return;
        
        switch (_currentState)
        {
            case AIState.Patrolling:
                HandlePatrolState();
                break;
            case AIState.Chasing:
                HandleChaseState();
                break;
            case AIState.Returning:
                HandleReturnState();
                break;
            case AIState.Guarding:
                HandleGuardState();
                break;
        }
    }
    
    private void HandlePatrolState()
    {
        // Check for player units
        Unit player = FindNearestPlayer();
        if (player != null)
        {
            _currentTarget = player;
            ChangeState(AIState.Chasing);
            return;
        }
        
        // Continue patrol
        if (_patrolPoints == null || _patrolPoints.Length == 0 || AllPatrolPointsNull())
        {
            ChangeState(AIState.Guarding);
            return;
        }
        
        if (!_agent.pathPending && _agent.remainingDistance <= _waypointReachDistance)
        {
            GoToNextPatrolPoint();
        }
    }
    
    private void GoToNextPatrolPoint()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0) return;
        
        // Find next valid patrol point
        int attempts = 0;
        while (attempts < _patrolPoints.Length)
        {
            if (_patrolPoints[_currentPatrolIndex] != null)
            {
                _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
                _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
                return;
            }
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            attempts++;
        }
        
        // No valid patrol points found
        ChangeState(AIState.Guarding);
    }
    
    private void HandleChaseState()
    {
        if (_currentTarget == null || _currentTarget.CurrentHealth <= 0)
        {
            _currentTarget = null;
            _unitComponent.ClearTarget(); // Clear Unit's target!
            ChangeState(AIState.Returning);
            return;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, _currentTarget.transform.position);
        float distanceFromGuard = Vector3.Distance(transform.position, _guardPosition);
        
        // Check if target escaped too far
        if (distanceToTarget > _chaseRadius || distanceFromGuard > _returnDistance)
        {
            _currentTarget = null;
            _unitComponent.ClearTarget(); // Clear Unit's target!
            ChangeState(AIState.Returning);
            return;
        }
        
        // Tell Unit component to attack the target
        _unitComponent.SetTarget(_currentTarget);
        
        // Move towards target
        _agent.SetDestination(_currentTarget.transform.position);
    }
    
    private void HandleReturnState()
    {
        // Don't detect players while returning!
        
        float distToGuard = Vector3.Distance(transform.position, _guardPosition);
        
        // Check if reached guard position
        if (distToGuard <= _waypointReachDistance)
        {
            if (_patrolPoints != null && _patrolPoints.Length > 0 && !AllPatrolPointsNull())
            {
                ChangeState(AIState.Patrolling);
            }
            else
            {
                ChangeState(AIState.Guarding);
            }
            return;
        }
        
        // Return to guard position
        _agent.SetDestination(_guardPosition);
    }
    
    private void HandleGuardState()
    {
        // Check for player units
        Unit player = FindNearestPlayer();
        if (player != null)
        {
            _currentTarget = player;
            ChangeState(AIState.Chasing);
        }
    }
    
    private Unit FindNearestPlayer()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, _detectionRadius, _playerLayer);
        
        Unit nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (Collider playerCollider in players)
        {
            Unit player = playerCollider.GetComponent<Unit>();
            if (player != null && player.IsPlayerUnit && player.CurrentHealth > 0)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                float distanceFromGuard = Vector3.Distance(transform.position, _guardPosition);
                
                // Don't detect players if we're too far from guard position
                if (distanceFromGuard > _returnDistance)
                {
                    continue;
                }
                
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = player;
                }
            }
        }
        
        return nearestPlayer;
    }
    
    private void ChangeState(AIState newState)
    {
        _currentState = newState;
        
        // Adjust speed based on state
        switch (newState)
        {
            case AIState.Chasing:
                _agent.speed = _chaseSpeed;
                break;
            default:
                _agent.speed = _patrolSpeed;
                break;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Detection radius (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        
        // Chase radius (orange)
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, _chaseRadius);
        
        // Return distance (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _returnDistance);
    }
}