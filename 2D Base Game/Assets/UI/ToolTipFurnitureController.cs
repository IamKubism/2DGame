using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipFurnitureController : MonoBehaviour
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
        //Tile t = GameObject.FindObjectOfType<MouseController>().GetTileUnderMouse();
        //if (t != null)
        //{
        //    if (t.curr_furniture == null)
        //    {
        //        myText.text = "None";
        //    }
        //    else
        //    {
        //        //myText.text = t.furniture.Describe(null,null); //ALSO REAL BAD
        //    }
        //}
    }
}
