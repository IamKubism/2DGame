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
}
