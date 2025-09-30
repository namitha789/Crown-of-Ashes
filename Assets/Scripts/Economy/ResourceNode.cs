using UnityEngine;

   public class ResourceNode : MonoBehaviour
   {
       public enum ResourceType
       {
           Gold,
           Materials,
           Influence
       }
       
       [SerializeField] private ResourceType _resourceType;
       [SerializeField] private int _resourceAmount = 100;
       [SerializeField] private int _harvestRate = 5;
       [SerializeField] private float _harvestCooldown = 2f;
       [SerializeField] private GameObject _visualModel;
       [SerializeField] private GameObject _depletedModel;
       
       private bool _isActive = true;
       private float _harvestTimer;
       
       public ResourceType Type => _resourceType;
       public int RemainingAmount => _resourceAmount;
       public bool IsActive => _isActive;
       
       public void HarvestResource(Harvester harvester)
       {
           if (!_isActive || _resourceAmount <= 0)
               return;
               
           if (_harvestTimer <= 0)
           {
               int amountToHarvest = Mathf.Min(_harvestRate, _resourceAmount);
               _resourceAmount -= amountToHarvest;
               
               // Return resources based on type
               switch (_resourceType)
               {
                   case ResourceType.Gold:
                       ResourceManager.Instance.AddResources(amountToHarvest, 0, 0);
                       break;
                   case ResourceType.Materials:
                       ResourceManager.Instance.AddResources(0, amountToHarvest, 0);
                       break;
                   case ResourceType.Influence:
                       ResourceManager.Instance.AddResources(0, 0, amountToHarvest);
                       break;
               }
               
               _harvestTimer = _harvestCooldown;
               
               // Check if depleted
               if (_resourceAmount <= 0)
               {
                   DepletedResource();
               }
           }
       }
       
       private void Update()
       {
           if (_harvestTimer > 0)
           {
               _harvestTimer -= Time.deltaTime;
           }
       }
       
       private void DepletedResource()
       {
           _isActive = false;
           
           if (_visualModel != null)
           {
               _visualModel.SetActive(false);
           }
           
           if (_depletedModel != null)
           {
               _depletedModel.SetActive(true);
           }
       }
   }
