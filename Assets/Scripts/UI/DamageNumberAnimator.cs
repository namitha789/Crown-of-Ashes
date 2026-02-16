using UnityEngine;
using TMPro;
using System.Collections;

public class DamageNumberAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _riseSpeed = 2f;
    [SerializeField] private float _fadeSpeed = 1f;
    [SerializeField] private float _lifetime = 1f;
    
    private Camera _mainCamera;
    private Color _startColor;
    private float _elapsed = 0f;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        _startColor = _text.color;
        StartCoroutine(AnimateAndDestroy());
    }
    
    private void Update()
    {
        // Rise upward
        transform.position += Vector3.up * _riseSpeed * Time.deltaTime;
        
        // Face camera
        if (_mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.transform.position);
        }
        
        // Fade out
        _elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, _elapsed * _fadeSpeed/ _lifetime);
        _text.color = new Color(_startColor.r, _startColor.g, _startColor.b, alpha);
    }
    
    private IEnumerator AnimateAndDestroy()
    {
        yield return new WaitForSeconds(_lifetime);
        Destroy(gameObject);
    }
    
    public void SetDamage(int damage)
    {
        _text.text = $"-{damage}";
    }
}
