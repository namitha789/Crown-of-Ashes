using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    [Header("Building Buttons")]
    [SerializeField] private Button _barracksButton;
    [SerializeField] private Button _resourceCollectorButton;
    [SerializeField] private Button _towerButton;
    
    [Header("References")]
    [SerializeField] private BuildingPlacer _buildingPlacer;
    
    private void Start()
    {
        _barracksButton.onClick.AddListener(OnBarracksClicked);
        _resourceCollectorButton.onClick.AddListener(OnResourceCollectorClicked);
        _towerButton.onClick.AddListener(OnTowerClicked);
    }
    
    private void OnBarracksClicked()
    {
        _buildingPlacer.StartPlacingBarracks();
    }
    
    private void OnResourceCollectorClicked()
    {
        _buildingPlacer.StartPlacingCollector();
    }
    
    private void OnTowerClicked()
    {
        _buildingPlacer.StartPlacingTower();
    }
}