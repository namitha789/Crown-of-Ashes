using System.Collections.Generic;
   using UnityEngine;

   public class ObjectPool
   {
       private GameObject _prefab;
       private List<GameObject> _pooledObjects;
       
       public ObjectPool(GameObject prefab, int initialSize)
       {
           _prefab = prefab;
           _pooledObjects = new List<GameObject>();
           
           // Initialize the pool with the specified size
           for (int i = 0; i < initialSize; i++)
           {
               CreateNewObject();
           }
       }
       
       private GameObject CreateNewObject()
       {
           GameObject obj = Object.Instantiate(_prefab);
           obj.SetActive(false);
           _pooledObjects.Add(obj);
           return obj;
       }
       
       public GameObject GetObject()
       {
           // Find an inactive object in the pool
           foreach (GameObject obj in _pooledObjects)
           {
               if (!obj.activeInHierarchy)
               {
                   return obj;
               }
           }
           
           // If no inactive objects found, create a new one
           return CreateNewObject();
       }
   }