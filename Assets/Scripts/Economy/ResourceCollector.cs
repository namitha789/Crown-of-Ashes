using UnityEngine;

public class ResourceCollector : Building
{
    [Header("Income Settings")]
    [SerializeField] private int _goldIncomeAmount = 5;
    [SerializeField] private int _materialsIncomeAmount = 3;
    [SerializeField] private float _incomeInterval = 5f;
    
    private float _timeSinceLastIncome = 0f;
    private bool _isProducing = false;
    
    protected override void Start()
    {
        base.Start();
        
        Debug.Log($"[ResourceCollector] Start() - IsConstructing: {IsConstructing}");
        
        if (IsConstructed)
        {
            StartIncome();
        }
    }
    
    private void Update()
    {
        if (!_isProducing || IsConstructing) return;
        
        _timeSinceLastIncome += Time.deltaTime;
        
        if (_timeSinceLastIncome >= _incomeInterval)
        {
            GenerateIncome();
            _timeSinceLastIncome = 0f;
        }
    }
    
    protected override void CompleteConstruction()
    {
        base.CompleteConstruction();
        
        Debug.Log("[ResourceCollector] Construction complete - starting income!");
        StartIncome();
    }
    
    private void StartIncome()
    {
        if (_isProducing)
        {
            Debug.LogWarning("[ResourceCollector] Income already started!");
            return;
        }
        
        _isProducing = true;
        _timeSinceLastIncome = 0f;
        Debug.Log($"[ResourceCollector] ✓ INCOME STARTED! +{_goldIncomeAmount}G, +{_materialsIncomeAmount}M every {_incomeInterval}s");
    }
    
    private void GenerateIncome()
    {
        ResourceManager.Instance.AddResources(_goldIncomeAmount, _materialsIncomeAmount, 0);
        Debug.Log($"[ResourceCollector] 💰 Generated: +{_goldIncomeAmount}G, +{_materialsIncomeAmount}M");
    }

    // ========== NEW: UPGRADE OVERRIDE ==========
    protected override void OnUpgraded()
    {
        base.OnUpgraded(); // Call parent (increases health)
        
        // Store old values for logging
        int oldGold = _goldIncomeAmount;
        int oldMaterials = _materialsIncomeAmount;
        
        // Increase income by 2 gold and 1 material per level
        _goldIncomeAmount += 2;
        _materialsIncomeAmount += 1;
        
        Debug.Log($"ResourceCollector upgraded! Income: {oldGold}G → {_goldIncomeAmount}G, {oldMaterials}M → {_materialsIncomeAmount}M per {_incomeInterval}s");
    }
    // ===========================================
}