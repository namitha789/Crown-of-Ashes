using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public static void DealDamage(Unit attacker, Unit target, int damage)
    {
        if (target == null || attacker == null) return;
        
        target.TakeDamage(damage);
        
        EventManager.TriggerEvent("UnitDamaged", new CombatData 
        { 
            Attacker = attacker, 
            Target = target, 
            Damage = damage 
        });
        
        Debug.Log($"{attacker.UnitName} dealt {damage} damage to {target.UnitName}");
    }
}

[System.Serializable]
public class CombatData
{
    public Unit Attacker;
    public Unit Target;
    public int Damage;
}