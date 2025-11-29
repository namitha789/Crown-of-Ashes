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

    public void DealDamage(Unit attacker, Unit target, int damage)
    {
        if (target == null || target.IsDead) return;

        target.TakeDamage(damage, attacker);

        EventManager.TriggerEvent(
            "UnitDamaged",
            new CombatData(attacker, target, damage, target.transform.position)
        );
    }
}