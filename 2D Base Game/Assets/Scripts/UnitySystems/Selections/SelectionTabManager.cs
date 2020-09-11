using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HighKings;

public class SelectionTabManager : MonoBehaviour
{
    List<SelectionComponent> selections;
    public GameObject button_prefab;
    public SelectionStatInfoManager info_manager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PassSelection(List<SelectionComponent> selects)
    {
        selections = selects;
        SetAllButtons();
    }

    void SetAllButtons()
    {
        //while (transform.childCount > 0)
        //{
        //    Destroy(transform.GetChild(transform.childCount - 1).gameObject);
        //}
        foreach (SelectionComponent select in selections)
        {
            //CreateButtonForSelections(select);
        }
    }

    //void CreateButtonForSelections(SelectionComponent selected)
    //{
    //    GameObject b = Instantiate(button_prefab);
    //    b.name = "Button Set Selected - " + selected.entity_string_id;
    //    b.transform.GetComponentInChildren<Text>().text =   selected.display_name.Length > 0 ? 
    //                                                        selected.display_name : 
    //                                                        selected.type_id + " - " + selected.entity_string_id;

    //    b.GetComponent<Button>().onClick.AddListener(
    //        delegate {
    //            info_manager.SetInfoFromSelection(selected);
    //        });
    //    b.transform.SetParent(transform);
    //    b.transform.localPosition = new Vector3(0, 0, 0);
    //}


}


