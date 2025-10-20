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
    
    private void Start()
    {
        _currentHealth = _maxHealth;
        SpawnInitialEnemies();
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
            GameObject enemy = Instantiate(_enemyUnitPrefab, position, Quaternion.identity);
            enemy.transform.parent = transform;
        }
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        
        if (_currentHealth <= 0)
        {
            DestroyOutpost();
        }
    }
    
    private void DestroyOutpost()
    {
        // Give rewards
        ResourceManager.Instance.AddResources(_rewardGold, 0, 0);
        ProgressionManager.Instance.AddAshShards(_rewardShards);
        
        EventManager.TriggerEvent("OutpostDestroyed", this);
        
        Destroy(gameObject);
    }
}