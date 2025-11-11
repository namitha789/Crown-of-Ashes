using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    [SerializeField] private GameObject _damageNumberPrefab;
    
    private static DamageNumberManager _instance;
    public static DamageNumberManager Instance => _instance;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    
    private void OnEnable()
    {
        EventManager.StartListening("UnitDamaged", OnUnitDamaged);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening("UnitDamaged", OnUnitDamaged);
    }
    
    private void OnUnitDamaged(object data)
    {
        if (data is CombatData combatData)
        {
            SpawnDamageNumber(combatData.Damage, combatData.HitPosition);
        }
    }
    
    public void SpawnDamageNumber(int damage, Vector3 position)
    {
        if (_damageNumberPrefab == null) return;
        
        // Spawn slightly above hit position
        Vector3 spawnPos = position + Vector3.up * 1.5f;
        
        GameObject dmgNum = Instantiate(_damageNumberPrefab, spawnPos, Quaternion.identity);
        DamageNumberAnimator animator = dmgNum.GetComponent<DamageNumberAnimator>();
        
        if (animator != null)
        {
            animator.SetDamage(damage);
        }
    }
}

