using UnityEngine;

public class Tower : Building
{
    [Header("Combat Settings")]
    [SerializeField] private float _attackRange = 10f;
    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private LayerMask _enemyLayer;
    
    [Header("Visuals")]
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private GameObject _projectilePrefab;
    
    private float _lastAttackTime = 0f;
    private Unit _currentTarget;
    
    protected override void Start()
    {
        base.Start();
        
        if (_projectileSpawnPoint == null)
        {
            GameObject spawnObj = new GameObject("ProjectileSpawn");
            spawnObj.transform.SetParent(transform);
            spawnObj.transform.localPosition = new Vector3(0, 2, 0);
            _projectileSpawnPoint = spawnObj.transform;
        }
    }
    
    private void Update()
    {
        if (!IsConstructed) return;
        
        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            _currentTarget = FindNearestEnemy();
            
            if (_currentTarget != null)
            {
                AttackTarget(_currentTarget);
                _lastAttackTime = Time.time;
            }
        }
    }
    
    private Unit FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, _attackRange, _enemyLayer);
        
        Unit nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (Collider enemyCollider in enemies)
        {
            Unit enemy = enemyCollider.GetComponent<Unit>();
            if (enemy != null && !enemy.IsDead)
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
    
    private void AttackTarget(Unit target)
    {
        if (target == null || target.IsDead) return;
        
        target.TakeDamage(_attackDamage);
        
        Debug.Log($"{gameObject.name} attacked {target.gameObject.name} for {_attackDamage} damage");
        
        if (_projectilePrefab != null && _projectileSpawnPoint != null)
        {
            SpawnProjectile(target.transform.position);
        }
        
        EventManager.TriggerEvent("TowerAttacked", new CombatData
        {
            Attacker = null,
            Target = target,
            Damage = _attackDamage,
            HitPosition = target.transform.position
        });
    }
    
    private void SpawnProjectile(Vector3 targetPosition)
    {
        GameObject projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, Quaternion.identity);
        Vector3 direction = (targetPosition - _projectileSpawnPoint.position).normalized;
        projectile.transform.LookAt(targetPosition);
        Destroy(projectile, 1f);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}