using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float pulseSpeed = 2f;
    
    [Header("Fade Settings")]
    public float minAlpha = 0.3f;
    public float maxAlpha = 0.8f;
    
    private Material material;
    private Vector3 originalScale;
    private Color originalColor;
    
    void Start()
    {
        originalScale = transform.localScale;
        
        // Get the material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
            originalColor = material.color;
        }
    }
    
    void Update()
    {
        // Pulse scale
        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        float scale = Mathf.Lerp(minScale, maxScale, pulse);
        transform.localScale = originalScale * scale;
        
        // Pulse alpha
        if (material != null)
        {
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(minAlpha, maxAlpha, pulse);
            material.color = newColor;
        }
    }
}
