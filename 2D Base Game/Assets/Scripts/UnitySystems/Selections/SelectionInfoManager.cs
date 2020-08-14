using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HighKings;

public class SelectionInfoManager : MonoBehaviour
{
    public GameController game_controller;
    TextMeshProUGUI text_component;

    // Start is called before the first frame update
    void Start()
    {
        text_component = GetComponentInChildren<TextMeshProUGUI>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInfoFromSelection(SelectionComponent comp)
    {
        switch (comp.type_id)
        {
            case "character":
                //text_component.text = GetCharacterInfo(comp.entity_string_id);
                break;
        }
    }

    //private string GetCharacterInfo(string entity_id)
    //{
    //    string info = "";
    //    Character to_describe = game_controller.worldController.world.character_list.GetCharacterById(entity_id); //Kinda retarded but whatever

    //    info += to_describe.entity_string_id + "\n";
    //    info += to_describe.movement_speed + "\n";

    //    return info;
    //}
}
