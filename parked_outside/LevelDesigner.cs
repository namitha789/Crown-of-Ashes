using UnityEditor;
   using UnityEngine;
   using System.Collections.Generic;

   public class LevelDesigner : EditorWindow
   {
       private GameObject _terrainPrefab;
       private GameObject _resourceNodePrefab;
       private GameObject _treePrefab;
       private GameObject _rockPrefab;
       
       private float _gridSize = 1f;
       private Vector2Int _mapSize = new Vector2Int(20, 20);
       
       [MenuItem("Tools/Crown of Ashes/Level Designer")]
       public static void ShowWindow()
       {
           GetWindow<LevelDesigner>("Level Designer");
       }
       
       private void OnGUI()
       {
           GUILayout.Label("Level Design Tools", EditorStyles.boldLabel);
           
           _terrainPrefab = EditorGUILayout.ObjectField("Terrain Prefab", _terrainPrefab, typeof(GameObject), false) as GameObject;
           _resourceNodePrefab = EditorGUILayout.ObjectField("Resource Node Prefab", _resourceNodePrefab, typeof(GameObject), false) as GameObject;
           _treePrefab = EditorGUILayout.ObjectField("Tree Prefab", _treePrefab, typeof(GameObject), false) as GameObject;
           _rockPrefab = EditorGUILayout.ObjectField("Rock Prefab", _rockPrefab, typeof(GameObject), false) as GameObject;
           
           _gridSize = EditorGUILayout.FloatField("Grid Size", _gridSize);
           _mapSize = EditorGUILayout.Vector2IntField("Map Size", _mapSize);
           
           GUILayout.Space(10);
           
           if (GUILayout.Button("Create Empty Terrain"))
           {
               CreateEmptyTerrain();
           }
           
           if (GUILayout.Button("Add Random Resource Nodes"))
           {
               AddRandomResourceNodes();
           }
           
           if (GUILayout.Button("Add Random Trees"))
           {
               AddRandomTrees();
           }
           
           if (GUILayout.Button("Add Random Rocks"))
           {
               AddRandomRocks();
           }
       }
       
       private void CreateEmptyTerrain()
       {
           if (_terrainPrefab == null)
           {
               Debug.LogError("Terrain prefab is not assigned!");
               return;
           }
           
           GameObject terrainParent = new GameObject("Terrain");
           
           for (int x = 0; x < _mapSize.x; x++)
           {
               for (int z = 0; z < _mapSize.y; z++)
               {
                   Vector3 position = new Vector3(x * _gridSize, 0, z * _gridSize);
                   GameObject terrainTile = PrefabUtility.InstantiatePrefab(_terrainPrefab) as GameObject;
                   terrainTile.transform.position = position;
                   terrainTile.transform.SetParent(terrainParent.transform);
               }
           }
       }
       
       private void AddRandomResourceNodes()
       {
           if (_resourceNodePrefab == null)
           {
               Debug.LogError("Resource node prefab is not assigned!");
               return;
           }
           
           GameObject nodesParent = GameObject.Find("ResourceNodes");
           if (nodesParent == null)
           {
               nodesParent = new GameObject("ResourceNodes");
           }
           
           // Add 10% of map size as resource nodes
           int nodesToAdd = Mathf.FloorToInt(_mapSize.x * _mapSize.y * 0.1f);
           
           for (int i = 0; i < nodesToAdd; i++)
           {
               float x = Random.Range(0, _mapSize.x * _gridSize);
               float z = Random.Range(0, _mapSize.y * _gridSize);
               Vector3 position = new Vector3(x, 0, z);
               
               GameObject node = PrefabUtility.InstantiatePrefab(_resourceNodePrefab) as GameObject;
               node.transform.position = position;
               node.transform.SetParent(nodesParent.transform);
               
               // Randomly assign resource type
               ResourceNode resourceNode = node.GetComponent<ResourceNode>();
               if (resourceNode != null)
               {
                   int resourceType = Random.Range(0, 3);
                   resourceNode.GetType().GetField("_resourceType").SetValue(resourceNode, resourceType);
               }
           }
       }
       
       private void AddRandomTrees()
       {
           if (_treePrefab == null)
           {
               Debug.LogError("Tree prefab is not assigned!");
               return;
           }
           
           GameObject treesParent = GameObject.Find("Trees");
           if (treesParent == null)
           {
               treesParent = new GameObject("Trees");
           }
           
           // Add 15% of map size as trees
           int treesToAdd = Mathf.FloorToInt(_mapSize.x * _mapSize.y * 0.15f);
           
           for (int i = 0; i < treesToAdd; i++)
           {
               float x = Random.Range(0, _mapSize.x * _gridSize);
               float z = Random.Range(0, _mapSize.y * _gridSize);
               Vector3 position = new Vector3(x, 0, z);
               
               GameObject tree = PrefabUtility.InstantiatePrefab(_treePrefab) as GameObject;
               tree.transform.position = position;
               tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
               tree.transform.SetParent(treesParent.transform);
           }
       }
       
       private void AddRandomRocks()
       {
           if (_rockPrefab == null)
           {
               Debug.LogError("Rock prefab is not assigned!");
               return;
           }
           
           GameObject rocksParent = GameObject.Find("Rocks");
           if (rocksParent == null)
           {
               rocksParent = new GameObject("Rocks");
           }
           
           // Add 8% of map size as rocks
           int rocksToAdd = Mathf.FloorToInt(_mapSize.x * _mapSize.y * 0.08f);
           
           for (int i = 0; i < rocksToAdd; i++)
           {
               float x = Random.Range(0, _mapSize.x * _gridSize);
               float z = Random.Range(0, _mapSize.y * _gridSize);
               Vector3 position = new Vector3(x, 0, z);
               
               GameObject rock = PrefabUtility.InstantiatePrefab(_rockPrefab) as GameObject;
               rock.transform.position = position;
               rock.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
               float scale = Random.Range(0.8f, 1.2f);
               rock.transform.localScale = new Vector3(scale, scale, scale);
               rock.transform.SetParent(rocksParent.transform);
           }
       }
   }