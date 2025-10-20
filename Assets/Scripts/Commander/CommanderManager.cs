using UnityEngine;

public class CommanderManager : MonoBehaviour
{
    public static CommanderManager Instance { get; private set; }
    
    [Header("Commander")]
    [SerializeField] private CommanderData _currentCommander;
    
    private float _abilityCooldownTimer;
    private bool _abilityReady = true;
    
    public CommanderData CurrentCommander => _currentCommander;
    public bool IsAbilityReady => _abilityReady;
    public float CooldownProgress => _abilityCooldownTimer / _currentCommander.abilityCooldown;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        if (_currentCommander != null)
        {
            InitializeCommander();
        }
    }
    
    private void Update()
    {
        if (!_abilityReady)
        {
            _abilityCooldownTimer += Time.deltaTime;
            
            if (_abilityCooldownTimer >= _currentCommander.abilityCooldown)
            {
                _abilityReady = true;
                _abilityCooldownTimer = 0f;
                EventManager.TriggerEvent("AbilityReady", null);
            }
        }
    }
    
    private void InitializeCommander()
    {
        ResourceManager.Instance.SetStartingResources(
            _currentCommander.startingGold,
            _currentCommander.startingMaterials,
            0
        );
    }
    
    public void UseAbility(Vector3 position)
    {
        if (!_abilityReady) return;
        
        _abilityReady = false;
        _abilityCooldownTimer = 0f;
        
        // Simple AOE damage
        Collider[] hits = Physics.OverlapSphere(position, _currentCommander.abilityRadius);
        foreach (Collider hit in hits)
        {
            Unit unit = hit.GetComponent<Unit>();
            if (unit != null && !unit.IsPlayerUnit)
            {
                unit.TakeDamage(_currentCommander.abilityDamage);
            }
        }
        
        EventManager.TriggerEvent("AbilityUsed", position);
        Debug.Log($"Used {_currentCommander.abilityName} at {position}");
    }
}