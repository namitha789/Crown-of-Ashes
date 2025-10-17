using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _materialsText;
    [SerializeField] private TextMeshProUGUI _influenceText;
    
    private void Start()
    {
        EventManager.StartListening("ResourcesUpdated", OnResourcesUpdated);
        UpdateDisplay(200, 100, 0); // Initial values
    }
    
    private void OnDestroy()
    {
        EventManager.StopListening("ResourcesUpdated", OnResourcesUpdated);
    }
    
    private void OnResourcesUpdated(object data)
    {
        if (data is ResourceData resourceData)
        {
            UpdateDisplay(resourceData.Gold, resourceData.Materials, resourceData.Influence);
        }
    }
    
    private void UpdateDisplay(int gold, int materials, int influence)
    {
        _goldText.text = $"Gold: {gold}";
        _materialsText.text = $"Materials: {materials}";
        _influenceText.text = $"Influence: {influence}";
    }
}

[System.Serializable]
public class ResourceData
{
    public int Gold;
    public int Materials;
    public int Influence;
    
    public ResourceData(int gold, int materials, int influence)
    {
        Gold = gold;
        Materials = materials;
        Influence = influence;
    }
}
