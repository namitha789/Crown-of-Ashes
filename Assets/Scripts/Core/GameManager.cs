using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private GameState _currentState;
    
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _currentState = GameState.MainMenu;
    }
    
    public void ChangeState(GameState newState)
    {
        _currentState = newState;
        OnGameStateChanged(_currentState);
    }
    
    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                break;
            case GameState.Playing:
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                break;
        }
        
        EventManager.TriggerEvent("GameStateChanged", _currentState);
    }
}