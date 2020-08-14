using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutomaticVerticalScript : MonoBehaviour
{

    public float childHeight = 35f;

    // Start is called before the first frame update
    void Start()
    {
        AdjustSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustSize()
    {
        Vector2 size = this.GetComponent<RectTransform>().sizeDelta;

        size.y = this.transform.childCount * childHeight;
        
        foreach(Button go in this.GetComponentsInChildren<Button>())
        {
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(150, childHeight);
        }

        this.GetComponent<RectTransform>().sizeDelta = size;
    }
}

public class AutomaticButtonFillScript : MonoBehaviour
{
    public float child_height = 35f;
    public float child_width = 100f;
    public float max_width;
    public float max_height;

    private void Start()
    {
        AdjustButtons();
    }

    public void AdjustButtons()
    {
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            b.GetComponent<RectTransform>().sizeDelta = new Vector2(child_width, child_height);
        }
    }
}
