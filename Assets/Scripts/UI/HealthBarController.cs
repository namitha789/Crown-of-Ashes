using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Unit _unit;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Canvas _canvas;
    
    [Header("Settings")]
    [SerializeField] private Vector3 _offset = new Vector3(0, 2, 0);
    [SerializeField] private bool _hideWhenFull = true;
    
    private Camera _mainCamera;
    private float _maxHealth;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        
        if (_unit != null)
        {
            _maxHealth = _unit.MaxHealth;
            UpdateHealthBar();
        }
    }
    
    private void Update()
    {
        if (_unit == null || _unit.IsDead)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // Position above unit
        transform.position = _unit.transform.position + _offset;
        
        // Face camera
        if (_mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.transform.position);
        }
        
        UpdateHealthBar();
    }
    
    private void UpdateHealthBar()
    {
        if (_unit == null) return;
        
        float currentHealth = _unit.CurrentHealth;
        float healthPercent = currentHealth / _maxHealth;
        
        // Update fill amount
        _fillImage.fillAmount = healthPercent;
        
        // Update color (green → yellow → red)
        if (healthPercent > 0.5f)
        {
            _fillImage.color = Color.Lerp(Color.yellow, Color.green, (healthPercent - 0.5f) * 2);
        }
        else
        {
            _fillImage.color = Color.Lerp(Color.red, Color.yellow, healthPercent * 2);
        }
        
        // Update text
        if (_hpText != null)
        {
            _hpText.text = $"{Mathf.CeilToInt(currentHealth)}/{Mathf.CeilToInt(_maxHealth)}";
        }
        
        // Hide when at full health
        if (_hideWhenFull)
        {
            _canvas.enabled = healthPercent < 1f;
        }
    }
    
    public void Initialize(Unit unit)
    {
        _unit = unit;
        _maxHealth = unit.MaxHealth;
        UpdateHealthBar();
    }
}
