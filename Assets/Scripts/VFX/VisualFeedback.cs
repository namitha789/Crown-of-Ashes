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

    // *** NEW: Add this header and field ***
    [Header("Commander Ability VFX")]
    [SerializeField] private GameObject _abilityEffectPrefab;

    // Object pools
    private ObjectPool _movementMarkerPool;
    private ObjectPool _attackVFXPool;
    private ObjectPool _resourceGatherVFXPool;
    // *** NEW: Add this pool ***
    private ObjectPool _abilityVFXPool;

    private void Awake()
    {
        // Initialize pools only if prefabs are assigned
        if (_movementMarkerPrefab != null)
            _movementMarkerPool = new ObjectPool(_movementMarkerPrefab, 10);
        if (_attackVFXPrefab != null)
            _attackVFXPool = new ObjectPool(_attackVFXPrefab.gameObject, 5);
        if (_resourceGatherVFXPrefab != null)
            _resourceGatherVFXPool = new ObjectPool(_resourceGatherVFXPrefab.gameObject, 5);
        
        // *** NEW: Initialize ability VFX pool ***
        if (_abilityEffectPrefab != null)
            _abilityVFXPool = new ObjectPool(_abilityEffectPrefab, 3);
    }

    private void Start()
    {
        EventManager.StartListening("UnitMoved", OnUnitMoved);
        EventManager.StartListening("UnitAttacked", OnUnitAttacked);
        EventManager.StartListening("ResourceGathered", OnResourceGathered);
        
        // *** NEW: Listen for ability events ***
        EventManager.StartListening("AbilityUsed", OnAbilityUsed);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("UnitMoved", OnUnitMoved);
        EventManager.StopListening("UnitAttacked", OnUnitAttacked);
        EventManager.StopListening("ResourceGathered", OnResourceGathered);
        
        // *** NEW: Stop listening ***
        EventManager.StopListening("AbilityUsed", OnAbilityUsed);
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

    // *** NEW: Add this method ***
    private void OnAbilityUsed(object data)
    {
        if (data is Vector3 position && _abilityEffectPrefab != null)
        {
            GameObject effect = _abilityVFXPool != null
                ? _abilityVFXPool.GetObject()
                : Instantiate(_abilityEffectPrefab, position, Quaternion.identity);

            effect.transform.position = position;
            effect.SetActive(true);

            // If it has a particle system, use it
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                StartCoroutine(ReturnToPoolAfterParticles(effect, ps));
            }
            else
            {
                // Otherwise, just return to pool after 2 seconds
                StartCoroutine(ReturnToPoolAfterDelay(effect, 2f));
            }
        }
    }

    // ----------------------------------------------------
    // Helper Methods (keep existing)
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
