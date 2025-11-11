using UnityEngine;

public class Tower : Building
{
    [Header("Combat Settings")]
    [SerializeField] private float _attackRange = 10f;
    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackCooldown = 2f; // seconds
    [SerializeField] private LayerMask _enemyLayer;

    [Header("Visuals")]
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private GameObject _projectilePrefab;

    private float _lastAttackTime = 0f;
    private Unit _currentTarget;

    protected override void Start()
    {
        base.Start();

        // Create projectile spawn point if not assigned
        if (_projectileSpawnPoint == null)
        {
            var spawnObj = new GameObject("ProjectileSpawn");
            spawnObj.transform.SetParent(transform);
            spawnObj.transform.localPosition = new Vector3(0f, 2f, 0f);
            _projectileSpawnPoint = spawnObj.transform;
        }
    }

    private void Update()
    {
        base.Update(); // keep construction/health behavior from Building

        if (!IsConstructed) return;

        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            _currentTarget = FindNearestEnemy();
            if (IsUnitAlive(_currentTarget))
            {
                AttackTarget(_currentTarget);
                _lastAttackTime = Time.time;
            }
        }
    }

    private Unit FindNearestEnemy()
    {
        // Include triggers to support units that use trigger colliders
        var hits = Physics.OverlapSphere(transform.position, _attackRange, _enemyLayer, QueryTriggerInteraction.Collide);

        Unit nearest = null;
        float bestDistSq = float.PositiveInfinity;

        foreach (var col in hits)
        {
            if (!col.TryGetComponent<Unit>(out var enemy)) continue;

            // If your Unit exposes IsPlayerUnit (it does in your project), make sure we only consider enemies
            if (enemy.IsPlayerUnit) continue;

            if (!IsUnitAlive(enemy)) continue;

            float dSq = (enemy.transform.position - transform.position).sqrMagnitude;
            if (dSq < bestDistSq)
            {
                bestDistSq = dSq;
                nearest = enemy;
            }
        }
        return nearest;
    }

    private static bool IsUnitAlive(Unit u)
    {
        // Your Unit type doesn’t expose IsDead; use generic checks that always exist
        // (MonoBehaviour.isActiveAndEnabled ensures the component and GameObject are active)
        return u != null && u.isActiveAndEnabled && u.gameObject.activeInHierarchy;
    }

    private void AttackTarget(Unit target)
    {
        if (!IsUnitAlive(target)) return;

        // Deal damage directly (your Unit already has TakeDamage in Sumedh's system)
        target.TakeDamage(_attackDamage);
        Debug.Log($"{gameObject.name} attacked {target.gameObject.name} for {_attackDamage} damage");

        // Optional visual
        if (_projectilePrefab != null && _projectileSpawnPoint != null)
        {
            var go = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, Quaternion.identity);
            var proj = go.GetComponent<Projectile>();
            if (proj != null) proj.SetTarget(target.transform.position);
            Destroy(go, 2f);
        }

        // Optional event payload
        var data = new TowerCombatData
        {
            Attacker = this,
            Target = target,
            Damage = _attackDamage,
            HitPosition = target.transform.position
        };
        EventManager.TriggerEvent("TowerAttacked", data);
    }

    protected override void OnUpgraded()
    {
        base.OnUpgraded();
        _attackDamage += 10;
        _attackRange += 1f;
        Debug.Log($"Tower upgraded! New damage: {_attackDamage}, New range: {_attackRange}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    public struct TowerCombatData
    {
        public Tower Attacker;
        public Unit Target;
        public int Damage;
        public Vector3 HitPosition;
    }
}
