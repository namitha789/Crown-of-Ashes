using UnityEngine;
using System.Collections;

public class UnitFlashEffect : MonoBehaviour
{
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private float _flashDuration = 0.1f;
    
    private Color[] _originalColors;
    private Material[] _materials;
    
    private void Start()
    {
        // Auto-find all renderers if not assigned
        if (_renderers == null || _renderers.Length == 0)
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }
        
        // Store original colors
        _materials = new Material[_renderers.Length];
        _originalColors = new Color[_renderers.Length];
        
        for (int i = 0; i < _renderers.Length; i++)
        {
            _materials[i] = _renderers[i].material;
            _originalColors[i] = _materials[i].color;
        }
        
        EventManager.StartListening("UnitDamaged", OnUnitDamaged);
    }
    
    private void OnDestroy()
    {
        EventManager.StopListening("UnitDamaged", OnUnitDamaged);
    }
    
    private void OnUnitDamaged(object data)
    {
        if (data is CombatData combatData && combatData.Target == GetComponent<Unit>())
        {
            StartCoroutine(FlashRoutine());
        }
    }
    
    private IEnumerator FlashRoutine()
    {
        // Flash all materials to red
        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i] != null)
                _materials[i].color = _flashColor;
        }
        
        yield return new WaitForSeconds(_flashDuration);
        
        // Restore original colors
        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i] != null)
                _materials[i].color = _originalColors[i];
        }
    }
}