using UnityEngine;

public class AbilityVFXController : MonoBehaviour
{
    [Header("VFX Prefabs")]
    [SerializeField] private GameObject _warCryVFXPrefab;
    [SerializeField] private GameObject _explosionVFXPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip _abilitySound;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Listen for ability used events
        EventManager.StartListening("AbilityUsed", OnAbilityUsed);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("AbilityUsed", OnAbilityUsed);
    }

    private void OnAbilityUsed(object data)
    {
        if (data is Vector3 position)
        {
            SpawnWarCryVFX(position);
            PlayAbilitySound();
        }
    }

    private void SpawnWarCryVFX(Vector3 position)
    {
        GameObject vfxPrefab = _warCryVFXPrefab != null ? _warCryVFXPrefab : _explosionVFXPrefab;

        if (vfxPrefab != null)
        {
            GameObject vfx = Instantiate(vfxPrefab, position, Quaternion.identity);

            // Auto-destroy after particle lifetime
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(vfx, 3f); // Default 3 seconds
            }
        }
        else
        {
            Debug.LogWarning("No VFX prefab assigned for War Cry!");
        }
    }

    private void PlayAbilitySound()
    {
        if (_abilitySound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_abilitySound);
        }
    }
}