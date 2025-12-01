using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Starting Resources")]
    [SerializeField] private int _startingGold = 500;
    [SerializeField] private int _startingMaterials = 300;
    [SerializeField] private int _startingInfluence = 0;

    [Header("Audio")]
    [SerializeField] private AudioClip _spendGoldSound;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _spendSoundVolume = 1f;

    private ResourceData _playerResources;

    // ========== NEW: PUBLIC PROPERTIES FOR UPGRADE SYSTEM ==========
    public int Gold => _playerResources.Gold;
    public int Materials => _playerResources.Materials;
    public int Influence => _playerResources.Influence;
    public int Food => _playerResources.Influence; // Alias for compatibility
    // ===============================================================
    
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
    
    public void SetStartingResources(int gold, int materials, int influence)
    {
        _playerResources = new ResourceData(gold, materials, influence);
        UpdateResourcesUI();
    }
    
    public void AddResources(int gold, int materials, int influence)
    {
        _playerResources.Gold += gold;
        _playerResources.Materials += materials;
        _playerResources.Influence += influence;
        
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

            // Play spend sound if any gold was spent
            if (gold > 0)
            {
                PlaySpendGoldSound();
            }

            UpdateResourcesUI();
            return true;
        }

        Debug.LogWarning("Not enough resources!");
        return false;
    }

    private void PlaySpendGoldSound()
    {
        if (_spendGoldSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_spendGoldSound, _spendSoundVolume);
        }
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