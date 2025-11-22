using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandCenterHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private GameObject _warningPanel; // Warning text already set in scene, just show/hide panel

    [Header("Settings")]
    [SerializeField] private float _lowHealthThreshold = 0.3f;
    [SerializeField] private Color _normalColor = Color.green;
    [SerializeField] private Color _warningColor = Color.yellow;
    [SerializeField] private Color _criticalColor = Color.red;

    private CommandCenter _commandCenter;

    private void Start()
    {
        _commandCenter = CommandCenter.PlayerInstance;

        if (_commandCenter == null)
        {
            Debug.LogError("CommandCenterHealthUI: No player Command Center found!");
            gameObject.SetActive(false);
            return;
        }

        if (_warningPanel != null)
        {
            _warningPanel.SetActive(false);
        }

        EventManager.StartListening("CommandCenterDamaged", OnCommandCenterDamaged);

        UpdateHealthBar();
    }

    private void OnDestroy()
    {
        EventManager.StopListening("CommandCenterDamaged", OnCommandCenterDamaged);
    }

    private void OnCommandCenterDamaged(object data)
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (_commandCenter == null) return;

        float healthPercent = (float)_commandCenter.CurrentHealth / _commandCenter.MaxHealth;

        // Update fill amount
        if (_healthBarFill != null)
        {
            _healthBarFill.fillAmount = healthPercent;

            // Update color based on health
            if (healthPercent > 0.5f)
            {
                _healthBarFill.color = _normalColor;
            }
            else if (healthPercent > _lowHealthThreshold)
            {
                _healthBarFill.color = _warningColor;
            }
            else
            {
                _healthBarFill.color = _criticalColor;
            }
        }

        // Update text
        if (_healthText != null)
        {
            _healthText.text = $"HP: {_commandCenter.CurrentHealth}/{_commandCenter.MaxHealth}";
        }

        // Show/hide warning panel if low health (warning text is already set in the scene)
        if (_warningPanel != null)
        {
            if (healthPercent <= _lowHealthThreshold && !_commandCenter.IsDestroyed)
            {
                _warningPanel.SetActive(true);
            }
            else
            {
                _warningPanel.SetActive(false);
            }
        }
    }
}