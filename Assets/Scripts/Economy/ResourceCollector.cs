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
    
    // Only use the override method - remove OnConstructionComplete()
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
}