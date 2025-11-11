using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

[AddComponentMenu("RTS/Buildings/Barracks")]
public class Barracks : Building
{
    [Header("Unit Production")]
    [SerializeField] private GameObject _warriorPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private int _trainingCost = 50;
    [SerializeField] private float _trainingTime = 3f;
    [SerializeField] private int _maxQueueSize = 5;

    private readonly Queue<UnitOrder> _productionQueue = new Queue<UnitOrder>();
    private Coroutine _queueRoutine;
    private bool _isProducing = false;
    private float _currentProductionTime = 0f;

    public int QueueCount => _productionQueue.Count;
    public bool IsProducing => _isProducing;
    public float ProductionProgress => Mathf.Clamp01(_trainingTime <= 0f ? 0f : _currentProductionTime / _trainingTime);

    protected override void Start()
    {
        base.Start();
        DebugCheckpoint.Hit("Barracks.Start");
        EnsureSpawnPoint();
    }

    private void EnsureSpawnPoint()
    {
        if (_spawnPoint == null)
        {
            GameObject spawnObj = new GameObject("SpawnPoint");
            spawnObj.transform.SetParent(transform, false);
            spawnObj.transform.localPosition = new Vector3(3f, 0f, 0f);
            _spawnPoint = spawnObj.transform;
        }
    }

    public void TrainWarrior()
    {
        DebugCheckpoint.Hit("Barracks.TrainWarrior");

        if (_warriorPrefab == null)
        {
            Debug.LogError("[Barracks] Warrior prefab not assigned.");
            return;
        }

        if (_productionQueue.Count >= _maxQueueSize)
        {
            Debug.LogWarning("[Barracks] Production queue is full!");
            return;
        }

        if (ResourceManager.Instance != null)
        {
            if (!ResourceManager.Instance.SpendGold(_trainingCost))
            {
                Debug.LogWarning("[Barracks] Not enough gold to train warrior!");
                return;
            }
        }
        else
        {
            Debug.LogWarning("[Barracks] ResourceManager.Instance is null - skipping cost in this context (tests?).");
        }

        _productionQueue.Enqueue(new UnitOrder
        {
            UnitPrefab = _warriorPrefab,
            ProductionTime = Mathf.Max(0f, _trainingTime)
        });

        if (_queueRoutine == null)
        {
            _queueRoutine = StartCoroutine(ProcessQueue());
        }

        Debug.Log($"[Barracks] Warrior queued. Queue size: {_productionQueue.Count}");
    }

    private IEnumerator ProcessQueue()
    {
        DebugCheckpoint.Hit("Barracks.ProcessQueue.Start");

        _isProducing = true;

        while (_productionQueue.Count > 0)
        {
            UnitOrder order = _productionQueue.Dequeue();
            _currentProductionTime = 0f;

            while (_currentProductionTime < order.ProductionTime)
            {
                _currentProductionTime += Time.deltaTime;
                yield return null;
            }

            SpawnUnit(order.UnitPrefab);
        }

        _isProducing = false;
        _currentProductionTime = 0f;
        _queueRoutine = null;

        DebugCheckpoint.Hit("Barracks.ProcessQueue.End");
    }

    private void SpawnUnit(GameObject unitPrefab)
    {
        DebugCheckpoint.Hit("Barracks.SpawnUnit");

        if (unitPrefab == null || _spawnPoint == null)
        {
            Debug.LogError("[Barracks] Cannot spawn unit - missing prefab or spawn point!");
            return;
        }

        GameObject unit = Instantiate(unitPrefab, _spawnPoint.position, Quaternion.identity);

        int selectableLayer = LayerMask.NameToLayer("Selectable");
        if (selectableLayer >= 0)
        {
            unit.layer = selectableLayer;
        }

        Debug.Log($"[Barracks] Spawned {unit.name} at {_spawnPoint.position}");

        // Optional move-away if unit has NavMeshAgent and is on a NavMesh
        Unit unitComponent = unit.GetComponent<Unit>();
        if (unitComponent != null)
        {
            var agent = unit.GetComponent<NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                Vector3 rallyPoint = _spawnPoint.position + _spawnPoint.forward * 5f;
                agent.SetDestination(rallyPoint);
            }
        }
    }

    [System.Serializable]
    public class UnitOrder
    {
        public GameObject UnitPrefab;
        public float ProductionTime;
    }
}
