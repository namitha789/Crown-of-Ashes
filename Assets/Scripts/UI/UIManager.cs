using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Singleton pattern - only one UIManager exists
    public static UIManager Instance { get; private set; }
    
    // References to our UI panels (we'll assign these in Inspector)
    [Header("UI Panels")]
    [SerializeField] private GameObject _gameplayHUD;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _mainMenu;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    private void Start()
    {
        // Subscribe to game state changes
        EventManager.StartListening("GameStateChanged", OnGameStateChanged);
        
        // Start by showing main menu
        ShowMainMenu();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe when this object is destroyed
        EventManager.StopListening("GameStateChanged", OnGameStateChanged);
    }
    
    // This gets called when game state changes
    private void OnGameStateChanged(object stateObj)
    {
        // Convert the object to GameState
        if (stateObj is GameManager.GameState state)
        {
            switch (state)
            {
                case GameManager.GameState.MainMenu:
                    ShowMainMenu();
                    break;
                case GameManager.GameState.Playing:
                    ShowGameplayHUD();
                    break;
                case GameManager.GameState.Paused:
                    ShowPauseMenu();
                    break;
            }
        }
    }
    
    private void ShowMainMenu()
    {
        _mainMenu.SetActive(true);
        _gameplayHUD.SetActive(false);
        _pauseMenu.SetActive(false);
        Debug.Log("Showing Main Menu");
    }
    
    private void ShowGameplayHUD()
    {
        _mainMenu.SetActive(false);
        _gameplayHUD.SetActive(true);
        _pauseMenu.SetActive(false);
        Debug.Log("Showing Gameplay HUD");
    }
    
    private void ShowPauseMenu()
    {
        _pauseMenu.SetActive(true);
        Debug.Log("Showing Pause Menu");
    }
    
    // PUBLIC METHODS that buttons can call
    public void OnStartGameClicked()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }
    
    public void OnPauseClicked()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Paused);
    }
    
    public void OnResumeClicked()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }
    
    public void OnMainMenuClicked()
    {
        GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);
    }
}