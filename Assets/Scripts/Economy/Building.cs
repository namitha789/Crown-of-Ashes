using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Building Settings")]
    [SerializeField] private string _buildingName = "Building";
    [SerializeField] private int _maxHealth = 200;
    [SerializeField] private float _constructionTime = 5f;
    [SerializeField] private GameObject _constructionEffect;
    
    private int _currentHealth;
    private float _constructionProgress;
    private bool _isConstructing = true;
    
    public bool IsConstructing => _isConstructing;
    public float ConstructionProgress => _constructionProgress / _constructionTime;
    
    private void Start()
    {
        _currentHealth = _maxHealth;
        
        if (_constructionEffect != null)
        {
            _constructionEffect.SetActive(true);
        }
    }
    
    private void Update()
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
    
    private void CompleteConstruction()
    {
        _isConstructing = false;
        
        if (_constructionEffect != null)
        {
            _constructionEffect.SetActive(false);
        }
        
        EventManager.TriggerEvent("BuildingCompleted", this);
        Debug.Log($"{_buildingName} construction complete!");
    }
    
    public void TakeDamage(int damage)
    {
        if (_isConstructing) return;
        
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        
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