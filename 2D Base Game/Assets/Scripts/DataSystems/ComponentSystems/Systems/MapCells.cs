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
        private readonly int len_x;
        private readonly int len_y;
        private readonly int len_z;
        public Entity[,,] entities_cells;
        public Cell[,,] cells;

        public MapCells(int len_x, int len_y, int len_z)
        {
            this.len_x = len_x;
            this.len_y = len_y;
            this.len_z = len_z;
            entities_cells = new Entity[len_x, len_y, len_z];
            cells = new Cell[len_x, len_y, len_z];
            PrototypeLoader.instance.AddSystemLoc("map_cells", this);
        }

        public void AddEntities(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                Position pos = e.GetComponent<Position>("Position");
                int[] index = pos.p;
                if(entities_cells[index[0],index[1],index[2]] != null)
                {
                    Debug.LogError($"Tried to add a cell twice at {index[0]}, {index[1]}, {index[2]}");
                    continue;
                }
                entities_cells[index[0], index[1], index[2]] = e;
                cells[index[0], index[1], index[2]] = e.GetComponent<Cell>("Celled");
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
            for (int x = Math.Max(p_cen[0] - sqr_dist, 0); x < Math.Min(p_cen[0] + sqr_dist, len_x - 1); x += 1)
            {
                for (int y = Math.Max(p_cen[1] - sqr_dist, 0); y < Math.Min(p_cen[1] + sqr_dist, len_y - 1); y += 1)
                {
                    int[] p = new int[3] { x, y, 0 };
                    if (MathFunctions.SqrDist(p_cen, p) <= sqr_dist && MathFunctions.SqrDist(p_cen, p) > 0)
                    {
                        to_return.Add(entities_cells[x, y, p_cen[2]]);
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
                int[] ind = p.p;
                cell_area.Add(entities_cells[ind[0], ind[1], ind[2]]);
            }

            return cell_area;
        }

        /// <summary>
        /// For creation of entities with positions, to reset cell position instead use "SwapOccupants"
        /// </summary>
        /// <param name="entities"></param>
        public void AddOccupants(List<Entity> entities)
        {
            foreach(Entity e in entities)
            {
                int[] pos = e.GetComponent<Position>("Position").p;
                Cell c = cells[pos[0], pos[1], pos[2]];
                c.AddOccupant(e);
            }
        }

        public void SwapOccupants(Dictionary<Entity,Position> entity_next_pos)
        {
            foreach(KeyValuePair<Entity,Position> ep in entity_next_pos)
            {
                int[] pos = ep.Value.p;
                Cell c = cells[pos[0], pos[1], pos[2]];
                c.AddOccupant(ep.Key);
                pos = ep.Key.GetComponent<Position>("Position").p;
                c = cells[pos[0], pos[1], pos[2]];
                c.RemoveOccupant(ep.Key);
            }
        }
    }
}
