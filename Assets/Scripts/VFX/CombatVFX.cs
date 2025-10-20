using UnityEngine;

public class CombatVFX : MonoBehaviour
{
    [Header("VFX Prefabs")]
    [SerializeField] private GameObject _attackImpactPrefab;
    [SerializeField] private GameObject _deathEffectPrefab;
    
    private void Start()
    {
        EventManager.StartListening("UnitDamaged", OnUnitDamaged);
        EventManager.StartListening("UnitDied", OnUnitDied);
    }
    
    private void OnDestroy()
    {
        EventManager.StopListening("UnitDamaged", OnUnitDamaged);
        EventManager.StopListening("UnitDied", OnUnitDied);
    }
    
    private void OnUnitDamaged(object data)
    {
        if (data is CombatData combatData && _attackImpactPrefab != null)
        {
            Vector3 impactPosition = combatData.Target.transform.position + Vector3.up;
            GameObject impact = Instantiate(_attackImpactPrefab, impactPosition, Quaternion.identity);
            Destroy(impact, 1f);
        }
    }
    
    private void OnUnitDied(object data)
    {
        if (data is Unit unit && _deathEffectPrefab != null)
        {
            GameObject death = Instantiate(_deathEffectPrefab, unit.transform.position, Quaternion.identity);
            Destroy(death, 2f);
        }
    }
}
