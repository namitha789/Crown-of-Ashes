using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static Dictionary<string, Action<object>> _eventDictionary = new Dictionary<string, Action<object>>();
    
    public static void StartListening(string eventName, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent += listener;
            _eventDictionary[eventName] = thisEvent;
        }
        else
        {
            _eventDictionary.Add(eventName, listener);
        }
    }
    
    public static void StopListening(string eventName, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            _eventDictionary[eventName] = thisEvent;
        }
    }
    
    public static void TriggerEvent(string eventName, object data = null)
    {
        if (_eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent?.Invoke(data);
        }
    }
}