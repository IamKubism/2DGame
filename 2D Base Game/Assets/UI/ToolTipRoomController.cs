using UnityEngine;
using UnityEngine.UI;

public class ToolTipRoomController : MonoBehaviour
{
    private Text myText;

    private void Start()
    {
        myText = GetComponent<Text>();

        if (myText == null)
        {
            Debug.LogError("Mouseover no text ui component found");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    //    if (GameObject.FindObjectOfType<MouseController>().GetTileUnderMouse() != null)
    //        if (GameObject.FindObjectOfType<MouseController>().GetTileUnderMouse().room != null)
    //            myText.text = GameObject.FindObjectOfType<MouseController>().GetTileUnderMouse().room.name;
    }
}