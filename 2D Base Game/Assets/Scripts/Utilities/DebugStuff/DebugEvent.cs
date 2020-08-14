using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inspired by a unity video I saw and realized this is really good for debugging purposes:
/// See: https://www.youtube.com/watch?v=raQ3iHhE_Kk&feature=youtu.be
/// </summary>
[CreateAssetMenu]
public class DebugEvent : ScriptableObject
{
    private List<DebugListener> listeners = new List<DebugListener>();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i -= 1)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(DebugListener debugListener)
    {
        if (listeners.Contains(debugListener) == false)
            listeners.Add(debugListener);
    }

    public void UnRegisterListener(DebugListener debugListener)
    {
        if (listeners.Contains(debugListener))
            listeners.Remove(debugListener);
    }
}
