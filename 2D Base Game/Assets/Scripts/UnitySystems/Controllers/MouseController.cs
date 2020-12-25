using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Psingine;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Psingine.UI;

namespace Psingine
{
    public class MouseController : MonoBehaviour
    {
        public static MouseController instance;

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

        public List<Entity> selected_entities;
        int current_selectable;

        public SpriteManager sprite_manager;

        Entity curr_tile;
        public Entity main_selected;
        public EntityAction selectable_action;

        /// <summary>
        /// Events called on clicks 
        /// </summary>
        public List<Event> curr_click_events;
        public HashSet<Entity> clicked_entities;

        Func<Entity> curr_retrieval_action;
        public static int curr_z;

        private void Awake()
        {
            curr_click_events = new List<Event>();
            clicked_entities = new HashSet<Entity>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (instance == null)
            {
                instance = this;
            }

            selected_entities = new List<Entity>();
            drag_preview_game_objects = new List<GameObject>();

            SimplePool.Preload(tile_cursor_prefab, 100);
            preview_tile = tile_cursor_prefab;
            curr_z = 0;
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
                if (GetWhichMouseButtonDown() >= 0)
                {
                    last_click_pos = curr_frame_pos;
                }
                switch (GetWhichMouseButtonDown())
                {

                    case 2:
                        break;
                    case 1:
                        CleanUpTargets();
                        break;
                    case 0:
                        break;
                }
                switch (GetWhichMouseButtonDrag())
                {
                    case 2:
                        UpdateCameraPosition();
                        break;
                    case 1:
                        SetTargets();
                        break;
                    case 0:
                        break;
                }
                switch (GetWhichMouseButtonUp())
                {
                    case 2:
                        InvokeClickEvent(2);
                        break;
                    case 1:
                        InvokeClickEvent(1);
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
        public static Entity GetTileUnderMouse()
        {
            int x = Mathf.FloorToInt(instance.curr_frame_pos.x);
            int y = Mathf.FloorToInt(instance.curr_frame_pos.y);
            return World.instance.GetTileFromCoords(x, y, 0);
        }

        public Position GetPositionUnderMouse()
        {
            return GetTileUnderMouse().GetComponent<Position>();
        }

        public void InvokeClickAction()
        {
            
            //Debug.Log("Invoking click");
            Entity target = curr_retrieval_action?.Invoke();
            if (main_selected != null && target != null)
            {
                selectable_action?.Invoke(main_selected, target);
                //Debug.Log("Invoked click");
            }
            
        }

        public void InvokeClickEvent(int button_num)
        {
            if(curr_click_events.Count <= button_num || curr_click_events[button_num] == null)
            {
                Debug.Log("No click event");
                return;
            }
            Event ev = new Event(curr_click_events[button_num]);
            ev.SetParamValue("targets", new HashSet<Entity>(clicked_entities), (l1, l2) =>
            {
                return l2;
            });
            ev.Invoke(main_selected);
        }

        public static void SetClickAction(EntityAction action)
        {
            instance.selectable_action = action;
            instance.curr_retrieval_action = ActionList.instance.GetRetrievalAction(action.retrieval_action);
            Debug.Log($"Set action to: {instance.selectable_action.action_id}, retrieval action: {instance.curr_retrieval_action.ToString()}");
        }

        public static void SetClickAction(string action_id)
        {
            instance.selectable_action = ActionList.instance.GetAction(action_id);
            instance.curr_retrieval_action = ActionList.instance.GetRetrievalAction(instance.selectable_action.retrieval_action);
            Debug.Log($"Set action to: {action_id}, retrieval action: {instance.selectable_action.retrieval_action}");
        }

        public void OnGameStart()
        {
            SetClickAction("AssignMovement");
            Debug.Log("Set movement action");
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

        //TODO: When we do chunk loading we need to make sure these numbers don't get huge
        public void UpdateCameraPosition()
        {
            Camera.main.transform.Translate(last_frame_pos - curr_frame_pos);
        }

        public void CleanUpTargets()
        {
            clicked_entities = new HashSet<Entity>();
        }

        public void SetTargets()
        {

            //Clean up old drag previews
            for(int i = drag_preview_game_objects.Count; i > 0; i -= 1)
            {
                GameObject go = drag_preview_game_objects[i-1];
                drag_preview_game_objects.RemoveAt(i - 1);
                SimplePool.Despawn(go);
            }

            int[] p1 = new int[3] { Mathf.FloorToInt(last_click_pos.x), Mathf.FloorToInt(last_click_pos.y), curr_z };
            int[] p2 = new int[3] { Mathf.FloorToInt(curr_frame_pos.x), Mathf.FloorToInt(curr_frame_pos.y), curr_z };

            //Get objects that don't have a selectable component basically (and make the tile drag preview objects)
            List<Entity> tiles = World.instance.GetTileArea(p1, p2);
            foreach (Entity tile in tiles)
            {
                clicked_entities.Add(tile);
                foreach(Entity e in tile.GetComponent<Cell>().occupants)
                {
                    clicked_entities.Add(e);
                }
                GameObject go = SimplePool.Spawn(tile_cursor_prefab, new Vector3(tile.GetComponent<Position>().x, tile.GetComponent<Position>().y, 0), Quaternion.identity);
                drag_preview_game_objects.Add(go);
            }

            //Get possible selectables outside of the drag area by tiles/ aren't in the occupants for whatever reason
            Vector3 center = (1 / 2) * last_click_pos + (1 / 2) * curr_frame_pos;
            Vector3 half_extents = (1 / 2) * (last_click_pos - curr_frame_pos) + new Vector3(0,0,20);
            Collider[] selects = Physics.OverlapBox(center, half_extents);
            List<GameObject> objs = new List<GameObject>();
            foreach (Collider col in selects)
            {
                objs.Add(col.gameObject);
            }
            foreach(Entity e in GameObjectManager.instance.GetEntitiesFromObjects(objs))
            {
                selected_entities.Add(e);
            }
        }

        /// <summary>
        /// Currently I am only gonna do stuff for a one tile click and then upgrade to dragged things
        /// </summary>
        public void SelectionProcedure()
        {
            SetActiveSelectablesFromMouse();
            main_selected = GetNextSelectable();
            selection_info.MakeDisplays(main_selected);
        }

        void AddActiveSelectable(Entity selectable)
        {
            if (selected_entities.Contains(selectable))
            {
                return;
            }
            SelectionComponent select_comp = selectable.GetComponent<SelectionComponent>();
            for (int i = 0; i < selected_entities.Count; i += 1)
            {
                if (selected_entities[i].GetComponent<SelectionComponent>().priority >= select_comp.priority)
                {
                    selected_entities.Insert(i, selectable);
                    return;
                }
            }
            selected_entities.Add(selectable);
        }

        void RemoveActiveSelectable(SelectionComponent selected)
        {
            current_selectable = 0;
        }

        void RemoveActiveSelectable(Entity selected)
        {
            current_selectable = 0;
            selected_entities.Remove(selected);
        }

        Entity GetNextSelectable()
        {
            //Debug.Log(selected_entities.Count);
            if (selected_entities.Count == 0)
            {
                return null;
            }
            int index = current_selectable;
            current_selectable = (current_selectable + 1) % selected_entities.Count;
            //Debug.Log($"current_selectable :{current_selectable}");
            return selected_entities[index];
        }

        public static Entity GetSelectableUnderMouse()
        {
            List<RaycastHit> raycasts = new List<RaycastHit>(Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)));
            Debug.Log("Raycast num: " + raycasts.Count);
            List<GameObject> sels = new List<GameObject>();

            foreach (RaycastHit r in raycasts)
            {
                sels.Add(r.transform.gameObject);
            }

            List<Entity> selects = GameObjectManager.instance.GetEntitiesFromObjects(sels);
            Entity select = null;

            for (int i = selects.Count; i > 0; i -= 1)
            {
                if (selects[i - 1].HasComponent("SelectionComponent"))
                {
                    if (select == null || selects[i - 1].GetComponent<SelectionComponent>("SelectionComponent").priority < select.GetComponent<SelectionComponent>("SelectionComponent").priority)
                    {
                        select = selects[i - 1];
                    }
                }
            }

            return select;
        }

        void SetActiveSelectablesFromMouse()
        {
            List<RaycastHit> raycasts = new List<RaycastHit>(Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition)));
            //Debug.Log("Raycast num: " + raycasts.Count);
            List<GameObject> sels = new List<GameObject>();

