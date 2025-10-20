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
        EventManager.StartListening("UnitDied", OnUnitDied);
    }
    
    private void OnDestroy()
    {
        EventManager.StopListening("OutpostDestroyed", OnOutpostDestroyed);
        EventManager.StopListening("UnitDied", OnUnitDied);
    }
    
    private void OnOutpostDestroyed(object data)
    {
        _outpostsDestroyed++;
        
        Debug.Log($"Outposts destroyed: {_outpostsDestroyed}/{_outpostsToDestroy}");
        
        if (_outpostsDestroyed >= _outpostsToDestroy && !_gameEnded)
        {
            TriggerVictory();
        }
    }
    
    private void OnUnitDied(object data)
    {
        if (data is Unit unit && unit.IsPlayerUnit)
        {
            // Check if all player units are dead
            Unit[] allUnits = FindObjectsOfType<Unit>();
            bool hasPlayerUnits = false;
            
            foreach (Unit u in allUnits)
            {
                if (u.IsPlayerUnit)
                {
                    hasPlayerUnits = true;
                    break;
                }
            }
            
            if (!hasPlayerUnits && !_gameEnded)
            {
                TriggerDefeat();
            }
        }
    }
    
    private void TriggerVictory()
    {
        _gameEnded = true;
        EventManager.TriggerEvent("GameVictory", null);
        Debug.Log("VICTORY!");
    }
    
    private void TriggerDefeat()
    {
        _gameEnded = true;
        EventManager.TriggerEvent("GameDefeat", null);
        Debug.Log("DEFEAT!");
    }
}