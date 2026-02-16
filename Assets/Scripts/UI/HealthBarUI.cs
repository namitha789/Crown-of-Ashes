using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Unit _unit;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Text _hpText; // Optional: for displaying HP numbers
    
    [Header("Settings")]
    [SerializeField] private Vector3 _offset = new Vector3(0, 2, 0);
    [SerializeField] private bool _hideWhenFull = true;
    
    private Camera _mainCamera;
    private float _displayedHealth;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        
        if (_canvas != null)
        {
            _canvas.worldCamera = _mainCamera;
        }
        
        // Initialize displayed health to current health
        if (_unit != null)
        {
            _displayedHealth = _unit.CurrentHealth;
        }
        
        EventManager.StartListening("UnitHealthChanged", OnUnitHealthChanged);
        UpdateHealthBar();
    }
    
    private void OnDestroy()
    {
        EventManager.StopListening("UnitHealthChanged", OnUnitHealthChanged);
    }
    
    private void LateUpdate()
    {
        if (_mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);
        }
    }
    
    private void OnUnitHealthChanged(object data)
    {
        if (data is Unit unit && unit == _unit)
        {
            UpdateHealthBar();
        }
    }
    
    private void UpdateHealthBar()
    {
        if (_unit == null || _fillImage == null) return;
        
        float currentHealth = _unit.CurrentHealth;
        float maxHealth = _unit.MaxHealth;
        float healthPercent = currentHealth / maxHealth;
        
        // Smooth lerp to new value
        _displayedHealth = Mathf.Lerp(_displayedHealth, currentHealth, Time.deltaTime * 5f);
        float displayPercent = _displayedHealth / maxHealth;
        
        _fillImage.fillAmount = displayPercent;
        
        // Update color with smooth transitions
        if (displayPercent > 0.5f)
        {
            _fillImage.color = Color.Lerp(Color.yellow, Color.green, (displayPercent - 0.5f) * 2);
        }
        else
        {
            _fillImage.color = Color.Lerp(Color.red, Color.yellow, displayPercent * 2);
        }
        
        // Update text with actual value (optional)
        if (_hpText != null)
        {
            _hpText.text = $"{Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(maxHealth)}";
        }
        
        // Hide when at full health
        if (_hideWhenFull && _canvas != null)
        {
            _canvas.enabled = healthPercent < 0.99f;
        }
    }
}