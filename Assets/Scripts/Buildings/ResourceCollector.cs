using UnityEngine;

public class ResourceCollector : Building
{
    [Header("Income Settings")]
    [SerializeField] private int _goldIncomeAmount = 5;
    [SerializeField] private int _materialsIncomeAmount = 3;
    [SerializeField] private float _incomeInterval = 5f; // seconds

    private float _timeSinceLastIncome = 0f;
    private bool _isProducing = false;

    protected override void Start()
    {
        base.Start();

        // If prefab has 0 construction time, begin immediately
        if (IsConstructed)
            StartIncome();
    }

    private void Update()
    {
        base.Update(); // keep the base construction timer

        if (!_isProducing || !IsConstructed) return;

        _timeSinceLastIncome += Time.deltaTime;
        if (_timeSinceLastIncome >= _incomeInterval)
        {
            GenerateIncome();
            _timeSinceLastIncome = 0f;
        }
    }

    private void StartIncome()
    {
        _isProducing = true;
        _timeSinceLastIncome = 0f;
        Debug.Log($"{gameObject.name} started generating resources");
    }

    private void GenerateIncome()
    {
        // Use convenience methods (updates UI)
        ResourceManager.Instance.AddGold(_goldIncomeAmount);
        ResourceManager.Instance.AddMaterials(_materialsIncomeAmount);

        Debug.Log($"Resource Collector generated: +{_goldIncomeAmount} Gold, +{_materialsIncomeAmount} Materials");
        EventManager.TriggerEvent("ResourceGenerated", this);
    }

    // Called from Building.CompleteConstruction via SendMessage
    public void OnConstructionComplete()
    {
        StartIncome();
    }

    protected override void OnUpgraded()
    {
        base.OnUpgraded();
        // Increase income by 2/1 per level
        _goldIncomeAmount += 2;
        _materialsIncomeAmount += 1;

        Debug.Log($"Resource Collector upgraded! New income: +{_goldIncomeAmount}G, +{_materialsIncomeAmount}M every {_incomeInterval}s");
    }
}
