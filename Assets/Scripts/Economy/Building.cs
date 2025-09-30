using UnityEngine;

   public abstract class Building : MonoBehaviour
   {
       [SerializeField] protected string _buildingName;
       [SerializeField] protected int _maxHealth = 100;
       [SerializeField] protected float _constructionTime = 5f;
       
       protected int _currentHealth;
       protected bool _isConstructed;
       protected float _constructionProgress;
       
       protected virtual void Awake()
       {
           _currentHealth = _maxHealth;
           _isConstructed = false;
           _constructionProgress = 0f;
       }
       
       protected virtual void Update()
       {
           if (!_isConstructed)
           {
               _constructionProgress += Time.deltaTime;
               
               if (_constructionProgress >= _constructionTime)
               {
                   CompleteConstruction();
               }
           }
       }
       
       protected virtual void CompleteConstruction()
       {
           _isConstructed = true;
           Debug.Log($"{_buildingName} construction completed!");
       }
       
       public virtual void TakeDamage(int damage)
       {
           _currentHealth -= damage;
           
           if (_currentHealth <= 0)
           {
               DestroyBuilding();
           }
       }
       
       protected virtual void DestroyBuilding()
       {
           Debug.Log($"{_buildingName} has been destroyed!");
           Destroy(gameObject);
       }
   }