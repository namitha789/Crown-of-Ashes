using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    [Header("Building Buttons")]
    [SerializeField] private Button _barracksButton;
    [SerializeField] private Button _resourceCollectorButton;
    [SerializeField] private Button _towerButton;
    
    private void Start()
    {
        _barracksButton.onClick.AddListener(OnBarracksClicked);
        _resourceCollectorButton.onClick.AddListener(OnResourceCollectorClicked);
        _towerButton.onClick.AddListener(OnTowerClicked);
    }
    
    private void OnBarracksClicked()
    {
        Debug.Log("Barracks button clicked");
        // Will be connected to BuildingPlacer in Abhinav's implementation
    }
    
    private void OnResourceCollectorClicked()
    {
        Debug.Log("Resource Collector button clicked");
    }
    
    private void OnTowerClicked()
    {
        Debug.Log("Tower button clicked");
    }
}
