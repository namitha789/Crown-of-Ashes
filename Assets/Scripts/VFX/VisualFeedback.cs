using UnityEngine;
using System.Collections;

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

    // Object pools
    private ObjectPool _movementMarkerPool;
    private ObjectPool _attackVFXPool;
    private ObjectPool _resourceGatherVFXPool;

    private void Awake()
    {
        // Initialize pools only if prefabs are assigned
        if (_movementMarkerPrefab != null)
            _movementMarkerPool = new ObjectPool(_movementMarkerPrefab, 10);
        if (_attackVFXPrefab != null)
            _attackVFXPool = new ObjectPool(_attackVFXPrefab.gameObject, 5);
        if (_resourceGatherVFXPrefab != null)
            _resourceGatherVFXPool = new ObjectPool(_resourceGatherVFXPrefab.gameObject, 5);
    }

    private void Start()
    {
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

    // ----------------------------------------------------
    // Event Handlers
    // ----------------------------------------------------
    private void OnUnitMoved(object data)
    {
        if (data is Vector3 position)
        {
            ShowMovementMarker(position);
        }
    }

    private void OnUnitAttacked(object data)
    {
        if (data is Vector3 position && _attackVFXPrefab != null)
        {
            GameObject attackVFX = _attackVFXPool != null
                ? _attackVFXPool.GetObject()
                : Instantiate(_attackVFXPrefab.gameObject, position, Quaternion.identity);

            attackVFX.transform.position = position;
            attackVFX.SetActive(true);

            ParticleSystem ps = attackVFX.GetComponent<ParticleSystem>();
            ps.Play();

            StartCoroutine(ReturnToPoolAfterParticles(attackVFX, ps));
        }
    }

    private void OnResourceGathered(object data)
    {
        if (data is Vector3 position && _resourceGatherVFXPrefab != null)
        {
            GameObject resourceVFX = _resourceGatherVFXPool != null
                ? _resourceGatherVFXPool.GetObject()
                : Instantiate(_resourceGatherVFXPrefab.gameObject, position, Quaternion.identity);

            resourceVFX.transform.position = position;
            resourceVFX.SetActive(true);

            ParticleSystem ps = resourceVFX.GetComponent<ParticleSystem>();
            ps.Play();

            StartCoroutine(ReturnToPoolAfterParticles(resourceVFX, ps));
        }
    }

    // ----------------------------------------------------
    // Helper Methods
    // ----------------------------------------------------
    private void ShowMovementMarker(Vector3 position)
    {
        if (_movementMarkerPool != null)
        {
            GameObject marker = _movementMarkerPool.GetObject();
            marker.transform.position = position;
            marker.SetActive(true);
            StartCoroutine(ReturnToPoolAfterDelay(marker, _movementMarkerDuration));
        }
        else if (_movementMarkerPrefab != null)
        {
            GameObject marker = Instantiate(_movementMarkerPrefab, position, Quaternion.identity);
            Destroy(marker, _movementMarkerDuration);
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private IEnumerator ReturnToPoolAfterParticles(GameObject obj, ParticleSystem ps)
    {
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetimeMultiplier);
        obj.SetActive(false);
    }
}
