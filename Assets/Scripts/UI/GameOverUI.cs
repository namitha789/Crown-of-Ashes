using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
[Header("References")]
[SerializeField] private GameObject _victoryPanel;
[SerializeField] private GameObject _defeatPanel;
[SerializeField] private TextMeshProUGUI _rewardsText;
[SerializeField] private Button _continueButton;

private void Start()
{
_victoryPanel.SetActive(false);
_defeatPanel.SetActive(false);

_continueButton.onClick.AddListener(OnContinueClicked);

EventManager.StartListening("GameVictory", OnGameVictory);
EventManager.StartListening("GameDefeat", OnGameDefeat);
}

private void OnDestroy()
{
EventManager.StopListening("GameVictory", OnGameVictory);
EventManager.StopListening("GameDefeat", OnGameDefeat);
}

private void OnGameVictory(object data)
{
_victoryPanel.SetActive(true);
Time.timeScale = 0f;

int shardsEarned = 100;
ProgressionManager.Instance.AddAshShards(shardsEarned);
ProgressionManager.Instance.CompleteRun(true);

_rewardsText.text = $"Ash Shards Earned: {shardsEarned}";
}

private void OnGameDefeat(object data)
{
_defeatPanel.SetActive(true);
Time.timeScale = 0f;

int shardsEarned = 25;
ProgressionManager.Instance.AddAshShards(shardsEarned);
ProgressionManager.Instance.CompleteRun(false);

_rewardsText.text = $"Ash Shards Earned: {shardsEarned}";
}

private void OnContinueClicked()
{
Time.timeScale = 1f;
SceneManager.LoadScene("MainMenu");
}
}
