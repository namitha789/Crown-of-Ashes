using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[AddComponentMenu("RTS/Buildings/Barracks")]
[RequireComponent(typeof(Collider))]
public class Barracks : Building
{
    [Header("Unit Production")]
    [SerializeField] private GameObject _warriorPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _trainingCost = 50;
    [SerializeField] private float _trainingTime = 3f;
    [SerializeField] private int _maxQueueSize = 5;

    private readonly Queue<UnitOrder> _productionQueue = new Queue<UnitOrder>();
    private bool _isProducing = false;
    private bool _isProducingUnit = false;
    private float _currentProductionTime = 0f;
    private Camera _mainCamera;
    private int _spawnedUnitCount = 0; // Track number of spawned units for positioning

    public int QueueCount => _productionQueue.Count + (_isProducingUnit ? 1 : 0);
    public bool IsProducing => _isProducing;
    public float ProductionProgress => _trainingTime <= 0 ? 1f : _currentProductionTime / _trainingTime;

    protected override void Start()
    {
        base.Start();

        _mainCamera = Camera.main;

        if (_spawnPoint == null)
        {
            var spawnObj = new GameObject("SpawnPoint");
            spawnObj.transform.SetParent(transform);
            spawnObj.transform.localPosition = new Vector3(3f, 0f, 0f);
            _spawnPoint = spawnObj.transform;
        }
    }

    // REMOVED: No longer override Update() since Building doesn't have it anymore

    private void OpenUI()
    {
        BuildingInfoUI panel = FindAnyObjectByType<BuildingInfoUI>(FindObjectsInactive.Include);
        
        if (panel != null)
        {
            panel.ShowBuilding(this);
        }
    }

   public void TrainWarrior()
{
    Debug.Log("TrainWarrior() called!");
    
    if (_warriorPrefab == null)
    {
        Debug.LogError("BARRACKS: Warrior prefab is NOT assigned!");
        return;
    }
    
    Debug.Log("✓ Warrior prefab assigned");

    if (ResourceManager.Instance == null)
    {
        Debug.LogError("BARRACKS: ResourceManager not found!");
        return;
    }
    
    Debug.Log("✓ ResourceManager found");

    if (_productionQueue.Count >= _maxQueueSize)
    {
        Debug.LogWarning($"BARRACKS: Queue full! ({_productionQueue.Count}/{_maxQueueSize})");
        return;
    }
    
    Debug.Log($"✓ Queue has space: {_productionQueue.Count}/{_maxQueueSize}");

    if (!ResourceManager.Instance.SpendResources(_trainingCost, 0, 0))
    {
        Debug.LogWarning($"BARRACKS: Not enough resources! Need {_trainingCost} gold");
        return;
    }
    
    Debug.Log($"✓ Spent {_trainingCost} gold");

    _productionQueue.Enqueue(new UnitOrder
    {
        UnitPrefab = _warriorPrefab,
        ProductionTime = _trainingTime
    });

    Debug.Log($"✓ Added to queue. Queue size: {_productionQueue.Count}");

    BuildingInfoUI buildingUI = FindAnyObjectByType<BuildingInfoUI>();
    if (buildingUI != null)
    {
        buildingUI.ShowBuilding(this);
    }

    Debug.Log($"IsConstructed: {IsConstructed}, IsProducing: {_isProducing}");

    if (IsConstructed && !_isProducing)
    {
        Debug.Log("🚀 Starting production!");
        StartCoroutine(ProcessQueue());
    }
    else
    {
        Debug.LogWarning($"NOT starting production. IsConstructed: {IsConstructed}, IsProducing: {_isProducing}");
    }
}

    // This gets called by Building.cs via SendMessage
    private void OnConstructionComplete()
    {
        if (_productionQueue.Count > 0 && !_isProducing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        _isProducing = true;

        while (_productionQueue.Count > 0)
        {
            var order = _productionQueue.Dequeue();
            _isProducingUnit = true;
            _currentProductionTime = 0f;

            while (_currentProductionTime < order.ProductionTime)
            {
                _currentProductionTime += Time.deltaTime;
                yield return null;
            }

            SpawnUnit(order.UnitPrefab);
            _isProducingUnit = false;
        }

        _isProducing = false;
        _currentProductionTime = 0f;
    }

    private void SpawnUnit(GameObject unitPrefab)
    {
        if (unitPrefab == null)
        {
            return;
        }
        
        if (_spawnPoint == null)
        {
            return;
        }

        // Add random offset to spawn position to avoid exact overlap
        Vector3 randomOffset = new Vector3(
            Random.Range(-1.5f, 1.5f),
            0f,
            Random.Range(-1.5f, 1.5f)
        );

        Vector3 spawnPos = _spawnPoint.position + randomOffset;
        bool onNavMesh = NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 5f, NavMesh.AllAreas);

        if (onNavMesh)
        {
            spawnPos = hit.position;
        }

        // Face spawn point's forward direction for consistent orientation
        Quaternion spawnRotation = _spawnPoint.rotation;
        GameObject unit = Instantiate(unitPrefab, spawnPos, spawnRotation);
        
        if (unit == null)
        {
            return;
        }

        int playerLayer = LayerMask.NameToLayer("PlayerUnits");
        if (playerLayer >= 0)
        {
            SetLayerRecursively(unit, playerLayer);
        }
        else
        {
            int selectableLayer = LayerMask.NameToLayer("Selectable");
            if (selectableLayer >= 0)
            {
                SetLayerRecursively(unit, selectableLayer);
            }
        }

        var unitComp = unit.GetComponent<Unit>();
        if (unitComp != null)
        {
            var agent = unit.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                if (!agent.isOnNavMesh)
                {
                    if (onNavMesh)
                    {
                        agent.Warp(spawnPos);
                    }
                }
                
                // Calculate rally point with formation offset
                Vector3 baseRallyPoint = _spawnPoint.position + _spawnPoint.forward * 8f;

                // Apply formation offset so units don't stack
                int unitsPerRow = 5; // 5 units per row
                int row = _spawnedUnitCount / unitsPerRow;
                int col = _spawnedUnitCount % unitsPerRow;

                // Increased spacing from 2f to 4f for more personal space
                // Center the formation: (col - 2) centers around middle position
                Vector3 offset = _spawnPoint.right * (col - 2) * 4f + _spawnPoint.forward * row * 4f;
                Vector3 rallyPoint = baseRallyPoint + offset;

                _spawnedUnitCount++; // Increment for next unit

                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(rallyPoint);
                }
            }
        }
    }

    [System.Serializable]
    public class UnitOrder
    {
        public GameObject UnitPrefab;
        public float ProductionTime;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}