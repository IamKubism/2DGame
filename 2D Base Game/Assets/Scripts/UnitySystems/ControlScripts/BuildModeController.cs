using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// This class is designed to control user activated functions on tiles
/// </summary>
public class BuildModeController : MonoBehaviour
{
    /// <summary>
    /// Depreciating
    /// </summary>
    //public TileType buildmodetile { get; protected set; }

    //public UserRelevantData userData;

    /// <summary>
    /// Depreciating
    /// </summary>
    //string buildModeObjectType;

    /// <summary>
    /// Depreciating
    /// </summary>
    bool buildModeIsObjects = false;

    /// <summary>
    /// Class used to initialize function data that will be called in game
    /// </summary>
    //Dictionary<string,string> activeData;

    //public Tile[] activeTiles;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// For in game use, creates a job for characters to carry out
    /// </summary>
    /// <param name="t"></param>
    /// <param name="j"></param>
    //public void MakeJobBuild(Tile t, Job j) 
    //{
    //    if (t == null)
    //    {
    //        Debug.LogError("Tried to make a job on a null tile");
    //        return;
    //    }
    //    if (j.JobReqs(t) && t.pending_build_job == null)
    //    {

    //    }
    //}

    /// <summary>
    /// Do the action that a job does, but instantly
    /// </summary>
    /// <param name="t"></param>
    /// <param name="j"></param>
    //public void MakeInstantJobBuild(Tile t, Job j) 
    //{
    //    if (t == null)
    //    {
    //        Debug.LogError("Tried to make a job on a null tile");
    //        return;
    //    }
    //    if (j.JobReqs(t) && t.pending_build_job == null)
    //    {

    //    }
    //}

    /// <summary>
    /// //For Debugging purposes, do some action on a tile
    /// </summary>
    /// <param name="t"></param>
    /// <param name="a"></param>
    //public void DoInstantFunctionOnTile(Tile t, Action<Tile> a) 
    //{
    //    if (t == null)
    //    {
    //        Debug.LogError("Tried to preform an action on a null tile");
    //        return;
    //    }
    //    a(t);
    //}

    //public void SetActiveDataValue(string id, string value)
    //{
    //    if (activeData.ContainsKey(id))
    //        activeData[id] = value;
    //    else
    //    {
    //        Debug.LogError("Could not find " + id + " in active data dictionary.");
    //    }
    //}

    //public void ActivateOnTiles(Tile[] tiles)
    //{
    //    throw new NotImplementedException();
    //}
}





/// <summary>
/// Depreciating
/// </summary>
//public void SetMode_BuildFloor()
//{
//    buildModeIsObjects = false;
//    buildmodetile = MainGame.instance.GetTileType("Stone_Rough");
//}

/// <summary>
/// Depreciating
///// </summary>
//public void SetMode_Buldoze()
//{
//    buildModeIsObjects = false;
//    buildmodetile = MainGame.instance.GetTileType("Empty");
//}

/// <summary>
/// Depreciating
///// </summary>
//public void SetMode_None()
//{
//    buildModeIsObjects = false;
//    //buildmodetile = null;
//}

/// <summary>
/// Depreciating
/// </summary>
/// <param name="id"></param>
//public void SetFloorMode(string id)
//{
//    buildModeIsObjects = false;
//    buildmodetile = MainGame.instance.GetTileType(id);
//}

/// <summary>
/// Depreciating
/// </summary>
/// <param name="objtype"></param>
//public void SetMode_BuildInstalledObject(string objtype)
//{
//    buildModeIsObjects = true;
//    buildModeObjectType = objtype;
//}

/// <summary>
/// In the process of depreciating
/// </summary>
/// <param name="t"></param>
//public void DoBuildFurniture(Tile t)
//{
//    if (t != null)
//    {
//        if (buildModeIsObjects == true)
//        {
//            string furnitureType = buildModeObjectType;

//            if (WorldController.Instance.world.IsFurniturePlacementValid(furnitureType, t) && t.pending_build_job == null)
//            {
//                Job j = new Job(t, 
//                    (theJob) =>
//                    {
//                        WorldController.Instance.world.PlaceFurniture(furnitureType, theJob.dest_tile);
//                    }, furnitureType, 1f, true);

//                t.pending_build_job = j;

//                WorldController.Instance.world.job_queue.Enqueue(j, 1); //TODO The 1 is a placeholder, in the future we should change it with the priority system

//            }
//        }
//        else
//        {
//            t.curr_type = buildmodetile;
//        }
//    }
//}