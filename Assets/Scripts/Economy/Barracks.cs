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
    private float _currentProductionTime = 0f;
    private Camera _mainCamera;

    public int QueueCount => _productionQueue.Count;
    public bool IsProducing => _isProducing;
    public float ProductionProgress => _trainingTime <= 0 ? 1f : _currentProductionTime / _trainingTime;

    protected override void Start()
    {
        base.Start();

        _mainCamera = Camera.main;
        
        if (_mainCamera == null)
        {
            Debug.LogError("[Barracks] Main Camera not found!");
        }

        if (_spawnPoint == null)
        {
            var spawnObj = new GameObject("SpawnPoint");
            spawnObj.transform.SetParent(transform);
            spawnObj.transform.localPosition = new Vector3(3f, 0f, 0f);
            _spawnPoint = spawnObj.transform;
        }
        
        // Validate collider for clicks
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("[Barracks] No Collider found! Clicks won't work.");
        }
        else if (col.isTrigger)
        {
            Debug.LogWarning("[Barracks] Collider is a trigger. Uncheck 'Is Trigger' for clicks to work.");
        }
        
        Debug.Log($"[Barracks] Initialized on {gameObject.name}");
    }

    // Handle clicking on barracks
    private void Update()
    {
        // CRITICAL: Call base.Update() so construction progresses!
        base.Update();
        
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            // Skip if over UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (_mainCamera == null) return;

            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.cyan, 1f);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log($"[Barracks] {gameObject.name} clicked!");
                    OpenUI();
                }
            }
        }
    }

    private void OpenUI()
    {
        Debug.Log("[Barracks] OpenUI called");
        
        // Find the UI panel
        BarracksUI panel = FindObjectOfType<BarracksUI>(true);
        
        if (panel != null)
        {
            Debug.Log($"[Barracks] Found BarracksUI: {panel.gameObject.name}");
            panel.ShowFor(this);
        }
        else
        {
            Debug.LogError("[Barracks] BarracksUI not found in scene!");
        }
    }

    // Called by UI button
    public void TrainWarrior()
    {
        Debug.Log("[Barracks] TrainWarrior called");
        Debug.Log($"[Barracks] IsConstructed: {IsConstructed}");
        Debug.Log($"[Barracks] IsConstructing: {IsConstructing}");
        Debug.Log($"[Barracks] _isProducing: {_isProducing}");
        
        if (_warriorPrefab == null)
        {
            Debug.LogError("[Barracks] _warriorPrefab not assigned.");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[Barracks] ResourceManager.Instance is null.");
            return;
        }

        if (_productionQueue.Count >= _maxQueueSize)
        {
            Debug.LogWarning("[Barracks] Production queue is full!");
            return;
        }

        if (!ResourceManager.Instance.SpendResources(_trainingCost, 0, 0))
        {
            Debug.LogWarning("[Barracks] Not enough gold to train warrior!");
            return;
        }

        _productionQueue.Enqueue(new UnitOrder
        {
            UnitPrefab = _warriorPrefab,
            ProductionTime = _trainingTime
        });

        Debug.Log($"[Barracks] Warrior added to queue. Queue size: {_productionQueue.Count}");

        if (IsConstructed && !_isProducing)
        {
            Debug.Log("[Barracks] Starting production NOW!");
            StartCoroutine(ProcessQueue());
        }
        else if (!IsConstructed)
        {
            Debug.Log("[Barracks] Still constructing. Training will start when complete.");
        }
        else if (_isProducing)
        {
            Debug.Log("[Barracks] Already producing. Unit added to queue.");
        }
    }

    private void OnConstructionComplete()
    {
        Debug.Log("[Barracks] OnConstructionComplete called!");
        Debug.Log($"[Barracks] IsConstructed: {IsConstructed}");
        Debug.Log($"[Barracks] Queue count: {_productionQueue.Count}");
        Debug.Log($"[Barracks] _isProducing: {_isProducing}");
        
        if (_productionQueue.Count > 0 && !_isProducing)
        {
            Debug.Log("[Barracks] Starting queued production!");
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        _isProducing = true;
        Debug.Log($"[Barracks] Started processing queue. Items: {_productionQueue.Count}");

        while (_productionQueue.Count > 0)
        {
            var order = _productionQueue.Dequeue();
            _currentProductionTime = 0f;

            Debug.Log($"[Barracks] Training unit... {order.ProductionTime}s total time");

            while (_currentProductionTime < order.ProductionTime)
            {
                _currentProductionTime += Time.deltaTime;
                
                // Log every second
                if (Mathf.FloorToInt(_currentProductionTime) != Mathf.FloorToInt(_currentProductionTime - Time.deltaTime))
                {
                    Debug.Log($"[Barracks] Training progress: {Mathf.FloorToInt(_currentProductionTime)}/{order.ProductionTime}s");
                }
                
                yield return null;
            }

            Debug.Log("[Barracks] Training complete! Spawning now...");
            SpawnUnit(order.UnitPrefab);
        }

        _isProducing = false;
        _currentProductionTime = 0f;
        Debug.Log("[Barracks] Queue complete");
    }

    private void SpawnUnit(GameObject unitPrefab)
    {
        Debug.Log("[Barracks] SpawnUnit called");
        
        if (unitPrefab == null)
        {
            Debug.LogError("[Barracks] Cannot spawn - unitPrefab is NULL!");
            return;
        }
        
        if (_spawnPoint == null)
        {
            Debug.LogError("[Barracks] Cannot spawn - _spawnPoint is NULL!");
            return;
        }

        Debug.Log($"[Barracks] Attempting to spawn {unitPrefab.name} at {_spawnPoint.position}");

        Vector3 spawnPos = _spawnPoint.position;
        // Increased search radius from 1.5f to 5f for better NavMesh detection
        bool onNavMesh = NavMesh.SamplePosition(_spawnPoint.position, out NavMeshHit hit, 5f, NavMesh.AllAreas);
        
        if (onNavMesh)
        {
            spawnPos = hit.position;
            Debug.Log($"[Barracks] Found NavMesh position: {spawnPos}");
        }
        else
        {
            Debug.LogWarning($"[Barracks] No NavMesh found near spawn point! Check your NavMesh baking. Using raw position: {spawnPos}");
        }

        GameObject unit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);
        
        if (unit == null)
        {
            Debug.LogError("[Barracks] Instantiate returned NULL!");
            return;
        }

        // Set to PlayerUnits layer (matching your prefab setup)
        int playerLayer = LayerMask.NameToLayer("PlayerUnits");
        if (playerLayer >= 0)
        {
            SetLayerRecursively(unit, playerLayer);
            Debug.Log($"[Barracks] Set layer to PlayerUnits for {unit.name}");
        }
        else
        {
            // Fallback to Selectable if PlayerUnits doesn't exist
            int selectableLayer = LayerMask.NameToLayer("Selectable");
            if (selectableLayer >= 0)
            {
                SetLayerRecursively(unit, selectableLayer);
                Debug.Log($"[Barracks] Set layer to Selectable for {unit.name}");
            }
        }

        Debug.Log($"[Barracks] ✓ Successfully spawned {unit.name} at {spawnPos}");
        Debug.Log($"[Barracks] Unit layer: {LayerMask.LayerToName(unit.layer)}");

        var unitComp = unit.GetComponent<Unit>();
        if (unitComp != null)
        {
            Debug.Log($"[Barracks] Unit component found on {unit.name}");
            Debug.Log($"[Barracks] Is Player Unit: {unitComp.gameObject.GetComponent<Unit>()}");
            
            var agent = unit.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                Debug.Log($"[Barracks] NavMeshAgent found. IsOnNavMesh: {agent.isOnNavMesh}");
                
                // Give the agent a frame to settle on the NavMesh
                if (!agent.isOnNavMesh)
                {
                    // Try to warp to nearest NavMesh position
                    if (onNavMesh)
                    {
                        agent.Warp(spawnPos);
                        Debug.Log($"[Barracks] Warped agent to NavMesh position: {spawnPos}");
                    }
                }
                
                // Move to rally point after agent is on NavMesh
                Vector3 rallyPoint = _spawnPoint.position + _spawnPoint.forward * 5f;
                
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(rallyPoint);
                    Debug.Log($"[Barracks] Moving unit to rally point: {rallyPoint}");
                }
                else
                {
                    Debug.LogWarning("[Barracks] Agent still not on NavMesh after warp attempt");
                }
            }
            else
            {
                Debug.LogWarning($"[Barracks] No NavMeshAgent on {unit.name}");
            }
        }
        else
        {
            Debug.LogWarning($"[Barracks] No Unit component on {unit.name}");
        }
    }

    [System.Serializable]
    public class UnitOrder
    {
        public GameObject UnitPrefab;
        public float ProductionTime;
    }

    // Helper method to set layer recursively
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}