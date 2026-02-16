using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType
    {
        Gold,
        Materials,
        Influence
    }
    
    [Header("Resource Settings")]
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private int _totalAmount = 500;
    [SerializeField] private int _gatherRate = 10;
    [SerializeField] private float _gatherInterval = 2f;
    
    private int _remainingAmount;
    private float _gatherTimer;
    private bool _isBeingGathered;
    
    private void Awake()
    {
        _remainingAmount = _totalAmount;
    }
    
    private void Update()
    {
        if (_isBeingGathered && _remainingAmount > 0)
        {
            _gatherTimer += Time.deltaTime;
            
            if (_gatherTimer >= _gatherInterval)
            {
                GatherResource();
                _gatherTimer = 0f;
            }
        }
    }
    
    private void GatherResource()
    {
        int amountToGather = Mathf.Min(_gatherRate, _remainingAmount);
        _remainingAmount -= amountToGather;
        
        switch (_resourceType)
        {
            case ResourceType.Gold:
                ResourceManager.Instance.AddResources(amountToGather, 0, 0);
                break;
            case ResourceType.Materials:
                ResourceManager.Instance.AddResources(0, amountToGather, 0);
                break;
            case ResourceType.Influence:
                ResourceManager.Instance.AddResources(0, 0, amountToGather);
                break;
        }
        
        Debug.Log($"Gathered {amountToGather} {_resourceType}. Remaining: {_remainingAmount}");
        
        if (_remainingAmount <= 0)
        {
            DepletedResource();
        }
    }
    
    private void DepletedResource()
    {
        Debug.Log($"{_resourceType} node depleted!");
        _isBeingGathered = false;
        // Visual feedback (could fade out or change material)
        GetComponent<Renderer>().material.color = Color.gray;
    }
    
    private void OnMouseDown()
    {
        if (_remainingAmount > 0)
        {
            _isBeingGathered = !_isBeingGathered;
            Debug.Log($"{_resourceType} gathering: {_isBeingGathered}");
        }
    }
}