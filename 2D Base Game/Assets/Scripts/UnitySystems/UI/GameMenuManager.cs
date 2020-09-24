using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Linq;
using System;
using Newtonsoft.Json;
using HighKings;
using System.Reflection;

public class GameMenuManager : MonoBehaviour
{
    public GameObject button_prefab;
    public GameObject panel;
    public GameObject inventory_menu;
    public GameObject game_button;
    public GameObject secondary_actions;
    public GameObject selection_info;

    public MouseController mouse_controller;

    List<GameObject> active_menus; //TODO, for dynamic switching of interfaces

    /// <summary>
    /// Variable that tells the game where in the hierarchy something should be spawned
    /// </summary>
    int menu_num = 0;

    public float shift_dist = 110f;

    Dictionary<string, MenuData> menu_datas;
    MenuData next_menu;

    // Start is called before the first frame update
    void Start()
    {
        active_menus = new List<GameObject>();
        menu_datas = new Dictionary<string, MenuData>();

        string menu_data_path = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
        menu_data_path = System.IO.Path.Combine(menu_data_path, "Menus");
        menu_data_path = System.IO.Path.Combine(menu_data_path, "MenuDatas.JSON");

        menu_datas = JsonParser.instance.ParseString<Dictionary<string, MenuData>>(System.IO.File.ReadAllText(menu_data_path));
    }

    public void StartGame()
    { 
        secondary_actions = GameObject.Find("Secondary Actions");
        Destroy(GameObject.Find("Button - Start Game"));
        InitMainMenu();
        inventory_menu.SetActive(false);
        MainGame.instance.StartGame();
    }

    public void Resize()
    {
        GetComponent<AutomaticVerticalScript>().AdjustSize();
    }

    public void ToggleInventory()
    {
        if (inventory_menu.activeSelf)
        {
            inventory_menu.SetActive(false);
            active_menus.Remove(inventory_menu);
            return;
        }
        inventory_menu.SetActive(true);
        active_menus.Add(inventory_menu);
        //GameObject.FindObjectOfType<InventoryManager>().OnOpen();
    }

    public void CreateSpawnItemPane()
    {

    }


    public void CreateActionButton(string name, Action action, GameObject parent)
    {
        GameObject b = Instantiate(button_prefab);
        b.name = "Button - " + name;
        b.transform.GetComponentInChildren<Text>().text = name;

        b.GetComponent<Button>().onClick.AddListener(delegate { action(); });
        b.transform.SetParent(parent.transform);
    }



    GameObject CreateChangeCurrentTileEffectButton(GameObject parent, string name, string value, string displayName)
    {
        GameObject b = Instantiate(button_prefab);
        b.name = "Button - " + name;
        b.transform.GetComponentInChildren<Text>().text = displayName.Length > 0 ? displayName : name + " - " + value;

        b.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                //GameActionsController._instance.ChangeCurrentTileEffectValue(name, value);
            });
        b.transform.SetParent(parent.transform);
        return b;
    }

    /// <summary>
    /// For creating the highest hierarchy menu
    /// </summary>
    void InitMainMenu()
    {
        for (int i = active_menus.Count; i > 0; i -= 1)
        {
            GameObject temp = active_menus[i];
            active_menus.Remove(temp);
            Destroy(temp);
        }

        OpenMenuFromData(menu_datas["build"]);
    }

    void OpenMenuFromData(MenuData data)
    {
        CheckMenus();

        //Check if this is the null menu (If it is then we don't have to open anything)
        if (data.Id == "null")
        {
            return;
        }

        //Place Menu (For now we assume that it only gives tile effect and possibly opens a new menu)
        Vector3 menuPos = GameObject.Find("Game Menu").transform.position;
        if (active_menus.Count > 0)
        {
            menuPos = active_menus[data.hier_num - 1].transform.position + new Vector3(150f+2f, 0, 0);

        }//TODO: Menu Scaling
        GameObject pan = Instantiate(panel, menuPos, Quaternion.identity, GameObject.Find("UICanvas").transform);
        active_menus.Add(pan);

        //Get what number menu the next one to open would be
        int next_num = active_menus.Count;

        //Create all the buttons for the menu
        for (int i = 0; i < data.button_Args.Length; i +=1)
        {
            //Creates the tile effect change
            GameObject b = CreateChangeCurrentTileEffectButton(pan, data.action_Id, data.button_Args[i], data.button_Displays[i]);

            MenuData temp_next_menu;
            if (menu_datas.ContainsKey(data.menu_Args[i])== false)
            {
                Debug.Log("Could not find menu " + data.menu_Args[i]);
                temp_next_menu = menu_datas["null"];
            } else
            {
                temp_next_menu = menu_datas[data.menu_Args[i]];
            }

            //Institutes the open next menu change
            b.GetComponent<Button>().onClick.AddListener(
            delegate {
                next_menu = temp_next_menu;
                next_menu.hier_num = next_num;
                OpenMenuFromData(next_menu);
            });
        }
    }

    GameObject CreateSetMouseActionButton(GameObject parent, string display_name, string action_name)
    {
        GameObject b = Instantiate(button_prefab);
        b.name = "Button - " + name;
        b.transform.GetComponentInChildren<Text>().text = display_name.Length > 0 ? display_name : action_name;

        b.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                mouse_controller.selectable_action = ActionList.instance.GetAction(action_name);
            });
        b.transform.SetParent(parent.transform);
        return b;
    }

    GameObject CreateActionButton(GameObject parent, string display_name)
    {
        GameObject b = Instantiate(button_prefab);
        b.name = "Button - " + name;
        b.transform.GetComponentInChildren<Text>().text = display_name.Length > 0 ? display_name : "";

        b.transform.SetParent(parent.transform);
        return b;
    }

    void SetButtonDelegate(GameObject button, Action action)
    {
        button.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                action();
            }
            );
    }

    void OpenNewTileEffectChangeMenu(Dictionary<string,string> pairs)
    {
        CheckMenus();

        Vector3 menuPos = active_menus[active_menus.Count - 1].transform.position + new Vector3(110f,0,0);
        GameObject pan = Instantiate(panel, menuPos, Quaternion.identity, GameObject.Find("UICanvas").transform);
    }

    void CheckMenus()
    {
        if (menu_num > active_menus.Count)
        {
            Debug.LogError("Tried to make a menu too far out in the hierarchy");
            return;
        }

        for (int i = active_menus.Count; i > menu_num; i -= 1)
        {
            GameObject temp = active_menus[i];
            active_menus.Remove(temp);
            Destroy(temp);
        }
    }

}

