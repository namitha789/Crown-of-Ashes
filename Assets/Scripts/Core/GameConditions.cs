using UnityEngine;

public class GameConditions : MonoBehaviour
{
    [Header("Victory Conditions")]
    [SerializeField] private int _outpostsToDestroy = 3;

    private int _outpostsDestroyed = 0;
    private bool _gameEnded = false;

    private void Start()
    {
        EventManager.StartListening("OutpostDestroyed", OnOutpostDestroyed);
        EventManager.StartListening("GameDefeat", OnGameDefeat);

        Debug.Log($"Game Conditions: Need to destroy {_outpostsToDestroy} outposts to win");
    }

    private void OnDestroy()
    {
        EventManager.StopListening("OutpostDestroyed", OnOutpostDestroyed);
        EventManager.StopListening("GameDefeat", OnGameDefeat);
    }

    private void OnOutpostDestroyed(object data)
    {
        if (_gameEnded) return;

        _outpostsDestroyed++;

        Debug.Log($"Outposts destroyed: {_outpostsDestroyed}/{_outpostsToDestroy}");

        if (_outpostsDestroyed >= _outpostsToDestroy)
        {
            TriggerVictory();
        }
    }

    private void OnGameDefeat(object data)
    {
        if (_gameEnded) return;

        TriggerDefeat();
    }

    private void TriggerVictory()
    {
        if (_gameEnded) return;

        _gameEnded = true;

        // Award rewards
        if (ProgressionManager.Instance != null)
        {
            int ashShards = 100; // Base reward
            ProgressionManager.Instance.AddAshShards(ashShards);
            ProgressionManager.Instance.CompleteRun(true);

            Debug.Log($"VICTORY! Awarded {ashShards} ash shards");
        }

        EventManager.TriggerEvent("GameVictory", null);

        // Pause game
        Time.timeScale = 0f;
    }

    private void TriggerDefeat()
    {
        if (_gameEnded) return;

        _gameEnded = true;

        // Record defeat
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.CompleteRun(false);
            Debug.Log("DEFEAT! Command Center destroyed");
        }

        EventManager.TriggerEvent("GameDefeat", null);

        // Pause game
        Time.timeScale = 0f;
    }
}