using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OutpostHealthUI : MonoBehaviour
{
    [SerializeField] private SimpleOutpost _outpost;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _hpText;
    
    private Camera _mainCamera;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        
        if (_nameText != null)
        {
            _nameText.text = "Enemy Outpost";
        }
        
        UpdateHealthBar();
    }
    
    private void Update()
    {
        if (_outpost == null || _outpost.IsDestroyed)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // Always face camera
        if (_mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.transform.position);
        }
        
        UpdateHealthBar();
    }
    
    private void UpdateHealthBar()
    {
        if (_outpost == null) return;
        
        float healthPercent = _outpost.CurrentHealth / (float)_outpost.MaxHealth;
        
        _fillImage.fillAmount = healthPercent;
        
        // Color: Green → Red based on health
        // _fillImage.color = Color.Lerp(Color.red, Color.green, healthPercent);
        
        if (_hpText != null)
        {
            _hpText.text = $"{_outpost.CurrentHealth}/{_outpost.MaxHealth} HP";
        }
    }
}

