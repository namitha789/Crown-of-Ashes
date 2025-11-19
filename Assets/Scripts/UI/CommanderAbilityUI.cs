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

private void Start()
{
_abilityButton.onClick.AddListener(OnAbilityClicked);

if (CommanderManager.Instance.CurrentCommander != null)
{
_abilityNameText.text = CommanderManager.Instance.CurrentCommander.abilityName + " (Q)";
}

EventManager.StartListening("AbilityReady", OnAbilityReady);
}

private void OnDestroy()
{
EventManager.StopListening("AbilityReady", OnAbilityReady);
}

private void Update()
{
UpdateCooldownDisplay();
}

private void UpdateCooldownDisplay()
{
bool isReady = CommanderManager.Instance.IsAbilityReady;

_abilityButton.interactable = isReady;
_cooldownOverlay.fillAmount = isReady ? 0 : (1 - CommanderManager.Instance.CooldownProgress);

if (!isReady)
{
float remainingTime = CommanderManager.Instance.CurrentCommander.abilityCooldown *
(1 - CommanderManager.Instance.CooldownProgress);
_cooldownText.text = Mathf.Ceil(remainingTime).ToString();
}
else
{
_cooldownText.text = "";
}
}

private void OnAbilityClicked()
{
Debug.Log("Click on map to use ability");
}

private void OnAbilityReady(object data)
{
Debug.Log("Ability ready!");
}
}
