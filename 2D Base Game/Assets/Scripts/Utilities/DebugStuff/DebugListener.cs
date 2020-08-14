using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Inspired by a unity video I saw and realized this is really good for debugging purposes:
/// See: https://www.youtube.com/watch?v=raQ3iHhE_Kk&feature=youtu.be
/// </summary>
public class DebugListener : MonoBehaviour
{
    public DebugEvent callEvent;
    public UnityEvent Response;

    private void OnEnable()
    {
        callEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        callEvent.UnRegisterListener(this);
    }

    public void OnEventRaised()
    {
        Response.Invoke();
    }
}
