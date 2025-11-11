using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Building Settings")]
    [SerializeField] private string _buildingName = "Building";
    [SerializeField] protected int _maxHealth = 200;
    [SerializeField] protected float _constructionTime = 5f;
    [SerializeField] private GameObject _constructionEffect;

    protected int _currentHealth;
    private float _constructionProgress;
    private bool _isConstructing = false;

    public bool IsConstructing => _isConstructing;
    public bool IsConstructed => !_isConstructing;

    public float ConstructionProgress =>
        _constructionTime <= 0f ? 1f : Mathf.Clamp01(_constructionProgress / _constructionTime);

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;

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

    protected virtual void Update()
    {
        if (!_isConstructing) return;

        _constructionProgress += Time.deltaTime;
        
        if (_constructionProgress >= _constructionTime)
        {
            CompleteConstruction();
        }
    }

    protected virtual void StartConstruction()
    {
        // Prevent restarting construction if already constructing or completed
        if (_isConstructing)
        {
            Debug.LogWarning($"[Building] {_buildingName} is already constructing! Ignoring duplicate call.");
            return;
        }
        
        _isConstructing = true;
        _constructionProgress = 0f;

        if (_constructionEffect != null)
            _constructionEffect.SetActive(true);
        
        Debug.Log($"[Building] {_buildingName} construction started. Time: {_constructionTime}s");
    }

    protected virtual void CompleteConstruction()
    {
        // Prevent completing twice
        if (!_isConstructing)
        {
            Debug.LogWarning($"[Building] {_buildingName} CompleteConstruction called but not constructing!");
            return;
        }
        
        _isConstructing = false;

        if (_constructionEffect != null)
            _constructionEffect.SetActive(false);

        Debug.Log($"[Building] {_buildingName} construction complete! IsConstructing: {_isConstructing}, IsConstructed: {IsConstructed}");

        // Notify any systems listening
        EventManager.TriggerEvent("BuildingCompleted", this);

        // Lightweight hook for children
        SendMessage("OnConstructionComplete", SendMessageOptions.DontRequireReceiver);
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
}