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
        Debug.Log("[CHK] ResourceManager.Awake");
    }

    private void Start()
    {
        UpdateResourcesUI();
    }

    public void SetStartingResources(int gold, int materials, int influence)
    {
        _playerResources = new ResourceData(gold, materials, influence);
        UpdateResourcesUI();
        Debug.Log("[CHK] ResourceManager.SetStartingResources");
    }

    public void AddResources(int gold, int materials, int influence)
    {
        _playerResources.Gold += gold;
        _playerResources.Materials += materials;
        _playerResources.Influence += influence;

        UpdateResourcesUI();
        Debug.Log("[CHK] ResourceManager.AddResources");
    }

    /// <summary>
    /// Spend an exact combination of resources. Returns true if successful.
    /// </summary>
    public bool SpendResources(int gold, int materials, int influence)
    {
        if (_playerResources.Gold >= gold &&
            _playerResources.Materials >= materials &&
            _playerResources.Influence >= influence)
        {
            _playerResources.Gold -= gold;
            _playerResources.Materials -= materials;
            _playerResources.Influence -= influence;

            UpdateResourcesUI();
            Debug.Log("[CHK] ResourceManager.SpendResources");
            return true;
        }

        Debug.LogWarning("Not enough resources!");
        return false;
    }


        public void AddGold(int amount)
        {
            if (amount == 0) return;
            _playerResources.Gold += amount;
            UpdateResourcesUI();
    #if CHK
            DebugCheckpoint.Hit($"ResourceManager.AddGold(+{amount})");
    #endif
        }

        public void AddMaterials(int amount)
        {
            if (amount == 0) return;
            _playerResources.Materials += amount;
            UpdateResourcesUI();
    #if CHK
            DebugCheckpoint.Hit($"ResourceManager.AddMaterials(+{amount})");
    #endif
        }





    /// <summary>
    /// Convenience method used by Barracks: only spend gold.
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount <= 0) return true;

        if (_playerResources.Gold >= amount)
        {
            _playerResources.Gold -= amount;
            UpdateResourcesUI();
            Debug.Log("[CHK] ResourceManager.SpendGold");
            return true;
        }

        Debug.LogWarning($"Not enough gold! Have {_playerResources.Gold}, need {amount}");
        return false;
    }

    public ResourceData GetPlayerResources()
    {
        return _playerResources;
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    /// <summary>
    /// Test hook for EditMode/PlayMode tests.
    /// </summary>
    public void Debug_SetPlayerResources(ResourceData data)
    {
        _playerResources = data;
        UpdateResourcesUI();
        Debug.Log("[CHK] ResourceManager.Debug_SetPlayerResources");
    }
#endif

    private void UpdateResourcesUI()
    {
        EventManager.TriggerEvent("ResourcesUpdated", _playerResources);
    }
}
