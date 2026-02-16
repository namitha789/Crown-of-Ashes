using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatPanel;
    
    [Header("Victory Elements")]
    [SerializeField] private TextMeshProUGUI _rewardsText;
    [SerializeField] private Button _continueButton;
    
    [Header("Defeat Elements")]
    [SerializeField] private Button _retryButton;
    
    private void OnEnable()
    {
        EventManager.StartListening("GameWon", OnGameWon);
        EventManager.StartListening("GameLost", OnGameLost);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening("GameWon", OnGameWon);
        EventManager.StopListening("GameLost", OnGameLost);
    }
    
    private void Start()
    {
        _victoryPanel.SetActive(false);
        _defeatPanel.SetActive(false);
        
        if (_continueButton != null)
        {
            _continueButton.onClick.AddListener(OnContinueClicked);
        }
        
        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(OnRetryClicked);
        }
    }
    
    private void OnGameWon(object data)
    {
        Time.timeScale = 0f; // Pause game
        
        _victoryPanel.SetActive(true);
        
        // Calculate rewards (from ProgressionManager)
        int goldEarned = ProgressionManager.Instance.GetGoldEarned();
        int ashShards = ProgressionManager.Instance.GetAshShardsEarned();
        
        if (_rewardsText != null)
        {
            _rewardsText.text = $"Rewards:\n+{goldEarned} Gold\n+{ashShards} Ash Shards";
        }
        
        Debug.Log("Victory screen displayed!");
    }
    
    private void OnGameLost(object data)
    {
        Time.timeScale = 0f; // Pause game
        
        _defeatPanel.SetActive(true);
        
        Debug.Log("Defeat screen displayed!");
    }
    
    private void OnContinueClicked()
    {
        Time.timeScale = 1f; // Resume
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    private void OnRetryClicked()
    {
        Time.timeScale = 1f; // Resume
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
