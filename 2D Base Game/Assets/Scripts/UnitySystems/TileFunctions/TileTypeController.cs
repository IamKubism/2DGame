using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileControl : ITileBasedEffect
{
    string tile_type_id;

    public TileControl()
    {
        tile_type_id = "stone_rough";
    }

    //public void ActivateOnTiles(Tile[] tiles)
    //{
    //    foreach (Tile t in tiles)
    //    {
    //        //t.curr_type = MainGame.instance.tile_types_map.GetPrototypeVal(tile_type_id);
    //    }
    //}

    public void ChangeArgument(string name, string value)
    {
        tile_type_id = value;
    }
}