public class ButtonClass
{
    public string id;
    public string display_name;
    //Need some delegates?

}

/// <summary>
/// Utility structure to tell the game how a menu should be openned
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public struct MenuData
{
    /// <summary>
    /// How the menu is referred to
    /// </summary>
    [JsonProperty]
    public string Id;

    /// <summary>
    /// This is what number the menu is made in the hierarchy
    /// </summary>
    public int hier_num;

    [JsonProperty]
    public string action_Id;

    /// <summary>
    /// The arguments that are given in button definitions.
    /// </summary>
    [JsonProperty]
    public string[] button_Args;

    /// <summary>
    /// The arguments that are given in button definitions.
    /// </summary>
    [JsonProperty]
    public string[] menu_Args;

    /// <summary>
    /// The arguments that are given in button definitions.
    /// </summary>
    [JsonProperty]
    public string[] button_Displays;
}

public struct ButtonData
{
    public string button_display;
    public string type_name;
    public string func_name;
}


//public void CreateActionButton<Variable>(string inGameNameVariant, Variable arg, GameAction<Variable> action, GameObject parent)
//{
//    GameObject b = Instantiate(buttonPrefab);
//    b.name = "Button - " + action.ActionName;
//    b.transform.GetComponentInChildren<Text>().text = inGameNameVariant;

//    b.GetComponent<Button>().onClick.AddListener(delegate { action.CallVariableAction(arg); });
//    b.transform.SetParent(parent.transform);
//}

//public void CreateMenu(MenuData data, Dictionary<string,string> buttonParticulars)
//{
//    for (int i = activeMenus.Count; i < data.hierarchyNum; i += 1)
//    {
//        activeMenus.Remove(activeMenus[i]);
//    }

//    Vector3 menuPos = GameObject.Find("Game Menu").transform.position;

//    if (activeMenus.Count > 0)
//    {
//        //In the future the shift should be determined by ui scaling also
//        menuPos = activeMenus[activeMenus.Count - 1].transform.position + new Vector3(10f, 0, 0);
//    }

//    GameObject pan = (GameObject)Instantiate(panel,
//        menuPos,
//        Quaternion.identity,
//        GameObject.Find("Game Menu").transform);

//    pan.transform.name = data.id;

//    foreach (string s in buttonParticulars.Keys)
//    {
//        GameObject toAdd = (GameObject)Instantiate(buttonPrefab);
//        toAdd.transform.name = data.buttonPrototype.name + "_" + s + "_" + buttonParticulars[s];
//        toAdd.transform.GetComponentInChildren<Text>().text = data.buttonPrototype.displayName + " - " + buttonParticulars[s];
//        GetComponent<Button>().onClick.AddListener(
//            delegate {
//                data.buttonPrototype.toCall(buttonParticulars[s], s);
//            }
//            );
//        transform.SetParent(pan.transform);
//    }

