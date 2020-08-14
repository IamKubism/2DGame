using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using HighKings;
using System.Linq;

public class MouseController : MonoBehaviour
{
    /// <summary>
    /// TODO: Depreciate
    /// </summary>
    public GameObject tile_cursor_prefab;
    public GameObject preview_tile;

    public GameMenuManager menu_manager;

    public SelectionTabManager selection_tabs;
    public SelectionInfoManager selection_info;

    Vector3 last_frame_pos;
    Vector3 curr_frame_pos;
    Vector3 last_click_pos;
    Ray curr_frame_to_ray;
    List<RaycastHit> all_hits;

    Dictionary<SelectionComponent,string> selection_map; 

    List<GameObject> drag_preview_game_objects;
    List<GameObject> preview_objects;

    public List<SelectionComponent> active_selectables;
    int current_selectable;

    public SpriteManager sprite_manager;

    Entity curr_tile;


    ITileBasedEffect effectTocall;

    public Action<int, int, int> dragAction;

    public Action postDragAction;
    public Action preDragAction;
    public bool initializedDrag;    
    // Start is called before the first frame update
    void Start()
    {
        selection_map = new Dictionary<SelectionComponent, string>();
        active_selectables = new List<SelectionComponent>();
        drag_preview_game_objects = new List<GameObject>();

        SimplePool.Preload(tile_cursor_prefab, 100);
        all_hits = new List<RaycastHit>();
        initializedDrag = false;
        preview_tile = tile_cursor_prefab;

        //mouse_point = PointCollectionSystems.NewSinglePoint(new float[2]{0,0}, "mouse");
    }

    // Update is called once per frame
    void Update()
    {

        curr_tile = GetTileUnderMouse();
        curr_frame_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        curr_frame_pos.z = 0;

        CleanUpDisplayed();

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (GetWhichMouseButtonDown()>=0)
            {
                last_click_pos = curr_frame_pos;
                curr_frame_to_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //curr_frame_to_ray.origin = curr_frame_to_ray.origin + new Vector3(0, 0, 20f);
                //Debug.Log("X = " + last_click_pos.x + " Y = " + last_click_pos.y);
            }
            switch (GetWhichMouseButtonDrag())
            {
                case 2:
                    UpdateCameraPosition();
                    break;
                case 1:
                    //UpdateCameraPosition();
                    //Add area selection procedure
                    break;
                case 0:
                    //active_area = GetDraggedArea(last_click_pos, curr_frame_pos);
                    //PreviewImageOverTiles(active_area, preview_tile);
                    break;
                //default:
                //    Debug.Log("Clicked Unset Mouse Button");
                //    break;
            }
            switch (GetWhichMouseButtonUp())
            {
                case 2:
                    break;
                case 1:
                    //Return to select mode
                    SelectionProcedure();
                    //foreach(Selectable s in all_selections.active_selectables)
                    //{
                    //    Debug.Log(s.entity_string_id);
                    //}
                    break;
                case 0:
                    //effectTocall?.ActivateOnTiles(active_area);
                    break;
                //default:
                //    Debug.Log("Clicked Unset Mouse Button");
                //    break;
            }
            UpdateZoom("Mouse ScrollWheel");
        }

