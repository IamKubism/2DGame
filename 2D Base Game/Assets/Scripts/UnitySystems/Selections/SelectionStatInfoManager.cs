using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HighKings;

public class SelectionStatInfoManager : MonoBehaviour
{
    public GameController game_controller;
    public List<GameObject> active_stats;
    public Vector2 display_size;
    public Vector2 display_pos;

    // Start is called before the first frame update
    void Start()
    {
        active_stats = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewTab(Entity e)
    {

    }
}
