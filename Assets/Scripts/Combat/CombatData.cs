using UnityEngine;

[System.Serializable]
public class CombatData
{
    public Unit Attacker { get; set; }
    public Unit Target { get; set; }
    public int Damage { get; set; }
    public Vector3 HitPosition { get; set; }

    public CombatData() { }

    public CombatData(Unit attacker, Unit target, int damage, Vector3 hitPosition)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
        HitPosition = hitPosition;
    }
}