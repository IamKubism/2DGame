using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using HighKings;

/// <summary>
/// The main class that contains all tiles and facilitates changes between types, for the time being. In the future I should rename/make this
/// "Chunks" and move some of the procedures to an outside system
/// </summary>
[JsonObject]
public class World
{
    int width;
    int length;
    int height;
    Action<float> cb_update_world;
    
    //TODO: Make this handle multichunks
    public Path_TileGraph graph { get; protected set; }
    protected Path_Astar utility_astar;
    public Positions entity_positions;

    //TODO: Make this handle multichunks
    public NodeChunk tile_map;
    public Movers entity_movers;
    
    /// <summary>
    /// The currently running world (I might want to change this in the future)
    /// </summary>
    static public World _current { get; protected set; }

    public World(int width, int length, int height)
    {
        _current = this;
        this.width = width;
        this.length = length;
        this.height = height;

        //entity_positions = new Positions();
    }

    void SetUpTiles(int width, int length, int height)
    {
        this.width = width;
        this.length = length;
        this.height = height; //TODO, make height do something

        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        tile_map = new NodeChunk(width, length, height);
        watch.Stop();
        Debug.Log($"Tile map made in: {watch.Elapsed}");

        watch.Restart();
        graph = new Path_TileGraph(tile_map);
        watch.Stop();
        Debug.Log($"Graph made in: {watch.Elapsed}");
    }

    public void Update(float deltaT)
    {

        if (cb_update_world != null)
        {
            cb_update_world(deltaT);
        }

        //entity_movers.UpdateMovement(deltaT);
        //Debug.Log("test_mover location: X = " + movers.MoverPositionTo2DFloatPosition("test_mover").Item1 + " Y = " + movers.MoverPositionTo2DFloatPosition("test_mover").Item2);
    }

    public void SetUpWorld()
    {
        _current = this;

        SetUpTiles(width, length, height);

        //Debug.Log("World set up");
    }

    public int Width
    {
        get
        {
            return width;
        }
    }

    public int Height
    {
        get
        {
            return height;
        }
    }