            foreach (RaycastHit r in raycasts)
            {
                sels.Add(r.transform.gameObject);
            }

            List<Entity> selects = GameObjectManager.instance.GetEntitiesFromObjects(sels);

            for (int i = selected_entities.Count; i > 0; i -= 1)
            {
                //Debug.Log(selected_entities[i - 1].ToString());
                if (selects.Contains(selected_entities[i - 1]) == false)
                {
                    RemoveActiveSelectable(selected_entities[i - 1]);
                }
            }

            foreach (Entity e in selects)
            {
                //Debug.Log(e.entity_string_id);
                if (e.HasComponent("SelectionComponent"))
                    AddActiveSelectable(e);
            }
        }

        public void SetClickEvent(int button_num, Event ev)
        {
            while(curr_click_events.Count < button_num+1)
            {
                curr_click_events.Add(EventManager.instance.GetEvent("NullEvent"));
                Debug.Log(curr_click_events.Count);
            }
            curr_click_events[button_num] = new Event(ev);
        }

        public void SetActiveRetrievalAction(string tag_name)
        {
            curr_retrieval_action = ActionList.instance.GetRetrievalAction(tag_name);
        }


        int GetWhichMouseButtonDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                return 0;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                return 1;
            }
            else if (Input.GetMouseButtonDown(2))
            {
                return 2;
            }
            else
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
            //Debug.Log("Getting mouse button");
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