using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Building Settings")]
    [SerializeField] private string _buildingName = "Building";
    [SerializeField] protected int _maxHealth = 200;
    [SerializeField] protected float _constructionTime = 5f;
    [SerializeField] private GameObject _constructionEffect;

    // ---- Runtime state ----
    protected int _currentHealth;
    private float _constructionProgress;
    private bool _isConstructing = true;

    // ---- Public accessors ----
    public string BuildingName => _buildingName;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public bool IsConstructing => _isConstructing;
    public bool IsConstructed => !_isConstructing;
    public float ConstructionProgress =>
        _constructionTime <= 0f ? 1f : Mathf.Clamp01(_constructionProgress / _constructionTime);

    // ---- Upgrade System ----
    [Header("Upgrade System")]
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private int _maxLevel = 3;
    [SerializeField] private int _upgradeGoldCost = 100;
    [SerializeField] private int _upgradeMaterialsCost = 50;

    public int CurrentLevel => _currentLevel;
    public int MaxLevel => _maxLevel;
    public bool CanUpgrade => _currentLevel < _maxLevel;

    // Allow derived buildings (e.g., Barracks/Tower/Collector) to extend Start/CompleteConstruction
    protected virtual void Start()
    {
        _currentHealth = _maxHealth;

        // If construction time is zero, complete immediately
        if (_constructionTime <= 0f)
        {
            _isConstructing = false;
            if (_constructionEffect != null) _constructionEffect.SetActive(false);
            CompleteConstruction();
        }
        else
        {
            _isConstructing = true;
            if (_constructionEffect != null) _constructionEffect.SetActive(true);
        }
    }

    protected virtual void Update()
    {
        if (_isConstructing)
        {
            _constructionProgress += Time.deltaTime;
            if (_constructionProgress >= _constructionTime)
            {
                CompleteConstruction();
            }
        }
    }

    protected virtual void CompleteConstruction()
    {
        _isConstructing = false;

        if (_constructionEffect != null)
            _constructionEffect.SetActive(false);

        // Fire event & notify child scripts
        EventManager.TriggerEvent("BuildingCompleted", this);
        // Let child scripts react without hard coupling
        SendMessage("OnConstructionComplete", SendMessageOptions.DontRequireReceiver);

#if CHK
        DebugCheckpoint.Hit("Building.CompleteConstruction");
#endif
        Debug.Log($"{_buildingName} construction complete!");
    }

    public virtual void TakeDamage(int damage)
    {
        // By design: ignore damage while constructing
        if (_isConstructing) return;

        _currentHealth -= Mathf.Max(0, damage);
        _currentHealth = Mathf.Max(_currentHealth, 0);

        if (_currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    protected virtual void DestroyBuilding()
    {
        EventManager.TriggerEvent("BuildingDestroyed", this);
#if CHK
        DebugCheckpoint.Hit("Building.DestroyBuilding");
#endif
        Destroy(gameObject);
    }

    // ---- Upgrade API ----
    public bool TryUpgrade()
    {
        if (!CanUpgrade)
        {
            Debug.LogWarning($"{BuildingName} is already max level!");
            return false;
        }

        if (!ResourceManager.Instance.SpendResources(_upgradeGoldCost, _upgradeMaterialsCost, 0))
        {
            Debug.LogWarning($"Not enough resources to upgrade {BuildingName}!");
            return false;
        }

        _currentLevel++;
        OnUpgraded();

        Debug.Log($"{BuildingName} upgraded to level {_currentLevel}");
        return true;
    }

    protected virtual void OnUpgraded()
    {
        // Base upgrade: +20% max health and heal to full
        _maxHealth = Mathf.RoundToInt(_maxHealth * 1.2f);
        _currentHealth = _maxHealth;
    }
}