        last_frame_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        last_frame_pos.z = 0;
    }



    /// <summary>
    /// Returns the current tile under the mouse, and sets the currFramePos vector to the current mouse position
    /// </summary>
    /// <returns></returns>
    public Entity GetTileUnderMouse()
    {
        //curr_frame_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ////curr_frame_to_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (curr_frame_pos == null)
        //{
        //    Debug.LogError("currentFramePosition does not exist");
        //    return null;
        //}
        //curr_frame_pos.z = 0;
        int x = Mathf.FloorToInt(curr_frame_pos.x);
        int y = Mathf.FloorToInt(curr_frame_pos.y);
        return EntityManager.instance.GrabEntities("tile", new string[1] { $"tile_{x}_{y}_0" }).Values.ToList()[0];
    }

    public void OnGameStart()
    {
        //MainGame.instance.world.RegisterCharCreatedCallBack(
        //    (c) => 
        //    {
        //        AddSelectionComponenent(c.entity_string_id, "character", 0, sprite_manager.entity_object_map);
        //    });
        //all_selections = MainGame.instance.all_selectables;
    }

    public void AddSelectionComponenent(string entity_name, string entity_type, int priority, Dictionary<string, GameObject> parent_map)
    {
        GameObject parent = parent_map[entity_name];

        SelectionComponent selection = parent.AddComponent<SelectionComponent>();
        selection_map.Add(selection, entity_name);

        selection.entity_string_id = entity_name;
        selection.type_id = entity_type;
        selection.priority = priority;
        selection.display_name = selection.entity_string_id;

        BoxCollider box = parent.AddComponent<BoxCollider>();
        box.size = new Vector2(1f, 1f);
        box.isTrigger = true;

    }

    /// <summary>
    /// For Finding a set of tiles that is within an area given by the end position (endPos) which is given at the call time and a start position
    /// (startPos), TODO: this might be able to be done procedurally without reference to a worldcontorller
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    //private Tile[] GetDraggedArea(Vector3 startPos, Vector3 endPos)
    //{
    //    return WorldController.Instance.world.GetTilesInRectangle(startPos.x,startPos.y,
    //        endPos.x,endPos.y);
    //}

    //public void BuildModeDrag(int start_x, int end_x, int start_y, int end_y)
    //{
    //    BuildModeController bmcontrol = GameObject.FindObjectOfType<BuildModeController>();
    //    for (int x = start_x; x <= end_x; x += 1)
    //    {
    //        for (int y = start_y; y <= end_y; y += 1)
    //        {
    //            //Tile t = WorldController.Instance.world.GetTileAt(x, y, 0);

    //            if (t != null)
    //            {
    //                //bmcontrol.DoBuildFurniture(t);
    //            }
    //        }
    //    }
    //}

    public void CleanUpDisplayed()
    {
        while (drag_preview_game_objects.Count > 0)
        {
            GameObject go = drag_preview_game_objects[0];
            drag_preview_game_objects.RemoveAt(0);
            SimplePool.Despawn(go);
        }
    }

    /// <summary>
    /// Creates a preview image over a set of tiles (tilesUnder) that will have the game object (baseObject) over them
    /// This should maybe be moved elsewhere but it works here for now
    /// </summary>
    /// <param name="tilesUnder"></param>
    /// <param name="baseObject"></param>
    //public void PreviewImageOverTiles(Tile[] tilesUnder, GameObject baseObject)
    //{
    //    foreach (Tile t in tilesUnder)
    //    {
    //        GameObject go = SimplePool.Spawn(baseObject, t.TileToVector(), Quaternion.identity);
    //        drag_preview_game_objects.Add(go);
    //    }
    //}

    /// <summary>
    /// Invoke an action on a set of tiles
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="actionToDo"></param>
    //void DoAreaAction(Tile[] tiles, Action<Tile> actionToDo)
    //{
    //    foreach (Tile t in tiles)
    //    {
    //        actionToDo(t);
    //    }
    //}

    /// <summary>
    /// DEPRECIATING DO NOT USE
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void BuildDrag(int x, int y, int z)
    {
        BuildModeController bmcontrol = GameObject.FindObjectOfType<BuildModeController>();
        //bmcontrol.DoBuildFurniture(WorldController.Instance.world.GetTileAt(x, y, z));
    }

    /// <summary>
    /// Updates the zoom of the camera, which is controlled by the axis called (axisName)
    /// <param name="axisName"></param>
    /// </summary>
    public void UpdateZoom(string axisName)
    {
        Camera.main.orthographicSize -= (1.5f) * Camera.main.orthographicSize * Input.GetAxis(axisName);
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 30f);
    }

    /// <summary>
    /// Sets the Camera Position to the pos vector's position
    /// </summary>
    /// <param name="pos"></param>
    public void UpdateCameraPos(Vector3 pos)
    {
        Camera.main.transform.Translate(pos);
    }


    public void UpdateCameraPosition()
    {
        Camera.main.transform.Translate(last_frame_pos - curr_frame_pos);
    }

 //   public void DragDescribables(int x,int y, int z)
 //   {
 //       toBeDescribed.AddRange(WorldController.Instance.world.GetDescribablesAtTile(
 //                               WorldController.Instance.GetWorldTileFromFloats(x, y, z)));
 //   }

 //   public void DescribeObject(IDescribable d, string id)
 //   {
 //       FindObjectOfType<InspectorController>().InspectDescribable(d);
 ////       Debug.Log(d.Describe(id));
 //   }

 //   public void DescribeAllObjects()
 //   {
 //       foreach (IDescribable i in toBeDescribed)
 //       {
 //           DescribeObject(i, "Basic");
 //       }
 //   }

 //   public void ResetDescs()
 //   {
 //       toBeDescribed = new List<IDescribable>();
 //   }

    /// <summary>
    /// Currently I am only gonna do stuff for a one tile click and then upgrade to dragged things
    /// </summary>
    public void SelectionProcedure()
    {   
        SetActiveSelectablesFromMouse();

        if (active_selectables.Count > 0)
            Debug.Log("Active Selectable: " + GetNextSelectable().entity_string_id);
        else
            Debug.Log("No Active Selectables");

        selection_tabs.PassSelection(active_selectables);
    }


    /// <summary>
    /// Adds and sorts selectables in the thing
    /// </summary>
    /// <param name="selectable"></param>
    void AddActiveSelectable(SelectionComponent selectable)
    {
        if (active_selectables.Contains(selectable))
        {
            return;
        }
        int index = 0;
        foreach (SelectionComponent s in active_selectables)
        {
            if(s.priority >= selectable.priority)
            {
                active_selectables.Insert(index, selectable);
                return;
            }
            index += 1;
        }
        active_selectables.Add(selectable);
    }

    void RemoveActiveSelectable(SelectionComponent selected)
    {
        current_selectable = 0;
        active_selectables.Remove(selected);
    }

    /// <summary>
    /// Return the next selectable in the list by priority/ current selected
    /// </summary>
    /// <returns></returns>
    SelectionComponent GetNextSelectable()
    {
        if(active_selectables.Count == 0)
        {
            return null;
        }
        int index = current_selectable;
        current_selectable = (current_selectable + 1) % active_selectables.Count;
        return active_selectables[index];
    }

    void SetActiveSelectablesFromMouse()
    {
        //Debug.Log(curr_frame_to_ray.ToString());
        //Get all selection colliders
        List<RaycastHit> raycasts = new List<RaycastHit>(Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)));
        //Debug.Log("Raycast num: " + raycasts.Count);
        List<SelectionComponent> selects = new List<SelectionComponent>();


        foreach (RaycastHit r in raycasts)
        {
            //Debug.Log(r.point.ToString());
            if (r.transform.gameObject.GetComponent<SelectionComponent>() != null)
            {
                AddActiveSelectable(r.transform.gameObject.GetComponent<SelectionComponent>());
                selects.Add(r.transform.gameObject.GetComponent<SelectionComponent>());
            }
        }

        foreach (SelectionComponent s in active_selectables)
        {
            if (selects.Contains(s) == false)
            {
                RemoveActiveSelectable(s);
            }
        }
    }

    //RAYCAST STUFF (MAYBE I WILL USE IT LATER)
    public void SetRayCasts()
    {
        foreach (RaycastHit r in Physics.RaycastAll(curr_frame_to_ray))
        {
            all_hits.Add(r);
        }
    }

    public void ResetHits()
    {
        all_hits = new List<RaycastHit>();
    }

    void SetRayCastAtPoint(float x, float y)
    {
        foreach (RaycastHit r in Physics.RaycastAll(new Ray(new Vector3(0, 0, 0), new Vector3(x+.5f, y+.5f, 0))))
        {
            all_hits.Add(r);
        }
    }

    public void SetTileBasedEffect(ITileBasedEffect effect)
    {
        this.effectTocall = effect;
    }

    int GetWhichMouseButtonDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return 0;
        } else if (Input.GetMouseButtonDown(1))
        {
            return 1;
        } else if (Input.GetMouseButtonDown(2))
        {
            return 2;
        } else
        {
            return -1;
        }
    }
    int GetWhichMouseButtonDrag()
    {
        if (Input.GetMouseButton(0))
        {
            return 0;
        }
        else if (Input.GetMouseButton(1))
        {
            return 1;
        }
        else if (Input.GetMouseButton(2))
        {
            return 2;
        }
        else
        {
            return -1;
        }
    }

    int GetWhichMouseButtonUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            return 0;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            return 1;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            return 2;
        }
        else
        {
            return -1;
        }
    }

}


