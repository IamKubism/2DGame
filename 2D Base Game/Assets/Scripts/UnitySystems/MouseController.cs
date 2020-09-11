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
    public SelectionStatInfoManager selection_info;

    Vector3 last_frame_pos;
    Vector3 curr_frame_pos;
    Vector3 last_click_pos;

    List<GameObject> drag_preview_game_objects;
    List<GameObject> preview_objects;

    public List<SelectionComponent> active_selectables;
    public List<Entity> selected_entities;
    int current_selectable;

    public SpriteManager sprite_manager;

    Entity curr_tile;
    public Entity main_selected;

    ITileBasedEffect effectTocall;

    // Start is called before the first frame update
    void Start()
    {
        selected_entities = new List<Entity>();
        active_selectables = new List<SelectionComponent>();
        drag_preview_game_objects = new List<GameObject>();

        SimplePool.Preload(tile_cursor_prefab, 100);
        preview_tile = tile_cursor_prefab;
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
            }
            switch (GetWhichMouseButtonDrag())
            {
                case 2:
                    UpdateCameraPosition();
                    break;
                case 1:
                    break;
                case 0:
                    break;
            }
            switch (GetWhichMouseButtonUp())
            {
                case 2:
                    break;
                case 1:
                    AssignMovement();
                    break;
                case 0:
                    SelectionProcedure();
                    break;
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
        int x = Mathf.FloorToInt(curr_frame_pos.x);
        int y = Mathf.FloorToInt(curr_frame_pos.y);
        return World.instance.GetTileFromCoords(x,y,0);
    }

    public Position GetPositionUnderMouse()
    {
        return GetTileUnderMouse().GetComponent<Position>("Position");
    }

    public void AssignMovement()
    {
        if(main_selected != null)
        {
            Movers.instance.MoverPathMaker(GetPositionUnderMouse(), main_selected);
        }
    }

    public void OnGameStart()
    {
    }

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

    /// <summary>
    /// Currently I am only gonna do stuff for a one tile click and then upgrade to dragged things
    /// </summary>
    public void SelectionProcedure()
    {   
        SetActiveSelectablesFromMouse();

        Entity next_selectable = GetNextSelectable();
        main_selected = next_selectable;

        if (active_selectables.Count > 0)
            Debug.Log("Active Selectable: " + next_selectable.ToString());
        else
            Debug.Log("No Active Selectables");

        selection_tabs.PassSelection(active_selectables);
    }

    void AddActiveSelectable(Entity selectable)
    {
        if (selected_entities.Contains(selectable))
        {
            return;
        }
        SelectionComponent select_comp = selectable.GetComponent<SelectionComponent>("selection_component");
        int index = 0;
        foreach (SelectionComponent s in active_selectables)
        {
            if (s.priority >= select_comp.priority)
            {
                active_selectables.Insert(index, select_comp);
                selected_entities.Insert(index, selectable);
                return;
            }
            index += 1;
        }
        active_selectables.Add(select_comp);
        selected_entities.Add(selectable);
    }

    void RemoveActiveSelectable(SelectionComponent selected)
    {
        current_selectable = 0;
        active_selectables.Remove(selected);
    }

    void RemoveActiveSelectable(Entity selected)
    {
        current_selectable = 0;
        selected_entities.Remove(selected);
        active_selectables.Remove(selected.GetComponent<SelectionComponent>("SelectionComponent"));
    }

    Entity GetNextSelectable()
    {
        Debug.Log(active_selectables.Count);
        if (active_selectables.Count == 0)
        {
            return null;
        }
        int index = current_selectable;
        current_selectable = (current_selectable + 1) % active_selectables.Count;
        return selected_entities[index];
    }

    void SetActiveSelectablesFromMouse()
    {
        List<RaycastHit> raycasts = new List<RaycastHit>(Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)));
        Debug.Log("Raycast num: " + raycasts.Count);
        List<GameObject> sels = new List<GameObject>();

        foreach (RaycastHit r in raycasts)
        {
            sels.Add(r.transform.gameObject);
        }

        List<Entity> selects = GameObjectManager.instance.GrabEntitiesFromObjects(sels);

        foreach (Entity e in selected_entities)
        {
            Debug.Log(e.entity_string_id);
            if (selects.Contains(e) == false)
            {
                RemoveActiveSelectable(e);
            }
        }

        foreach (Entity e in selects)
        {
            Debug.Log(e.entity_string_id);
            if(e.HasComponent("SelectionComponent"))
                AddActiveSelectable(e);
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


//public void SetMainSelected(Entity e)
//{
//    List<RaycastHit> raycasts = new List<RaycastHit>(Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)));
//    List<SelectionComponent> selects = new List<SelectionComponent>();

//    foreach (RaycastHit r in raycasts)
//    {
//        if (r.transform.gameObject.GetComponent<SelectionComponent>() != null)
//        {
//            selects.Add(r.transform.gameObject.GetComponent<SelectionComponent>());
//        }
//    }
//    if(main_selected == default)
//    {

//    }
//    foreach (SelectionComponent s in active_selectables)
//    {
//        if (selects.Contains(s) == false)
//        {
//            RemoveActiveSelectable(s);
//        }
//    }
//}

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


//public void AddSelectionComponent(string entity_name, string entity_type, int priority, Dictionary<string, GameObject> parent_map)
//{
//    GameObject parent = parent_map[entity_name];

//    SelectionComponent selection = parent.AddComponent<SelectionComponent>();
//    selection_map.Add(selection, entity_name);

//    selection.entity_string_id = entity_name;
//    selection.type_id = entity_type;
//    selection.priority = priority;
//    selection.display_name = selection.entity_string_id;

//    BoxCollider box = parent.AddComponent<BoxCollider>();
//    box.size = new Vector2(1f, 1f);
//    box.isTrigger = true;
//}

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


///// <summary>
///// Adds and sorts selectables in the thing
///// </summary>
///// <param name="selectable"></param>
//void AddActiveSelectable(SelectionComponent selectable)
//{
//    if (active_selectables.Contains(selectable))
//    {
//        return;
//    }
//    int index = 0;
//    foreach (SelectionComponent s in active_selectables)
//    {
//        if(s.priority >= selectable.priority)
//        {
//            active_selectables.Insert(index, selectable);
//            return;
//        }
//        index += 1;
//    }
//    active_selectables.Add(selectable);
//}