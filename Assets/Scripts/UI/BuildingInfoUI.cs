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
    
    private Building _selectedBuilding;
    
    private void Start()
    {
        _panel.SetActive(false);
        
        if (_actionButton != null)
        {
            _actionButton.onClick.AddListener(OnActionButtonClicked);
        }
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
        _buildingNameText.text = _selectedBuilding.BuildingName;
        _buildingHealthText.text = $"HP: {_selectedBuilding.CurrentHealth}/{_selectedBuilding.MaxHealth}";
        
        // Check if it's a barracks
        Barracks barracks = _selectedBuilding as Barracks;
        if (barracks != null)
        {
            UpdateBarracksInfo(barracks);
        }
        else
        {
            // Other building types
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
        
        _actionButtonText.text = "Train Warrior (50 Gold)";
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
}