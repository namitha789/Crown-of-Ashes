using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }
    
    [Header("Progression Data")]
    [SerializeField] private int _totalAshShards = 0;
    [SerializeField] private int _totalRuns = 0;
    [SerializeField] private int _totalWins = 0;
    
    public int TotalAshShards => _totalAshShards;
    public int TotalRuns => _totalRuns;
    public int TotalWins => _totalWins;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadProgression();
    }
    
    public void AddAshShards(int amount)
    {
        _totalAshShards += amount;
        SaveProgression();
        EventManager.TriggerEvent("AshShardsChanged", _totalAshShards);
    }
    
    public void CompleteRun(bool victory)
    {
        _totalRuns++;
        if (victory)
        {
            _totalWins++;
        }
        SaveProgression();
    }
    
    private void SaveProgression()
    {
        PlayerPrefs.SetInt("AshShards", _totalAshShards);
        PlayerPrefs.SetInt("TotalRuns", _totalRuns);
        PlayerPrefs.SetInt("TotalWins", _totalWins);
        PlayerPrefs.Save();
    }
    
    private void LoadProgression()
    {
        _totalAshShards = PlayerPrefs.GetInt("AshShards", 0);
        _totalRuns = PlayerPrefs.GetInt("TotalRuns", 0);
        _totalWins = PlayerPrefs.GetInt("TotalWins", 0);
    }
}