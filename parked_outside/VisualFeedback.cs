 public class VisualFeedback : MonoBehaviour
   {
       [Header("Selection VFX")]
       [SerializeField] private GameObject _selectionCirclePrefab;
       [SerializeField] private float _selectionCircleOffset = 0.1f;
       
       [Header("Movement VFX")]
       [SerializeField] private GameObject _movementMarkerPrefab;
       [SerializeField] private float _movementMarkerDuration = 1.5f;
       
       [Header("Combat VFX")]
       [SerializeField] private ParticleSystem _attackVFXPrefab;
       
       [Header("Resource VFX")]
       [SerializeField] private ParticleSystem _resourceGatherVFXPrefab;
       
       private ObjectPool _movementMarkerPool;
       private ObjectPool _attackVFXPool;
       private ObjectPool _resourceGatherVFXPool;
       
       private void Awake()
       {
           // Initialize object pools
           _movementMarkerPool = new ObjectPool(_movementMarkerPrefab, 10);
           _attackVFXPool = new ObjectPool(_attackVFXPrefab.gameObject, 5);
           _resourceGatherVFXPool = new ObjectPool(_resourceGatherVFXPrefab.gameObject, 5);
       }
       
       private void Start()
       {
           // Subscribe to events
           EventManager.StartListening("UnitMoved", OnUnitMoved);
           EventManager.StartListening("UnitAttacked", OnUnitAttacked);
           EventManager.StartListening("ResourceGathered", OnResourceGathered);
       }
       
       private void OnDestroy()
       {
           EventManager.StopListening("UnitMoved", OnUnitMoved);
           EventManager.StopListening("UnitAttacked", OnUnitAttacked);
           EventManager.StopListening("ResourceGathered", OnResourceGathered);
       }
       
       private void OnUnitMoved(object data)
       {
           if (data is Vector3 position)
           {
               // Spawn movement marker at the position
               GameObject marker = _movementMarkerPool.GetObject();
               marker.transform.position = position;
               marker.SetActive(true);
               
               // Return to pool after duration
               StartCoroutine(ReturnToPoolAfterDelay(marker, _movementMarkerDuration));
           }
       }
       
       private void OnUnitAttacked(object data)
       {
           if (data is Vector3 position)
           {
               // Spawn attack VFX at the position
               GameObject attackVFX = _attackVFXPool.GetObject();
               attackVFX.transform.position = position;
               attackVFX.SetActive(true);
               
               ParticleSystem ps = attackVFX.GetComponent<ParticleSystem>();
               ps.Play();
               
               // Return to pool after particles finish
               StartCoroutine(ReturnToPoolAfterParticles(attackVFX, ps));
           }
       }
       
       private void OnResourceGathered(object data)
       {
           if (data is Vector3 position)
           {
               // Spawn resource gather VFX at the position
               GameObject resourceVFX = _resourceGatherVFXPool.GetObject();
               resourceVFX.transform.position = position;
               resourceVFX.SetActive(true);
               
               ParticleSystem ps = resourceVFX.GetComponent<ParticleSystem>();
               ps.Play();
               
               // Return to pool after particles finish
               StartCoroutine(ReturnToPoolAfterParticles(resourceVFX, ps));
           }
       }
       
       private System.Collections.IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay)
       {
           yield return new WaitForSeconds(delay);
           obj.SetActive(false);
       }
       
       private System.Collections.IEnumerator ReturnToPoolAfterParticles(GameObject obj, ParticleSystem ps)
       {
           yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetimeMultiplier);
           obj.SetActive(false);
       }
   }