using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    /// <summary>
    /// Currently I am just trying to get things working for a 1 chunck world. This will store all of the occupancy type data for map tiles and mangage them
    /// </summary>
    public class MapCells : ISystemAdder
    {
        public Dictionary<Position.Tile, Entity> tiles;
        public Dictionary<Position.Tile, Cell> cells;
        public static MapCells instance;

        public MapCells()
        {
            if(instance == null)
            {
                instance = this;
            }
            tiles = new Dictionary<Position.Tile, Entity>();
            cells = new Dictionary<Position.Tile, Cell>();
            PrototypeLoader.instance.AddSystemLoc("map_cells", this);
        }

        public void AddEntities(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                Position pos = e.GetComponent<Position>("Position");
                if (tiles.ContainsKey(pos.tile))
                {
                    Debug.LogError($"Tried to add a cell twice at {pos.ToString()}");
                    continue;
                }
                tiles.Add(pos.tile, e);
                cells.Add(pos.tile, e.GetComponent<Cell>("Cell"));
            }
            OnAddedEntities(entities);
        }

        public void OnAddedEntities(List<Entity> entities)
        {

        }

        public List<Entity> GetCellArea(Entity center, int sqr_dist)
        {
            List<Entity> to_return = new List<Entity>(sqr_dist);
            int[] p_cen = center.GetComponent<Position>("Position").p;
            for (int x = p_cen[0]-sqr_dist; x < p_cen[0]+sqr_dist; x+=1)
            {
                for(int y = p_cen[1]-sqr_dist; y < p_cen[1]-sqr_dist; y += 1)
                {
                    if(tiles.ContainsKey(new Position.Tile { x = x, y = y, z = 0 }))
                    {
                        int[] p = new int[3] { x, y, 0 };
                        if (MathFunctions.SqrDist(p_cen, p) <= sqr_dist && MathFunctions.SqrDist(p_cen, p) > 0)
                        {
                            to_return.Add(tiles[new Position.Tile { x = x, y = y, z = 0 }]);
                        }
                    }
                }
            }
            return to_return;
        }

        public List<Entity> GetCellArea(Position[] positions)
        {
            List<Entity> cell_area = new List<Entity>(positions.Length);
            foreach(Position p in positions)
            {
                cell_area.Add(tiles[p.tile]);
            }

            return cell_area;
        }

        public Entity GetTileFromCoords(int x, int y, int z)
        {
            Position.Tile t = new Position.Tile { x = x, y = y, z = z };
            return tiles.ContainsKey(t) ? tiles[t] : null;
        }

        public Entity GetTileUnderEntity(Entity e)
        {
            return tiles[e.GetComponent<Position>("Position").tile];
        }

        /// <summary>
        /// For creation of entities with positions, to reset cell position instead use "SwapOccupants"
        /// </summary>
        /// <param name="entities"></param>
        public void AddOccupants(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                cells[e.GetComponent<Position>("Position").tile].AddOccupant(e);
            }
        }

        public void SwapOccupants(Dictionary<Entity,Position> entity_next_pos)
        {
            foreach(KeyValuePair<Entity,Position> ep in entity_next_pos)
            {
                int[] pos = ep.Value.p;
                //Cell c = cells[pos[0], pos[1], pos[2]];
                //c.AddOccupant(ep.Key);
                pos = ep.Key.GetComponent<Position>("Position").p;
                //c = cells[pos[0], pos[1], pos[2]];
                //c.RemoveOccupant(ep.Key);
            }
        }

    }
}
