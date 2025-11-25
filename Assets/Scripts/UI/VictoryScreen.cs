using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private TextMeshProUGUI _rewardsText;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("Settings")]
    [SerializeField] private int _baseAshShardReward = 100;
    [SerializeField] private int _goldReward = 200;

    private void Start()
    {
        if (_victoryPanel != null)
        {
            _victoryPanel.SetActive(false);
        }

        if (_continueButton != null)
        {
            _continueButton.onClick.AddListener(OnContinueClicked);
        }

        if (_mainMenuButton != null)
        {
            _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        EventManager.StartListening("GameVictory", OnGameVictory);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("GameVictory", OnGameVictory);
    }

    private void OnGameVictory(object data)
    {
        ShowVictoryScreen();
    }

    private void ShowVictoryScreen()
    {
        if (_victoryPanel != null)
        {
            _victoryPanel.SetActive(true);
        }

        // Set title
        if (_titleText != null)
        {
            _titleText.text = "VICTORY!";
        }

        // Show stats
        if (_statsText != null)
        {
            if (ProgressionManager.Instance != null)
            {
                _statsText.text = $"Total Runs: {ProgressionManager.Instance.TotalRuns}\n" +
                                 $"Total Victories: {ProgressionManager.Instance.TotalWins}";
            }
        }

        // Show rewards
        if (_rewardsText != null)
        {
            _rewardsText.text = $"Rewards Earned:\n" +
                               $"+ {_baseAshShardReward} Ash Shards\n" +
                               $"+ {_goldReward} Gold (spent during battle)";
        }

        // Pause game
        Time.timeScale = 0f;

        Debug.Log("Victory screen displayed!");
    }

    private void OnContinueClicked()
    {
        Debug.Log("Continue clicked - reloading gameplay scene");

        // Resume time
        Time.timeScale = 1f;

        // Reload the gameplay scene for another run
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("Main Menu clicked");

        // Resume time
        Time.timeScale = 1f;

        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
    }
}
