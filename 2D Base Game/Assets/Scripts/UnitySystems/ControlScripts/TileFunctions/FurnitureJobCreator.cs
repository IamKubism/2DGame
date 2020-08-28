using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobCreator : ITileBasedEffect
{
    //JobPrototype j;

    public JobCreator()
    {
        //j = MainGame.instance.GetJobPrototype("build_furniture");
        //j.SetComponentString("furniture_type", "wall_wood");
    }

    //public void ActivateOnTiles(Tile[] tiles)
    //{
    //    foreach (Tile t in tiles)
    //    {
    //        j.CloneJob(t);
    //    }
    //}

    public void ChangeArgument(string name, string value)
    {
        if(name == "jobType")
        {
        }
        else
        {
        }
    }
}
