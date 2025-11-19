using UnityEngine;

public class SlashFadeOut : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    private Material _material;
    private float _elapsedTime = 0f;
    private Color _initialColor;
    
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
            Debug.LogWarning("SlashFadeOut: No MeshRenderer found on " + gameObject.name);
            enabled = false;
        }
    }
    
    private void Update()
    {
        if (_material == null) return;
        
        _elapsedTime += Time.deltaTime;
        
        // Calculate fade progress (0 to 1)
        float progress = Mathf.Clamp01(_elapsedTime / _fadeDuration);
        
        // Apply curve to the progress for smoother fade
        float curvedProgress = _fadeCurve.Evaluate(progress);
        
        // Fade out the alpha
        Color newColor = _initialColor;
        newColor.a = Mathf.Lerp(_initialColor.a, 0f, curvedProgress);
        _material.color = newColor;
        
        // Optional: Scale up slightly as it fades
        transform.localScale = Vector3.Lerp(
            transform.localScale, 
            transform.localScale * 1.2f, 
            curvedProgress
        );
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