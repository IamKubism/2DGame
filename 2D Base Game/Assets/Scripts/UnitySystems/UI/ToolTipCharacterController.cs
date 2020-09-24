using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipCharacterController : MonoBehaviour
{
    Text myText;

    void Start()
    {
        myText = GetComponent<Text>();

        if (myText == null)
        {
            Debug.LogError("Mouseover no text ui component found");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
