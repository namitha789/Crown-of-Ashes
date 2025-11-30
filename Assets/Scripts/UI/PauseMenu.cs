using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TextMeshProUGUI _titleText;

    private bool _isPaused = false;

    private void Start()
{
    Debug.Log("PauseMenu Start() called - Script is running!");
    
    if (_pausePanel != null)
    {
        _pausePanel.SetActive(false);
    }

    if (_resumeButton != null)
    {
        _resumeButton.onClick.AddListener(OnResumeClicked);
    }

    if (_mainMenuButton != null)
    {
        _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }
}

private void Update()
{
    // Debug.Log("Update is running!");
    
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        Debug.Log("ESC pressed!");
        
        if (_isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
}

    private void Pause()
    {
        _isPaused = true;

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(true);
        }

        if (_titleText != null)
        {
            _titleText.text = "PAUSED";
        }

        Time.timeScale = 0f;

        Debug.Log("Game paused");
    }

    private void Resume()
    {
        _isPaused = false;

        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;

        Debug.Log("Game resumed");
    }

    private void OnResumeClicked()
    {
        Resume();
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