//Legacy code:
//void UpdateCursorDrags()
//{
//    Tile tileDragStart;

//    if (Input.GetMouseButtonDown(0))
//    {
//        drag_start_pos = curr_frame_pos;
//        tileDragStart = curr_tile;
//    }

//    int start_x = Mathf.FloorToInt(drag_start_pos.x);
//    int end_x = Mathf.FloorToInt(curr_frame_pos.x);
//    if (end_x < start_x)
//    {
//        int temp = start_x;
//        start_x = end_x;
//        end_x = temp;
//    }

//    int start_y = Mathf.FloorToInt(drag_start_pos.y);
//    int end_y = Mathf.FloorToInt(curr_frame_pos.y);
//    if (end_y < start_y)
//    {
//        int temp = start_y;
//        start_y = end_y;
//        end_y = temp;
//    }

//    //Clean up old drag previews
//    while (drag_preview_game_objects.Count > 0)
//    {
//        GameObject go = drag_preview_game_objects[0];
//        drag_preview_game_objects.RemoveAt(0);
//        SimplePool.Despawn(go);
//    }

//    //Execute predrag and display drag area
//    if (Input.GetMouseButton(0))
//    {
//        preDragAction?.Invoke();
//        initializedDrag = true;

//        //Display a preview of the drag area
//        for (int x = start_x; x <= end_x; x += 1)
//        {
//            for (int y = start_y; y <= end_y; y += 1)
//            {
//                Tile tempt = WorldController.Instance.world.GetTileAt(x, y, 0);
//                if (tempt != null)
//                {
//                    GameObject go = SimplePool.Spawn(tile_cursor_prefab, new Vector3(x, y, 0), Quaternion.identity);
//                    drag_preview_game_objects.Add(go);
//                }
//            }
//        }

//    }

//    //Execute drag command
//    if (Input.GetMouseButtonUp(0) && initializedDrag)
//    {
//        //if (dragAction != null)
//        //{
//        //    for (int x = start_x; x <= end_x; x += 1)
//        //    {
//        //        for (int y = start_y; y <= end_y; y += 1)
//        //        {
//        //            if (WorldController.Instance.GetWorldTileFromFloats(x, y, 0) != null)
//        //                dragAction(x, y, 0);
//        //        }
//        //    }
//        //}
//        //else
//        //{
//        //    Debug.LogError("Drag action is null");
//        //    //THIS SHOULD NOT BE USED
//        //    //Debug.LogError("dragAction is null. Consider setting a drag action for the game mode? or maybe something went wrong idk idk idk");
//        //    if (gameModeController.activeModeName.Equals("BuildMode"))
//        //    {
//        //        BuildModeDrag(start_x, end_x, start_y, end_y);
//        //    }
//        //    else if (gameModeController.activeModeName.Equals("RPGMode"))
//        //    {

//        //    }
//        //}
//        //postDragAction?.Invoke();
//        //initializedDrag = false;
//        //currTile = null;
//    }

//}