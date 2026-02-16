using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class SelectionInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private SelectionManager _selectionManager;
    
    private void Update()
    {
        UpdateInfo(); 
    }
    
    
    private void UpdateInfo()
    {
        List<Unit> selected = _selectionManager.GetSelectedUnits();
        
        if (selected.Count == 0)
        {
            _infoText.text = "No Units Selected";
            return;
        }
        
        if (selected.Count == 1)
        {
            Unit unit = selected[0];
            _infoText.text = $"{unit.gameObject.name}\n" +
                            $"HP: {unit.CurrentHealth}/{unit.MaxHealth}\n" +
                            $"Damage: {unit.AttackDamage}\n" +
                            $"Range: {unit.AttackRange}m";
        }
        else
        {
            _infoText.text = $"{selected.Count} Units Selected\n" +
                            $"Avg HP: {GetAverageHealth(selected):F0}%";
        }
    }

    private float GetAverageHealth(List<Unit> units)
    {
        if (units.Count == 0) return 0f;
        
        float totalPercent = 0f;
        foreach (Unit unit in units)
        {
            totalPercent += (unit.CurrentHealth / (float)unit.MaxHealth) * 100f;
        }
        
        return totalPercent / units.Count;
    }
}