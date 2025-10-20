using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class SelectionInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private SelectionManager _selectionManager;
    
    private void Update()
    {
        UpdateSelectionInfo();
    }
    
    private void UpdateSelectionInfo()
    {
        if (_selectionManager == null) return;
        
        List<Unit> selectedUnits = _selectionManager.GetSelectedUnits();
        
        if (selectedUnits.Count == 0)
        {
            _infoText.text = "No units selected";
        }
        else if (selectedUnits.Count == 1)
        {
            _infoText.text = $"Selected: {selectedUnits[0].UnitName}";
        }
        else
        {
            _infoText.text = $"Selected: {selectedUnits.Count} units";
        }
    }
}

