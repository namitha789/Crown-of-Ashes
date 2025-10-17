using UnityEngine;

   public class ResourceManager : MonoBehaviour
   {
       public static ResourceManager Instance { get; private set; }
       
       [SerializeField] private ResourceData _playerResources;
       
       private void Awake()
       {
           if (Instance != null && Instance != this)
           {
               Destroy(gameObject);
               return;
           }
           
           Instance = this;
           
           // Initialize starting resources
           _playerResources = new ResourceData
           {
               Gold = 200,
               Materials = 100,
               Influence = 0
           };
       }
       
       private void Start()
       {
           // Broadcast initial resources
           UpdateResourcesUI();
       }
       
       public void AddResources(int gold, int materials, int influence)
       {
           _playerResources.Gold += gold;
           _playerResources.Materials += materials;
           _playerResources.Influence += influence;
           
           UpdateResourcesUI();
       }
       
       public bool SpendResources(int gold, int materials, int influence)
       {
           if (_playerResources.Gold >= gold && 
               _playerResources.Materials >= materials && 
               _playerResources.Influence >= influence)
           {
               _playerResources.Gold -= gold;
               _playerResources.Materials -= materials;
               _playerResources.Influence -= influence;
               
               UpdateResourcesUI();
               return true;
           }
           
           return false;
       }
       
       public ResourceData GetPlayerResources()
       {
           return _playerResources;
       }
       
       private void UpdateResourcesUI()
       {
           EventManager.TriggerEvent("ResourcesUpdated", _playerResources);
       }
   }
   
