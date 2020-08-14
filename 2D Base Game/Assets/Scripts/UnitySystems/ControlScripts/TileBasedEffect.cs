using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileBasedEffect
{
    /* What the point of this is:
     * This is made to describe anything that should activate when the user is clicking on tiles
     * 
     * What it should have:
     * Some function that it will call when asked to by the mouse
     * Possibly parameters that effect this function's actions
     * 
     * 
     * 
     */

    //void ActivateOnTiles(Tile[] tiles);

    void ChangeArgument(string name, string value);
}

