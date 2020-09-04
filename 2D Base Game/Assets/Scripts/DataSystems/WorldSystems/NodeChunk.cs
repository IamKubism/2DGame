using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class NodeChunk
    {
        /// <summary>
        /// This is really just a coordination of the entities dictionary
        /// </summary>
        public Entity[,,] tiles { get; protected set; }
        public List<Entity>[,,] cells { get; protected set; }
        public Tuple<int, int> chunk_id;

        /// <summary>
        /// Stores all Tile entities in this chunk
        /// </summary>
        public Dictionary<string, Entity> entities { get; protected set; }

        public int len_x { get; protected set; }
        public int len_y { get; protected set; }
        public int len_z { get; protected set; }

        public NodeChunk(int len_x, int len_y, int len_z, int id_x = 0, int id_y = 0)
        {
            chunk_id = new Tuple<int, int>(id_x, id_y);
            tiles = new Entity[len_x, len_y, len_z];
            cells = new List<Entity>[len_x,len_y,len_z];
            this.len_x = len_x;
            this.len_y = len_y;
            this.len_z = len_z;
            for (int x = 0; x < len_x; x += 1)
            {
                for (int y = 0; y < len_y; y += 1)
                {
                    for (int z = 0; z < len_z; z += 1)
                    {
                        cells[x, y, z] = new List<Entity>(4);
                    }
                }
            }
        }

        public void CreateTiles()
        {
            string[] ids = new string[len_x * len_y * len_z];
            Dictionary<Entity, Dictionary<string, object[]>> args = new Dictionary<Entity, Dictionary<string, object[]>>();
            for (int x = 0; x < len_x; x += 1)
            {
                for (int y = 0; y < len_y; y += 1)
                {
                    for (int z = 0; z < len_z; z += 1)
                    {
                        ids[x + y * len_x + z * len_x * len_y] = $"tile_{x}_{y}_{z}";
                    }
                }
            }
            entities = EntityManager.instance.CreateEntities("tile", ids);
            for (int x = 0; x < len_x; x += 1)
            {
                for (int y = 0; y < len_y; y += 1)
                {
                    for (int z = 0; z < len_z; z += 1)
                    {
                        args.Add(entities[$"tile_{x}_{y}_{z}"], new Dictionary<string, object[]> { { "Position", new object[3] { x, y, z } }, { "Cell", new object[1] { entities[$"tile_{x}_{y}_{z}"] } } });
                        tiles[x, y, z] = entities[$"tile_{x}_{y}_{z}"];
                    }
                }
            }
            PrototypeLoader.instance.AttachPrototype("tile_stone_rough", args);
        }

        public List<Entity> GetNeighborTiles(Entity center, int sqr_dist)
        {
            List<Entity> to_return = new List<Entity>(sqr_dist);
            int[] p_cen = center.GetComponent<Position>("Position").p;
            int[] t = new int[3];
            t[2] = 0; //TODO: z level shit
            for (int x = Math.Max(p_cen[0] - sqr_dist, 0); x < Math.Min(p_cen[0] + sqr_dist, len_x - 1); x += 1)
            {
                for (int y = Math.Max(p_cen[1] - sqr_dist, 0); y < Math.Min(p_cen[1] + sqr_dist, len_y - 1); y += 1)
                {
                    t[0] = x;
                    t[1] = y;
                    if (MathFunctions.SqrDist(p_cen, t) <= sqr_dist && MathFunctions.SqrDist(t, p_cen) > 0)
                    {
                        to_return.Add(tiles[x,y,0]);
                    }
                }
            }
            return to_return;
        }

        public List<Entity> GetTileArea(Position[] positions)
        {
            List<Entity> to_return = new List<Entity>();
            foreach(Position p in positions)
            {
                Entity e = tiles[p.x, p.y, p.z];
                if (to_return.Contains(e))
                {
                    continue;
                }
                to_return.Add(e);
            }
            return to_return;
        }
    }
}
