using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingInfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _buildingNameText;
    [SerializeField] private TextMeshProUGUI _buildingHealthText;
    [SerializeField] private Button _actionButton;
    [SerializeField] private TextMeshProUGUI _actionButtonText;
    [SerializeField] private TextMeshProUGUI _queueText;
    [SerializeField] private Slider _progressBar;
    
    // ========== NEW: UPGRADE UI ELEMENTS ==========
    [Header("Upgrade UI Elements")]
    [SerializeField] private GameObject _upgradePanel;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeButtonText;
    [SerializeField] private TextMeshProUGUI _upgradeInfoText;
    // ==============================================
    
    private Building _selectedBuilding;
    
    private void Start()
    {
        _panel.SetActive(false);
        
        if (_actionButton != null)
        {
            _actionButton.onClick.AddListener(OnActionButtonClicked);
        }
        
        // ========== NEW: SETUP UPGRADE BUTTON ==========
        if (_upgradeButton != null)
        {
            _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }
        // ===============================================
    }
    
    private void Update()
    {
        if (_selectedBuilding != null && _panel.activeSelf)
        {
            UpdateBuildingInfo();
        }
    }
    
    public void ShowBuilding(Building building)
    {
        _selectedBuilding = building;
        _panel.SetActive(true);
        UpdateBuildingInfo();
    }
    
    public void HidePanel()
    {
        _panel.SetActive(false);
        _selectedBuilding = null;
    }
    
    private void UpdateBuildingInfo()
    {
        if (_selectedBuilding == null) return;
        
        // Update name and health
        _buildingNameText.text = "Barrack";
        _buildingHealthText.text = $"HP: {_selectedBuilding.CurrentHealth}/{_selectedBuilding.MaxHealth}";
        
        // ========== NEW: UPDATE UPGRADE INFO ==========
        UpdateUpgradeInfo();
        // ==============================================
        
        // Check if it's a barracks
        Barracks barracks = _selectedBuilding as Barracks;
        if (barracks != null)
        {
            UpdateBarracksInfo(barracks);
        }
        else
        {
            // Other building types - hide barracks-specific UI
            _actionButton.gameObject.SetActive(false);
            _queueText.gameObject.SetActive(false);
            _progressBar.gameObject.SetActive(false);
        }
    }
    
    private void UpdateBarracksInfo(Barracks barracks)
    {
        _actionButton.gameObject.SetActive(true);
        _queueText.gameObject.SetActive(true);
        _progressBar.gameObject.SetActive(true);
        
        _actionButtonText.text = "Train Warrior (5 Gold)";
        _queueText.text = $"Queue: {barracks.QueueCount}/5";
        
        if (barracks.IsProducing)
        {
            _progressBar.value = barracks.ProductionProgress;
        }
        else
        {
            _progressBar.value = 0f;
        }
    }
    
    private void OnActionButtonClicked()
    {
        Barracks barracks = _selectedBuilding as Barracks;
        if (barracks != null)
        {
            barracks.TrainWarrior();
        }
    }
    
    // ========== NEW: UPGRADE SYSTEM METHODS ==========
    private void UpdateUpgradeInfo()
    {
        if (_selectedBuilding == null) return;
        
        // Show/hide upgrade panel based on whether UI elements exist
        if (_upgradePanel != null)
        {
            _upgradePanel.SetActive(true);
        }
        
        // Update level text
        if (_levelText != null)
        {
            _levelText.text = $"Level {_selectedBuilding.CurrentLevel}/{_selectedBuilding.MaxLevel}";
        }
        
        // Update upgrade button
        if (_upgradeButton != null)
        {
            if (_selectedBuilding.CanUpgrade)
            {
                _upgradeButton.interactable = true;
                
                // Check if player has enough resources
                bool canAfford = false;
                if (ResourceManager.Instance != null)
                {
                    canAfford = ResourceManager.Instance.Gold >= _selectedBuilding.UpgradeGoldCost &&
                                ResourceManager.Instance.Materials >= _selectedBuilding.UpgradeMaterialsCost;
                }
                
                _upgradeButton.interactable = canAfford;
                
                if (_upgradeButtonText != null)
                {
                    _upgradeButtonText.text = $"Upgrade ({_selectedBuilding.UpgradeGoldCost}G, {_selectedBuilding.UpgradeMaterialsCost}M)";
                }
            }
            else
            {
                // Max level reached
                _upgradeButton.interactable = false;
                
                if (_upgradeButtonText != null)
                {
                    _upgradeButtonText.text = "Max Level";
                }
            }
        }
        
        // Update upgrade info text (shows what will improve)
        if (_upgradeInfoText != null)
        {
            if (_selectedBuilding.CanUpgrade)
            {
                string info = GetUpgradePreview();
                _upgradeInfoText.text = info;
            }
            else
            {
                _upgradeInfoText.text = "Building is at maximum level";
            }
        }
    }
    
    private string GetUpgradePreview()
    {
        string preview = "Next Level:\n";
        preview += $"• Health: +20%\n";
        
        // Tower-specific upgrades
        Tower tower = _selectedBuilding as Tower;
        if (tower != null)
        {
            preview += "• Damage: +10\n";
            preview += "• Range: +1";
            return preview;
        }
        
        // ResourceCollector-specific upgrades
        ResourceCollector collector = _selectedBuilding as ResourceCollector;
        if (collector != null)
        {
            preview += "• Gold Income: +2\n";
            preview += "• Materials Income: +1";
            return preview;
        }
        
        return preview;
    }
    
    public void OnUpgradeButtonClicked()
    {
        if (_selectedBuilding != null)
        {
            bool upgraded = _selectedBuilding.TryUpgrade();
            
            if (upgraded)
            {
                UpdateBuildingInfo(); // Refresh UI
                Debug.Log($"[BuildingInfoUI] Building upgraded successfully!");
            }
            else
            {
                Debug.Log($"[BuildingInfoUI] Upgrade failed");
            }
        }
    }
    // =================================================
}