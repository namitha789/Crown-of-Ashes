using UnityEngine;
public class AbilityTargeting : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private GameObject _targetingIndicatorPrefab;
    [SerializeField] private float _maxTargetDistance = 50f;

    private GameObject _targetingIndicator;
    private bool _isTargeting = false;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("AbilityTargeting: No main camera found!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        // Listen for Q key
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryActivateAbility();
        }

        // If targeting mode active, show indicator and wait for click
        if (_isTargeting)
        {
            UpdateTargetingIndicator();

            // Left click to confirm
            if (Input.GetMouseButtonDown(0))
            {
                ConfirmAbilityTarget();
            }

            // Right click to cancel
            if (Input.GetMouseButtonDown(1))
            {
                CancelAbilityTargeting();
            }
        }
    }

    private void TryActivateAbility()
    {
        if (CommanderManager.Instance == null || !CommanderManager.Instance.IsAbilityReady)
        {
            Debug.Log("Ability not ready or no CommanderManager!");
            return;
        }

        // Enter targeting mode
        _isTargeting = true;

        // Show targeting indicator
        if (_targetingIndicatorPrefab != null && _targetingIndicator == null)
        {
            _targetingIndicator = Instantiate(_targetingIndicatorPrefab);
        }

        if (_targetingIndicator != null)
        {
            _targetingIndicator.SetActive(true);
        }

        Debug.Log("Ability targeting mode activated! Click to target, right-click to cancel");
    }

    private void UpdateTargetingIndicator()
    {
        if (_targetingIndicator == null)
            return;

        // Raycast from mouse to ground
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _maxTargetDistance, _groundLayer))
        {
            // Position indicator at hit point
            _targetingIndicator.transform.position = hit.point;
        }
    }

    private void ConfirmAbilityTarget()
    {
        if (_targetingIndicator == null)
            return;

        Vector3 targetPosition = _targetingIndicator.transform.position;

        // Use ability at target position
        if (CommanderManager.Instance != null)
        {
            CommanderManager.Instance.UseAbility(targetPosition);
        }

        // Exit targeting mode
        CancelAbilityTargeting();
    }

    private void CancelAbilityTargeting()
    {
        _isTargeting = false;

        if (_targetingIndicator != null)
        {
            _targetingIndicator.SetActive(false);
        }

        Debug.Log("Ability targeting cancelled");
    }
}