//}


//void MakeButtonForAction(string s)
//{
//    GameObject go = (GameObject)Instantiate(gameButton);
//    go.name = "Button - " + s;
//    go.transform.GetComponentInChildren<Text>().text = gActionsManager.GetActionByName(s).ActionName;
//    go.transform.SetParent(GameObject.Find("Game Menu").transform);

//    go.GetComponent<Button>().onClick.AddListener(delegate { gActionsManager.GetActionByName(s).CallAction(); });
//}

//public void ActivateGameAction(string s)
//{
//    gActionsManager.GetActionByName(s).CallAction();
//}

//public void SwitchInterface(string gmodeName)
//{
//    foreach (Button b in this.GetComponentsInChildren<Button>())
//    {
//        this.transform.DetachChildren();
//        DestroyImmediate(b.gameObject);
//    }
//    foreach(GameObject o in activeMenus)
//    {
//        o.SetActive(false);
//    }
//    activeMenus = new List<GameObject>();

//    //TODO: This should be more dynamic
//    if (inventoryMenu.activeSelf)
//        ToggleInventory();
//    if (GameObject.Find("Furniture Build Menu") != null)
//        CloseFurnitureWindow();

//    this.GetComponent<AutomaticVerticalScript>().childHeight = 25; //Todo, resizing commands for user
//    this.GetComponent<AutomaticVerticalScript>().AdjustSize();
//    gActionsManager.SwitchInterface(gmodeName);
//}


//public void CreateSpawnMenu()
//{
//    if (GameObject.Find("InventoryObjectSpawnPanel") != null)
//    {
//        Destroy(GameObject.Find("InventoryObjectSpawnPanel"));
//        return;
//    }
//    else
//    {
//        GameObject pan = (GameObject)Instantiate(panel,
//                    new Vector3(GameObject.Find("Game Menu").GetComponent<RectTransform>().rect.width, 0, 0),
//                    Quaternion.identity,
//                    GameObject.Find("UICanvas").transform);
//        pan.transform.SetParent(GameObject.Find("UICanvas").transform);
//        pan.transform.name = "InventoryObjectSpawnPanel";
//        float x = GameObject.Find("Game Menu").transform.position.x;
//        float y = GameObject.Find("Game Menu").transform.position.y;
//        pan.transform.position.Set(x + 10f, y, 0f);

//        foreach (string i in MainGame.instance.inventory_item_prototypes.Keys)
//        {
//            //Debug.Log(i);
//            CreateInventorySpawnButton(i, pan);
//        }
//    }
//}

//public void CreateInventorySpawnButton(string s, GameObject parent)
//{
//    GameObject b = Instantiate(button_prefab);

//    b.name = "Button - " + s;
//    b.transform.GetComponentInChildren<Text>().text = "Spawn " + MainGame.instance.GetInventoryItemPrototype(s).ItemNameId; //TODO Fix it to be visible name

//    b.GetComponent<Button>().onClick.AddListener(delegate { GameObject.FindObjectOfType<InventoryManager>().SpawnItemInInventory(s); });
//    b.transform.SetParent(parent.transform);
//}


//public void ManageFurnitureWindow()
//{
//    if (GameObject.Find("Furniture Build Menu") != null)
//    {
//        CloseFurnitureWindow();
//    } else
//    {
//        OpenFurnitureWindow();
//    }
//}

//public void OpenFurnitureWindow()
//{
//    BuildModeController bmc = GameObject.FindObjectOfType<BuildModeController>();

//    GameObject bm = (GameObject)Instantiate(panel);
//    bm.transform.SetParent(GameObject.Find("UICanvas").transform);
//    bm.transform.name = "Furniture Build Menu";
//    float x = GameObject.Find("Game Menu").transform.position.x; 
//    float y = GameObject.Find("Game Menu").transform.position.y;
//    bm.transform.position.Set(x + 2f, y, 0f); //FIXME: janky

//    foreach (string s in MainGame.instance.FurniturePrototypes)
//    {
//        CreateButtonFurnitureBuild(bm,bmc,s);
//    }
//    active_menus.Add(bm);
//}

//void CreateButtonFurnitureBuild(GameObject mainPane, BuildModeController bmc, string s)
//{
//    GameObject go = (GameObject)Instantiate(button_prefab);
//    go.transform.SetParent(mainPane.transform);

//    go.name = "Button - Build " + s;

//    go.transform.GetComponentInChildren<Text>().text = "Build " + MainGame.instance.GetFurnitureInGameName(s);

//    Button b = go.GetComponent<Button>();
//    //string furnID = s; //Fixme: ????

//    //b.onClick.AddListener(delegate { bmc.SetMode_BuildInstalledObject(s); });
//}