    public int Length
    {
        get
        {
            return length;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///
    ///     CallBack Reigsters and Unregisters and calling
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //Calls

    public void CallCBUpdateWorld(float fl)
    {
        cb_update_world(fl);
    }


    //Registers

    public void RegisterUpdateCB(Action<float> cb)
    {
        cb_update_world += cb;
    }


    // Unregisters

    public void UnRegisterUpdateCB(Action<float> cb)
    {
        cb_update_world -= cb;
    }


    /// <summary>
    /// TODO: This should only have to reference one thing to get all the stuff
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // SAVING AND LOADING
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {

    }


    public World()
    {
    }
}

//LEGACY CODE:
//void CreateTile(int x, int y, int z, string type_id)
//{
//    /* Prototyping for general entity creation protocol
//     * Need Id Initializer
//     * Adds to entity list
//     * Needs tile type
//     * Needs to tell the sprite components what it gets
//     * Needs tile cost
//     * thats all for now
//     */
//    string entity_id = "tile_" + x + "_" + y + "_" + z;
//    entity_positions.SetTilePosition(entity_id, x, y, z);
//    tile_types.Add(entity_id, type_id);

//    //MainGame.instance.tile_prototypes.ChangeEntityType(entity_id, type_id);
//    MainGame.instance.CallOnRenderedCreated(entity_id);
//}

//public void Initialize() //TODO Maybe make this better it currently is just a bug bandage
//{
//    //SetUpRoomDictionary();
//    foreach (Tile t in tiles)
//    {
//        if(t.curr_furniture != null && cb_furniture_created != null)
//        {
//            cb_furniture_created(t.curr_furniture);
//        }
//    }
//    foreach (string s in character_list.char_list.Keys)
//    {
//        if(cb_character_created != null)
//        {
//            cb_character_created(character_list.char_list[s]);
//        }
//    }
//}

//public void PlaceFurniture(string objType, Tile t)
//{
//    //TODO Rotations + bigger objects
//    if (MainGame.instance.furniture_dictionary.ContainsKey(objType) == false)
//    {
//        Debug.LogError("PlaceInstalledObject Tried to place an unidentified object: " + objType);
//        return;
//    }

//    //Debug.Log(objType + " " + t.X + " " + t.Y);
//    Furniture obj = Furniture.PlaceInstance(MainGame.instance.furniture_dictionary[objType], t);

//    if (obj == null)
//    {
//        Debug.LogError("Something weird happened placing an object");
//        return;
//    }

//    if (cb_furniture_created != null)
//    {
//        cb_furniture_created(obj);
//        Debug.Log("Called furniture created");
//    }

//    active_furnitures.Add(obj);

//    //TODO: Shouldn't trigger every single time
//    if (t.is_room_dividing && needs_initializing == false)
//    {
//        //Room.DoRoomFloodFill(t);
//    }
//    //Debug.Log("Placed object " + objType + " at (" + obj.tile.X + "," + obj.tile.Y + ")");
//}

//public bool IsFurniturePlacementValid(string f, Tile t)
//{
//    return MainGame.instance.furniture_dictionary[f].func_pos_valid(t);
//}

//public Furniture GetFurniturePrototype(string objtype)
//{
//    if (MainGame.instance.furniture_dictionary.ContainsKey(objtype) == false)
//    {
//        Debug.LogError("GetFurniturePrototype tried to pull object that does not exist");
//        return null;
//    }
//    return MainGame.instance.furniture_dictionary[objtype];
//}

//public bool CheckTileIsOutside(Tile t)
//{
//    //If I can reach the lower left edge, I must be outside
//    //utility_astar = new Path_Astar(graph, t, tiles[0,0,0], true);
//    //return utility_astar.Length() != 0;
//}

//// Idk If I really needed this andahsgdjas
//public bool CanReachTiles(Tile start, Tile end)
//{
//    utility_astar = new Path_Astar(graph, start, end);
//    return utility_astar.Length() != 0;
//}

//public void RandomizeTiles()
//{
//    for (int x = 0; x < this.width; x += 1)
//    {
//        for (int y = 0; y < this.length; y += 1)
//        {
//            for (int z = 0; z < this.height; z += 1)
//            {
//                if (UnityEngine.Random.Range(0, 2) == 0)
//                {
//                    tiles[x, y, z].curr_type = MainGame.instance.tile_types_map.GetPrototypeVal("empty");
//                } else
//                {
//                    tiles[x, y, z].curr_type = MainGame.instance.tile_types_map.GetPrototypeVal("stone_rough");
//                }
//            }
//        }
//    }
//    Debug.Log("World Randomized");
//}

//public Tile GetTileAt(int x, int y, int z)
//{
//    if (x < 0 || y < 0 || x >= width || y >= length)
//    {
//        // Debug.LogError("Tile index out of bounds");
//        return null;
//    }
//    return tiles[x, y, z];
//}

//public Tile[] GetTilesInRectangle(float xbeg, float ybeg, float xend, float yend)
//{
//    int xlen = Mathf.CeilToInt(Math.Max(xbeg, xend)) - Mathf.FloorToInt(Math.Min(xbeg, xend));
//    int ylen = Mathf.CeilToInt(Math.Max(ybeg, yend)) - Mathf.FloorToInt(Math.Min(ybeg, yend));
//    int xmin = Math.Min(Mathf.FloorToInt(xbeg), Mathf.FloorToInt(xend));
//    int ymin = Math.Min(Mathf.FloorToInt(ybeg), Mathf.FloorToInt(yend));
//    Tile[] tilesToGet = new Tile[xlen*ylen];
//    for (int x = 0; x < xlen; x +=1)
//    {
//        for (int y = 0; y < ylen; y += 1)
//        {
//            tilesToGet[x * ylen + y] = tiles[x + xmin, y + ymin, 0];
//        }
//    }
//    return tilesToGet;
//}


//void MakePositionedEntity(string entity_id, int x, int y, int z)
//{
//    entity_positions.SetTilePosition(entity_id, new Position(entity_id, x, y, z));
//}