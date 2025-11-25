using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private TextMeshProUGUI _statsText;

    private void Start()
    {
        _startButton.onClick.AddListener(OnStartClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);

        UpdateStats();
    }

    private void UpdateStats()
    {
        if (ProgressionManager.Instance != null)
        {
            _statsText.text = $"Total Runs: {ProgressionManager.Instance.TotalRuns}\n" +
                            $"Victories: {ProgressionManager.Instance.TotalWins}\n" +
                            $"Ash Shards: {ProgressionManager.Instance.TotalAshShards}";
        }
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene("Gameplay");
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit button clicked!");
        
        #if UNITY_EDITOR
            // Stop playing in the editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Quit the application in a build
            Application.Quit();
        #endif
    }
}