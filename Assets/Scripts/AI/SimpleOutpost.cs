using UnityEngine;

public class SimpleOutpost : MonoBehaviour
{
    [Header("Outpost Settings")]
    [SerializeField] private int _maxHealth = 500;
    [SerializeField] private GameObject _enemyUnitPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _initialEnemyCount = 3;
    [SerializeField] private int _rewardGold = 100;
    [SerializeField] private int _rewardShards = 50;

    private int _currentHealth;
    private bool _isDestroyed = false;

    public bool IsDestroyed => _isDestroyed;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
        Debug.Log($"[SimpleOutpost] {gameObject.name} initialized at position: {transform.position}");
        SpawnInitialEnemies();
    }

    private void Update()
    {
        // Log position every second to check if it changes
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"[SimpleOutpost] {gameObject.name} current position: {transform.position}");
        }
    }

    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < _initialEnemyCount && i < _spawnPoints.Length; i++)
        {
            SpawnEnemy(_spawnPoints[i].position);
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        if (_enemyUnitPrefab != null)
        {
            // Spawn enemy as independent GameObject (not parented to castle)
            // Use prefab's default rotation instead of forcing Quaternion.identity
            GameObject enemy = Instantiate(_enemyUnitPrefab, position, _enemyUnitPrefab.transform.rotation);
            // Don't parent to castle - units need to be independent to move properly
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isDestroyed) return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        // NEW: Trigger event for counterattack system
        EventManager.TriggerEvent("OutpostDamaged", this);

        if (_currentHealth <= 0)
        {
            DestroyOutpost();
        }
    }

    private void DestroyOutpost()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;

        // Give rewards
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddResources(_rewardGold, 0, 0);
        }

        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.AddAshShards(_rewardShards);
        }

        EventManager.TriggerEvent("OutpostDestroyed", this);

        Destroy(gameObject);
    }
}