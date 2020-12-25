using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Psingine
{
    public class Path_TileGraph
    {
        public static Path_TileGraph movement_graph;
        public Dictionary<Entity, Path_Node<Entity>> tile_map;

        public NodeChunk map;

        public Path_TileGraph(NodeChunk map)
        {
            if(movement_graph == null)
            {
                movement_graph = this;
            }
            this.map = map;
            tile_map = new Dictionary<Entity, Path_Node<Entity>>();
            foreach (Entity e in map.entities.Values)
            {
                tile_map.Add(e, new Path_Node<Entity>(e));
            }

            //Set Graph Edges
            foreach (Path_Node<Entity> node in tile_map.Values)
            {
                //Set non-diagonals
                foreach (Entity n in map.GetNeighborTiles(node.data, 1))
                {
                    node.MakeNewEdge(tile_map[n], 1);
                }

                //Set diagonals
                foreach (Entity n in map.GetNeighborTiles(node.data, 2))
                {
                    node.MakeNewEdge(tile_map[n], 1.41f);
                }
            }
        }
    }
}
