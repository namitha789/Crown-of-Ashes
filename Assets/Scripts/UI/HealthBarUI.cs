using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
[Header("References")]
[SerializeField] private Unit _unit;
[SerializeField] private Image _fillImage;
[SerializeField] private Canvas _canvas;

[Header("Settings")]
[SerializeField] private Vector3 _offset = new Vector3(0, 2, 0);
[SerializeField] private bool _hideWhenFull = true;

private Camera _mainCamera;

private void Start()
{
_mainCamera = Camera.main;

if (_canvas != null)
{
_canvas.worldCamera = _mainCamera;
}

EventManager.StartListening("UnitHealthChanged", OnUnitHealthChanged);

UpdateHealthBar();
}

private void OnDestroy()
{
EventManager.StopListening("UnitHealthChanged", OnUnitHealthChanged);
}

private void LateUpdate()
{
if (_mainCamera != null)
{
transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
_mainCamera.transform.rotation * Vector3.up);
}
}

private void OnUnitHealthChanged(object data)
{
if (data is Unit unit && unit == _unit)
{
UpdateHealthBar();
}
}

private void UpdateHealthBar()
{
if (_unit == null || _fillImage == null) return;

float healthPercent = (float)_unit.CurrentHealth / _unit.MaxHealth;
_fillImage.fillAmount = healthPercent;

// Color based on health
if (healthPercent > 0.6f)
_fillImage.color = Color.green;
else if (healthPercent > 0.3f)
_fillImage.color = Color.yellow;
else
_fillImage.color = Color.red;

// Hide when full
if (_hideWhenFull && _canvas != null)
{
_canvas.enabled = healthPercent < 1f;
}
}
}
