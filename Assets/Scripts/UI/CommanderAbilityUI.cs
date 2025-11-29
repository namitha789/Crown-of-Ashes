using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommanderAbilityUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _abilityButton;
    [SerializeField] private Image _cooldownOverlay;
    [SerializeField] private TextMeshProUGUI _cooldownText;
    [SerializeField] private TextMeshProUGUI _abilityNameText;
    [SerializeField] private TextMeshProUGUI _instructionsText;

    private AbilityTargeting _abilityTargeting;

    private void Start()
    {
        _abilityButton.onClick.AddListener(OnAbilityClicked);

        if (CommanderManager.Instance != null && CommanderManager.Instance.CurrentCommander != null)
        {
            _abilityNameText.text = CommanderManager.Instance.CurrentCommander.abilityName + " (C)";
        }

        EventManager.StartListening("AbilityReady", OnAbilityReady);
        EventManager.StartListening("AbilityUsed", OnAbilityUsed);

        // Find AbilityTargeting component
        _abilityTargeting = FindFirstObjectByType<AbilityTargeting>();

        if (_instructionsText != null)
        {
            _instructionsText.text = "";
        }
    }

    private void OnDestroy()
    {
        EventManager.StopListening("AbilityReady", OnAbilityReady);
        EventManager.StopListening("AbilityUsed", OnAbilityUsed);
    }

    private void Update()
    {
        UpdateCooldownDisplay();
    }

    private void UpdateCooldownDisplay()
    {
        if (CommanderManager.Instance == null)
            return;

        bool isReady = CommanderManager.Instance.IsAbilityReady;

        _abilityButton.interactable = isReady;
        _cooldownOverlay.fillAmount = isReady ? 0 : (1 - CommanderManager.Instance.CooldownProgress);

        if (!isReady)
        {
            float remainingTime = CommanderManager.Instance.CurrentCommander.abilityCooldown *
                (1 - CommanderManager.Instance.CooldownProgress);
            _cooldownText.text = Mathf.Ceil(remainingTime).ToString();

            if (_instructionsText != null)
            {
                _instructionsText.text = "";
            }
        }
        else
        {
            _cooldownText.text = "READY";

            if (_instructionsText != null)
            {
                _instructionsText.text = "Press C or click to use ability";
            }
        }
    }

    private void OnAbilityClicked()
    {
        // Simulate C key press
        if (CommanderManager.Instance != null && CommanderManager.Instance.IsAbilityReady)
        {
            Debug.Log("Ability button clicked! Enter targeting mode...");

            if (_instructionsText != null)
            {
                _instructionsText.text = "Click to target | Right-click to cancel";
            }

            // The AbilityTargeting component will handle the rest
            // We just need to tell it to activate
            if (_abilityTargeting != null)
            {
                // Trigger targeting mode via event or direct call
                // For now, player can press C or we can add a method to AbilityTargeting
            }
        }
        else
        {
            Debug.Log("Ability not ready!");
        }
    }

    private void OnAbilityReady(object data)
    {
        Debug.Log("Ability ready!");

        if (_instructionsText != null)
        {
            _instructionsText.text = "Press C or click to use ability";
        }
    }

    private void OnAbilityUsed(object data)
    {
        Debug.Log("Ability used!");

        if (_instructionsText != null)
        {
            _instructionsText.text = "";
        }
    }
}