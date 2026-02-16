using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central event system for game-wide communication
/// </summary>
public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("EventManager");
                _instance = go.AddComponent<EventManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    
    private Dictionary<string, Action<object>> _eventDictionary = new Dictionary<string, Action<object>>();
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public static void StartListening(string eventName, Action<object> listener)
    {
        if (Instance._eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent += listener;
            Instance._eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent = listener;
            Instance._eventDictionary.Add(eventName, thisEvent);
        }
    }
    
    public static void StopListening(string eventName, Action<object> listener)
    {
        if (_instance == null) return;
        
        if (Instance._eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            Instance._eventDictionary[eventName] = thisEvent;
        }
    }
    
    public static void TriggerEvent(string eventName, object data)
    {
        if (Instance._eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent?.Invoke(data);
        }
    }
    
    private void OnDestroy()
    {
        _eventDictionary.Clear();
    }
}