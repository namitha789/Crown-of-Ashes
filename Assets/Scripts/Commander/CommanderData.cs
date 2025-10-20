using UnityEngine;

[CreateAssetMenu(fileName = "NewCommander", menuName = "Crown of Ashes/Commander")]
public class CommanderData : ScriptableObject
{
    [Header("Basic Info")]
    public string commanderName;
    [TextArea(3, 5)]
    public string description;
    
    [Header("Ability")]
    public string abilityName;
    public string abilityDescription;
    public float abilityCooldown = 60f;
    public int abilityDamage = 50;
    public float abilityRadius = 5f;
    
    [Header("Stats")]
    public int startingGold = 200;
    public int startingMaterials = 100;
}