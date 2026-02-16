using UnityEngine;
using System.Collections;

public class UnitDeathEffect : MonoBehaviour
{
    [SerializeField] private GameObject _deathVFXPrefab;
    [SerializeField] private GameObject _scorchMarkPrefab;
    [SerializeField] private float _fadeOutDuration = 0.5f;
    
    private Renderer[] _renderers;
    
    private void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }
    
    public void TriggerDeath()
    {
        StartCoroutine(DeathSequence());
    }
    
    private IEnumerator DeathSequence()
    {
        // Spawn death VFX
        if (_deathVFXPrefab != null)
        {
            Instantiate(_deathVFXPrefab, transform.position, Quaternion.identity);
        }
        
        // Leave scorch mark on ground
        if (_scorchMarkPrefab != null)
        {
            Vector3 groundPos = transform.position;
            groundPos.y = 0.01f; // Slightly above ground to prevent z-fighting
            Instantiate(_scorchMarkPrefab, groundPos, Quaternion.Euler(90, 0, 0));
        }
        
        // Fade out unit model
        float elapsed = 0f;
        
        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / _fadeOutDuration);
            
            foreach (Renderer renderer in _renderers)
            {
                if (renderer != null)
                {
                    Color color = renderer.material.color;
                    color.a = alpha;
                    renderer.material.color = color;
                }
            }
            
            yield return null;
        }
        
        // Destroy unit
        Destroy(gameObject);
    }
}
