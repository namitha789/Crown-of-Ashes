using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _defeatPanel;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _causeText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("Audio")]
    [SerializeField] private AudioClip _defeatMusic;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private float _musicVolume = 0.5f;

    private void Start()
    {
        if (_defeatPanel != null)
        {
            _defeatPanel.SetActive(false);
        }

        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(OnRetryClicked);
        }

        if (_mainMenuButton != null)
        {
            _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        EventManager.StartListening("GameDefeat", OnGameDefeat);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("GameDefeat", OnGameDefeat);
    }

    private void OnGameDefeat(object data)
    {
        ShowDefeatScreen();
    }

    private void ShowDefeatScreen()
    {
        if (_defeatPanel != null)
        {
            _defeatPanel.SetActive(true);
        }

        // Set title
        if (_titleText != null)
        {
            _titleText.text = "DEFEAT";
        }

        // Show cause of defeat
        if (_causeText != null)
        {
            _causeText.text = "Your Command Center was destroyed!";
        }

        // Show stats
        if (_statsText != null)
        {
            if (ProgressionManager.Instance != null)
            {
                int losses = ProgressionManager.Instance.TotalRuns - ProgressionManager.Instance.TotalWins;
                _statsText.text = $"Total Runs: {ProgressionManager.Instance.TotalRuns}\n" +
                                 $"Victories: {ProgressionManager.Instance.TotalWins}\n" +
                                 $"Defeats: {losses}";
            }
        }

        // Play defeat music
        PlayDefeatMusic();

        // Pause game
        Time.timeScale = 0f;

        Debug.Log("Defeat screen displayed!");
    }

    private void PlayDefeatMusic()
    {
        if (_defeatMusic != null && _musicSource != null)
        {
            _musicSource.clip = _defeatMusic;
            _musicSource.loop = true;
            _musicSource.volume = _musicVolume;
            _musicSource.Play();
        }
    }

    private void OnRetryClicked()
    {
        Debug.Log("Retry clicked - reloading gameplay scene");

        // Resume time
        Time.timeScale = 1f;

        // Reload the gameplay scene
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