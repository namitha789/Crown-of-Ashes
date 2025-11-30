using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour
{
    [Header("Building Settings")]
    [SerializeField] private string _buildingName = "Barrack";
    [SerializeField] protected int _maxHealth = 200;
    [SerializeField] protected float _constructionTime = 5f;
    [SerializeField] private GameObject _constructionEffect;

    [Header("Construction Costs")]
    [SerializeField] private int _constructionCostGold = 50;
    [SerializeField] private int _constructionCostMaterials = 25;

    // ========== NEW: UPGRADE SYSTEM ==========
    [Header("Upgrade System")]
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private int _maxLevel = 3;
    [SerializeField] private int _upgradeGoldCost = 100;
    [SerializeField] private int _upgradeMaterialsCost = 50;
    // =========================================

    protected int _currentHealth;
    private bool _isConstructing = false;
    private Coroutine _constructionCoroutine;

    // PUBLIC PROPERTIES
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public string BuildingName => _buildingName;
    public bool IsConstructing => _isConstructing;
    public bool IsConstructed => !_isConstructing;
    public float ConstructionProgress { get; private set; }
    
    // Cost properties
    public int ConstructionCostGold => _constructionCostGold;
    public int ConstructionCostMaterials => _constructionCostMaterials;

    // ========== NEW: UPGRADE PROPERTIES ==========
    public int CurrentLevel => _currentLevel;
    public int MaxLevel => _maxLevel;
    public bool CanUpgrade => _currentLevel < _maxLevel;
    public int UpgradeGoldCost => _upgradeGoldCost;
    public int UpgradeMaterialsCost => _upgradeMaterialsCost;
    // =============================================

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;

        Debug.Log($"[Building] {_buildingName} Start() called");

        if (_constructionTime > 0f)
        {
            StartConstruction();
        }
        else
        {
            // instant build
            CompleteConstruction();
        }
    }

    protected virtual void StartConstruction()
    {
        // Prevent restarting construction if already constructing or completed
        if (_isConstructing)
        {
            Debug.LogWarning($"[Building] {_buildingName} already constructing!");
            return;
        }
        
        _isConstructing = true;
        ConstructionProgress = 0f;

        Debug.Log($"[Building] {_buildingName} construction STARTED! Duration: {_constructionTime}s");

        if (_constructionEffect != null)
            _constructionEffect.SetActive(true);

        // Use coroutine for reliable timing
        _constructionCoroutine = StartCoroutine(ConstructionTimer());
    }

    private IEnumerator ConstructionTimer()
    {
        float elapsed = 0f;

        while (elapsed < _constructionTime)
        {
            elapsed += Time.deltaTime;
            ConstructionProgress = elapsed / _constructionTime;
            
            // Optional: Log every second
            if (Mathf.FloorToInt(elapsed) > Mathf.FloorToInt(elapsed - Time.deltaTime))
            {
                Debug.Log($"[Building] {_buildingName} construction: {Mathf.FloorToInt(elapsed)}/{_constructionTime}s");
            }
            
            yield return null;
        }

        CompleteConstruction();
    }

    protected virtual void CompleteConstruction()
    {
        // Prevent completing twice
        if (!_isConstructing)
        {
            Debug.Log($"[Building] {_buildingName} CompleteConstruction called but not constructing");
            return;
        }
        
        _isConstructing = false;
        ConstructionProgress = 1f;

        if (_constructionCoroutine != null)
        {
            StopCoroutine(_constructionCoroutine);
            _constructionCoroutine = null;
        }

        if (_constructionEffect != null)
            _constructionEffect.SetActive(false);

        Debug.Log($"[Building] {_buildingName} construction COMPLETE!");

        // Notify any systems listening
        EventManager.TriggerEvent("BuildingCompleted", this);

        // Lightweight hook for children
        SendMessage("OnConstructionComplete", SendMessageOptions.DontRequireReceiver);
    }

    protected virtual void OnDisable()
    {
        // Clean up coroutine if object is disabled
        if (_constructionCoroutine != null)
        {
            StopCoroutine(_constructionCoroutine);
            _constructionCoroutine = null;
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isConstructing) return;

        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        if (_currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    private void DestroyBuilding()
    {
        EventManager.TriggerEvent("BuildingDestroyed", this);
        Destroy(gameObject);
    }

    // ========== NEW: UPGRADE SYSTEM METHODS ==========
    public bool TryUpgrade()
    {
        if (!CanUpgrade)
        {
            Debug.LogWarning($"{_buildingName} is already max level!");
            return false;
        }
        
        // Check if ResourceManager exists
        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("ResourceManager not found! Cannot upgrade.");
            return false;
        }
        
        if (!ResourceManager.Instance.SpendResources(_upgradeGoldCost, _upgradeMaterialsCost, 0))
        {
            Debug.LogWarning($"Not enough resources to upgrade {_buildingName}!");
            return false;
        }
        
        _currentLevel++;
        OnUpgraded();
        
        Debug.Log($"{_buildingName} upgraded to level {_currentLevel}");
        return true;
    }

    protected virtual void OnUpgraded()
    {
        // Increase max health by 20% per level
        int oldMaxHealth = _maxHealth;
        _maxHealth = Mathf.RoundToInt(_maxHealth * 1.2f);
        _currentHealth = _maxHealth;
        
        Debug.Log($"{_buildingName} health increased: {oldMaxHealth} → {_maxHealth}");
        
        // Child classes override this for specific upgrades
    }
    // =================================================
}