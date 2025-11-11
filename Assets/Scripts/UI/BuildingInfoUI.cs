using UnityEngine;
using UnityEngine.UI;
using TMPro;

[AddComponentMenu("RTS/UI/Building Info UI")]
public class BuildingInfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _buildingNameText;
    [SerializeField] private TextMeshProUGUI _buildingHealthText;

    [Header("Health UI (optional)")]
    [SerializeField] private Slider _healthBar; // 0..1

    [Header("Action UI (Barracks)")]
    [SerializeField] private Button _actionButton;
    [SerializeField] private TextMeshProUGUI _actionButtonText;
    [SerializeField] private TextMeshProUGUI _queueText;
    [SerializeField] private Slider _progressBar;

    [Header("Upgrade UI")]
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeButtonText;

    private Building _selectedBuilding;

    private void Start()
    {
        if (_panel != null) _panel.SetActive(false);
        if (_actionButton != null) _actionButton.onClick.AddListener(OnActionButtonClicked);
        if (_upgradeButton != null) _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
    }

    private void Update()
    {
        if (_panel == null || !_panel.activeSelf) return;
        if (_selectedBuilding == null) { HidePanel(); return; }
        UpdateBuildingInfo();
    }

    public void ShowBuilding(Building building)
    {
        _selectedBuilding = building;
        if (_panel != null) _panel.SetActive(true);
        UpdateBuildingInfo();
    }

    public void HidePanel()
    {
        if (_panel != null) _panel.SetActive(false);
        _selectedBuilding = null;
    }

    private void UpdateBuildingInfo()
    {
        if (_selectedBuilding == null) return;

        // Name with Level
        if (_buildingNameText != null)
            _buildingNameText.text = $"{_selectedBuilding.BuildingName} (Lv. {_selectedBuilding.CurrentLevel})";

        // Health
        if (_buildingHealthText != null)
            _buildingHealthText.text = $"HP: {_selectedBuilding.CurrentHealth}/{_selectedBuilding.MaxHealth}";

        if (_healthBar != null)
        {
            float norm = _selectedBuilding.MaxHealth > 0
                ? (float)_selectedBuilding.CurrentHealth / _selectedBuilding.MaxHealth
                : 0f;
            _healthBar.value = Mathf.Clamp01(norm);
        }

        // Barracks-specific
        var barracks = _selectedBuilding as Barracks;
        if (barracks != null)
        {
            if (_actionButton != null) _actionButton.gameObject.SetActive(true);
            if (_queueText != null) _queueText.gameObject.SetActive(true);
            if (_progressBar != null) _progressBar.gameObject.SetActive(true);

            if (_actionButtonText != null) _actionButtonText.text = "Train Warrior (50 Gold)";
            if (_queueText != null) _queueText.text = $"Queue: {barracks.QueueCount}/5";
            if (_progressBar != null) _progressBar.value = barracks.IsProducing ? barracks.ProductionProgress : 0f;
        }
        else
        {
            if (_actionButton != null) _actionButton.gameObject.SetActive(false);
            if (_queueText != null) _queueText.gameObject.SetActive(false);
            if (_progressBar != null) _progressBar.gameObject.SetActive(false);
        }

        // Upgrade button (all buildings)
        if (_upgradeButton != null && _upgradeButtonText != null)
        {
            if (_selectedBuilding.CanUpgrade)
            {
                _upgradeButton.interactable = true;
                _upgradeButtonText.text = $"Upgrade ({GetUpgradeCostText(_selectedBuilding)})";
                _upgradeButton.gameObject.SetActive(true);
            }
            else
            {
                _upgradeButton.interactable = false;
                _upgradeButtonText.text = "Max Level";
                _upgradeButton.gameObject.SetActive(true);
            }
        }
    }

    private string GetUpgradeCostText(Building b)
    {
        // We can’t read private fields directly—just surface a simple display:
        // If you want exact numbers in text, move costs to public properties in Building.
        return "100G, 50M"; // adjust if you made costs public; otherwise leave this label generic
    }

    private void OnActionButtonClicked()
    {
        var barracks = _selectedBuilding as Barracks;
        if (barracks != null) barracks.TrainWarrior();
    }

    public void OnUpgradeButtonClicked()
    {
        if (_selectedBuilding == null) return;
        bool upgraded = _selectedBuilding.TryUpgrade();
        if (upgraded) UpdateBuildingInfo();
    }
}
