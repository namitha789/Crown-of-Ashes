using UnityEngine;

public class CombatVFX : MonoBehaviour
{
    [Header("VFX Prefabs - Existing")]
    [SerializeField] private GameObject _attackImpactPrefab;
    [SerializeField] private GameObject _deathEffectPrefab;
    
    [Header("VFX Prefabs - New")]
    [SerializeField] private GameObject _bloodSplatterPrefab;
    [SerializeField] private GameObject _sparksPrefab;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _swordSlashPrefab;
    
    private void OnEnable()
    {
        // Existing event listeners
        EventManager.StartListening("UnitDamaged", OnUnitDamaged);
        EventManager.StartListening("UnitDied", OnUnitDied);
        
        // New event listeners
        EventManager.StartListening("UnitAttacked", OnUnitAttacked);
        EventManager.StartListening("TowerAttacked", OnTowerAttacked);
    }
    
    private void OnDisable()
    {
        // Existing event listeners
        EventManager.StopListening("UnitDamaged", OnUnitDamaged);
        EventManager.StopListening("UnitDied", OnUnitDied);
        
        // New event listeners
        EventManager.StopListening("UnitAttacked", OnUnitAttacked);
        EventManager.StopListening("TowerAttacked", OnTowerAttacked);
    }
    
    // EXISTING FUNCTIONALITY PRESERVED
    private void OnUnitDamaged(object data)
    {
        if (data is CombatData combatData)
        {
            // Original behavior: spawn attack impact at target position + Vector3.up
            if (_attackImpactPrefab != null)
            {
                Vector3 impactPosition = combatData.Target.transform.position + Vector3.up;
                GameObject impact = Instantiate(_attackImpactPrefab, impactPosition, Quaternion.identity);
                Destroy(impact, 1f);
            }
            
            // NEW: Also spawn blood splatter at hit position if available
            if (_bloodSplatterPrefab != null && combatData.HitPosition != Vector3.zero)
            {
                SpawnBloodSplatter(combatData.HitPosition);
            }
        }
    }
    
    // EXISTING FUNCTIONALITY PRESERVED
    private void OnUnitDied(object data)
    {
        // Original behavior: receive Unit object and spawn death effect
        if (data is Unit unit)
        {
            if (_deathEffectPrefab != null)
            {
                GameObject death = Instantiate(_deathEffectPrefab, unit.transform.position, Quaternion.identity);
                Destroy(death, 2f);
            }
            
            // NEW: Also spawn explosion effect if available
            if (_explosionPrefab != null)
            {
                SpawnExplosion(unit.transform.position);
            }
        }
        // NEW: Support Vector3 position data
        else if (data is Vector3 position)
        {
            if (_explosionPrefab != null)
            {
                SpawnExplosion(position);
            }
        }
    }
    
    // NEW EVENT HANDLERS
    private void OnUnitAttacked(object data)
    {
        if (data is CombatData combatData)
        {
            SpawnSwordSlash(combatData.Attacker.transform.position, combatData.Target.transform.position);
        }
    }
    
    private void OnTowerAttacked(object data)
    {
        if (data is CombatData combatData)
        {
            SpawnSparks(combatData.HitPosition);
        }
    }
    
    // NEW VFX SPAWN METHODS
    private void SpawnBloodSplatter(Vector3 position)
    {
        if (_bloodSplatterPrefab == null) return;
        
        GameObject vfx = Instantiate(_bloodSplatterPrefab, position, Quaternion.identity);
        Destroy(vfx, 2f);
    }
    
    private void SpawnExplosion(Vector3 position)
    {
        if (_explosionPrefab == null) return;
        
        GameObject vfx = Instantiate(_explosionPrefab, position, Quaternion.identity);
        Destroy(vfx, 3f);
    }
    
    private void SpawnSwordSlash(Vector3 attackerPos, Vector3 targetPos)
    {
        if (_swordSlashPrefab == null) return;
        
        Vector3 spawnPos = (attackerPos + targetPos) / 2f; // Midpoint
        GameObject vfx = Instantiate(_swordSlashPrefab, spawnPos, Quaternion.identity);
        
        // Face the target
        vfx.transform.LookAt(targetPos);
        
        Destroy(vfx, 0.5f);
    }
    
    private void SpawnSparks(Vector3 position)
    {
        if (_sparksPrefab == null) return;
        
        GameObject vfx = Instantiate(_sparksPrefab, position, Quaternion.identity);
        Destroy(vfx, 1f);
    }
}