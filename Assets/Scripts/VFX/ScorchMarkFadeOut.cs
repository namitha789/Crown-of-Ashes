using UnityEngine;

public class ScorchMarkFadeOut : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float _fadeDuration = 10f;
    [SerializeField] private float _fadeDelay = 2f; // Wait before starting fade
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);
    
    private Material _material;
    private float _elapsedTime = 0f;
    private Color _initialColor;
    private bool _startedFading = false;
    
    private void Start()
    {
        // Get the material from the MeshRenderer
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        
        if (renderer != null)
        {
            // Create an instance of the material to avoid affecting the original
            _material = renderer.material;
            _initialColor = _material.color;
        }
        else
        {
            Debug.LogWarning("ScorchMarkFadeOut: No MeshRenderer found on " + gameObject.name);
            enabled = false;
        }
    }
    
    private void Update()
    {
        if (_material == null) return;
        
        _elapsedTime += Time.deltaTime;
        
        // Wait for delay before starting fade
        if (_elapsedTime < _fadeDelay)
        {
            return;
        }
        
        if (!_startedFading)
        {
            _startedFading = true;
            _elapsedTime = 0f; // Reset timer for fade duration
        }
        
        // Calculate fade progress (0 to 1)
        float progress = Mathf.Clamp01(_elapsedTime / _fadeDuration);
        
        // Apply curve to the progress
        float curvedProgress = _fadeCurve.Evaluate(progress);
        
        // Fade out the alpha
        Color newColor = _initialColor;
        newColor.a = Mathf.Lerp(_initialColor.a, 0f, curvedProgress);
        _material.color = newColor;
        
        // Optional: Shrink slightly as it fades
        float scale = Mathf.Lerp(1f, 0.8f, curvedProgress);
        transform.localScale = Vector3.one * 0.5f * scale;
    }
    
    private void OnDestroy()
    {
        // Clean up the material instance
        if (_material != null)
        {
            Destroy(_material);
        }
    }
}