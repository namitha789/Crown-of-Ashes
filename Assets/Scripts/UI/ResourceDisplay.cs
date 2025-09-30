using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [Header("Resource Text References")]
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _materialsText;
    [SerializeField] private TextMeshProUGUI _influenceText;
    
    private void Start()
    {
        Debug.Log("ResourceDisplay Started!");
        EventManager.StartListening("ResourcesUpdated", OnResourcesUpdated);
        UpdateDisplay(ResourceManager.Instance.GetPlayerResources());
    }
    
    private void OnDestroy()
    {
        EventManager.StopListening("ResourcesUpdated", OnResourcesUpdated);
    }
    
    private void OnResourcesUpdated(object resourceData)
    {
        if (resourceData is ResourceData data)
        {
            Debug.Log($"UI Updating: Gold={data.Gold}, Materials={data.Materials}, Influence={data.Influence}");
            UpdateDisplay(data);
        }
    }
    
    private void UpdateDisplay(ResourceData data)
    {
        // Format with labels for clarity
        _goldText.text = $"Gold: {data.Gold}";
        _materialsText.text = $"Materials: {data.Materials}";
        _influenceText.text = $"Influence: {data.Influence}";
    }
}