using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    
    [Header("Starting Resources")]
    [SerializeField] private int _startingGold = 200;
    [SerializeField] private int _startingMaterials = 100;
    [SerializeField] private int _startingInfluence = 0;
    
    private ResourceData _playerResources;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        _playerResources = new ResourceData(_startingGold, _startingMaterials, _startingInfluence);
    }
    
    private void Start()
    {
        UpdateResourcesUI();
    }
    
    public void AddResources(int gold, int materials, int influence)
    {
        _playerResources.Gold += gold;
        _playerResources.Materials += materials;
        _playerResources.Influence += influence;
        
        Debug.Log($"Resources added: Gold+{gold}, Materials+{materials}, Influence+{influence}");
        UpdateResourcesUI();
    }
    
    public bool SpendResources(int gold, int materials, int influence)
    {
        if (_playerResources.Gold >= gold && 
            _playerResources.Materials >= materials && 
            _playerResources.Influence >= influence)
        {
            _playerResources.Gold -= gold;
            _playerResources.Materials -= materials;
            _playerResources.Influence -= influence;
            
            Debug.Log($"Resources spent: Gold-{gold}, Materials-{materials}, Influence-{influence}");
            UpdateResourcesUI();
            return true;
        }
        
        Debug.LogWarning("Not enough resources!");
        return false;
    }
    
    public ResourceData GetPlayerResources()
    {
        return _playerResources;
    }
    
    private void UpdateResourcesUI()
    {
        EventManager.TriggerEvent("ResourcesUpdated", _playerResources);
    }
}