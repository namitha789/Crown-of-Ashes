using UnityEngine;

public class UpgradeTestScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Building _buildingToTest;
    
    [Header("Test Settings")]
    [SerializeField] private KeyCode _upgradeKey = KeyCode.U;
    [SerializeField] private KeyCode _infoKey = KeyCode.I;
    
    private void Update()
    {
        // Press U key to upgrade
        if (Input.GetKeyDown(_upgradeKey))
        {
            if (_buildingToTest != null)
            {
                Debug.Log("========================================");
                Debug.Log("=== UPGRADE ATTEMPT ===");
                Debug.Log($"Building: {_buildingToTest.BuildingName}");
                Debug.Log($"Current Level: {_buildingToTest.CurrentLevel}/{_buildingToTest.MaxLevel}");
                Debug.Log($"Cost: {_buildingToTest.UpgradeGoldCost}G, {_buildingToTest.UpgradeMaterialsCost}M");
                
                if (ResourceManager.Instance != null)
                {
                    Debug.Log($"Player Resources: {ResourceManager.Instance.Gold}G, {ResourceManager.Instance.Materials}M");
                }
                
                bool success = _buildingToTest.TryUpgrade();
                
                if (success)
                {
                    Debug.Log($"✅ UPGRADE SUCCESSFUL! Now level {_buildingToTest.CurrentLevel}");
                    ShowBuildingStats();
                }
                else
                {
                    Debug.Log("❌ UPGRADE FAILED!");
                }
                Debug.Log("========================================");
            }
            else
            {
                Debug.LogError("❌ No building assigned to UpgradeTestScript!");
            }
        }
        
        // Press I key to show info
        if (Input.GetKeyDown(_infoKey))
        {
            if (_buildingToTest != null)
            {
                Debug.Log("========================================");
                Debug.Log("=== BUILDING INFO ===");
                ShowBuildingStats();
                Debug.Log("========================================");
            }
        }
    }
    
    private void ShowBuildingStats()
    {
        if (_buildingToTest == null) return;
        
        Debug.Log($"📋 Building: {_buildingToTest.BuildingName}");
        Debug.Log($"📊 Level: {_buildingToTest.CurrentLevel}/{_buildingToTest.MaxLevel}");
        Debug.Log($"❤️ Health: {_buildingToTest.CurrentHealth}/{_buildingToTest.MaxHealth}");
        Debug.Log($"⬆️ Can Upgrade: {_buildingToTest.CanUpgrade}");
        
        if (_buildingToTest.CanUpgrade)
        {
            Debug.Log($"💰 Next Upgrade Cost: {_buildingToTest.UpgradeGoldCost}G, {_buildingToTest.UpgradeMaterialsCost}M");
        }
        
        // Tower-specific stats
        Tower tower = _buildingToTest as Tower;
        if (tower != null)
        {
            // Use reflection to get private fields for display
            var damageField = tower.GetType().GetField("_attackDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var rangeField = tower.GetType().GetField("_attackRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (damageField != null && rangeField != null)
            {
                Debug.Log($"⚔️ Attack Damage: {damageField.GetValue(tower)}");
                Debug.Log($"🎯 Attack Range: {rangeField.GetValue(tower)}");
            }
        }
        
        // ResourceCollector-specific stats
        ResourceCollector collector = _buildingToTest as ResourceCollector;
        if (collector != null)
        {
            var goldField = collector.GetType().GetField("_goldIncomeAmount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var materialsField = collector.GetType().GetField("_materialsIncomeAmount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var intervalField = collector.GetType().GetField("_incomeInterval", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (goldField != null && materialsField != null && intervalField != null)
            {
                Debug.Log($"💰 Gold Income: {goldField.GetValue(collector)} per {intervalField.GetValue(collector)}s");
                Debug.Log($"🔨 Materials Income: {materialsField.GetValue(collector)} per {intervalField.GetValue(collector)}s");
            }
        }
    }
}
