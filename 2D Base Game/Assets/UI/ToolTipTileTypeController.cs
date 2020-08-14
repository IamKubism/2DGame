using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipTileTypeController : MonoBehaviour
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
        //if (GameObject.FindObjectOfType<MouseController>().GetTileUnderMouse() != null)
        //    myText.text = GameObject.FindObjectOfType<MouseController>().GetTileUnderMouse().curr_type.display_name;
        //else
        //    myText.text = "Null";
    }
}
