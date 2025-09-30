using UnityEngine;

public class ResourceTester : MonoBehaviour
{
    public void AddGold()
    {
        ResourceManager.Instance.AddResources(100, 0, 0);
    }
    
    public void AddMaterials()
    {
        ResourceManager.Instance.AddResources(0, 50, 0);
    }
    
    public void AddInfluence()
    {
        ResourceManager.Instance.AddResources(0, 0, 10);
    }
    
    public void SpendResources()
    {
        ResourceManager.Instance.SpendResources(50, 25, 5);
    }
}