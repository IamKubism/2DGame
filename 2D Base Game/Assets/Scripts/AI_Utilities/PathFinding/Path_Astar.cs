using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Priority_Queue;
using System;
using HighKings;

namespace HighKings
{
    public class Path_Astar
    {
        Entity curr_pos;
        Entity end_pos;
        Queue<Entity> path;
        Queue<Position> position_path;
        Queue<Path_Edge<Entity>> edge_path;
        public Path_TileGraph graph;

        IBehavior cost_function;

        Func<Entity, float> full_func;
        /// <summary>
        /// Computes the optimum path from one tile to another, using the Astar algorithm (a variant of Djkstra's Algorithm)
        /// See: https://en.wikipedia.org/wiki/A*_search_algorithm
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="map"></param>
        public Path_Astar(Path_TileGraph graph, Entity start, List<Entity> end, IBehavior behavior)
        {
            this.graph = graph;
            cost_function = behavior;
            full_func = (ent) => { return cost_function.CalculateOnEntity(ent); };
            if (graph.tile_map.ContainsKey(start) == false)
            {
                Debug.LogError("Path_Astar -- Start Tile has no node");
                return;
            }
            if (graph.tile_map.ContainsKey(end[0]) == false)
            {
                Debug.LogError("Path_Astar -- End Tile Has no node");
                return;
            }

            end_pos = end[0];

            Dictionary<Entity, Path_Node<Entity>> node_map = graph.tile_map;

            List<Path_Node<Entity>> closed_set = new List<Path_Node<Entity>>();

            SimplePriorityQueue<Path_Node<Entity>> open_set = new SimplePriorityQueue<Path_Node<Entity>>();

            Dictionary<Path_Node<Entity>, float> g_score = new Dictionary<Path_Node<Entity>, float>();

            Dictionary<Path_Node<Entity>, Path_Node<Entity>> came_from = new Dictionary<Path_Node<Entity>, Path_Node<Entity>>();

            foreach (Path_Node<Entity> n in node_map.Values)
            {
                g_score[n] = Mathf.Infinity;
            }
            g_score[node_map[start]] = 0;

            Dictionary<Path_Node<Entity>, float> f_score = new Dictionary<Path_Node<Entity>, float>();

            foreach (Path_Node<Entity> n in node_map.Values)
            {
                f_score[n] = Mathf.Infinity;
            }
            f_score[node_map[start]] = Heuristic(node_map[start].data, end[0]);

            open_set.Enqueue(node_map[start], f_score[node_map[start]]);

            while (open_set.Count > 0)
            {
                Path_Node<Entity> current = open_set.Dequeue();

                if (end.Contains(current.data))
                {
                    ReconstructPath(came_from, current);
                    return;
                }
                closed_set.Add(current);
                foreach (Path_Edge<Entity> e in current.edges)
                {
                    if (closed_set.Contains(e.node))
                    {
                        continue;
                    }
                    if (e.modifier * FullCostFunction(e.node.data) <= 0)
                    {
                        closed_set.Add(e.node);
                        continue;
                    }

                    //TODO: Change e.cost to Behavior(e)
                    float tentative_g = g_score[current] + e.modifier * cost_function.CalculateOnEntity(e.node.data);

                    if (open_set.Contains(e.node) == false)
                    {
                        open_set.Enqueue(e.node, tentative_g + Heuristic(e.node.data, end[0]));
                    }
                    else if (tentative_g >= g_score[e.node])
                    {
                        continue;
                    }

                    came_from[e.node] = current;
                    g_score[e.node] = tentative_g;
                    f_score[e.node] = tentative_g + Heuristic(e.node.data, end[0]);
                    //Debug.Log(f_score[e.node]);
                }
            }
        }

        public Path_Astar(Path_TileGraph graph, Entity mover, Entity start, List<Entity> end)
        {
            this.graph = graph;
            Func<Entity, Entity, float> computer = (e1, e2) =>
            {
                Event cost = EventManager.instance.DoEvent(e2, "SetTileData");
                cost = new Event(cost, "TileCost");
                cost.AddUpdates(e1);
                cost.Invoke(e1);
                return cost.GetParamValue<float>("move_cost");
            };
            if (graph.tile_map.ContainsKey(start) == false)
            {
                Debug.LogError("Path_Astar -- Start Tile has no node");
                return;
            }
            if (graph.tile_map.ContainsKey(end[0]) == false)
            {
                Debug.LogError("Path_Astar -- End Tile Has no node");
                return;
            }

            end_pos = end[0];

            Dictionary<Entity, Path_Node<Entity>> node_map = graph.tile_map;

            List<Path_Node<Entity>> closed_set = new List<Path_Node<Entity>>();

            SimplePriorityQueue<Path_Node<Entity>> open_set = new SimplePriorityQueue<Path_Node<Entity>>();

            Dictionary<Path_Node<Entity>, float> g_score = new Dictionary<Path_Node<Entity>, float>();

            Dictionary<Path_Node<Entity>, Path_Node<Entity>> came_from = new Dictionary<Path_Node<Entity>, Path_Node<Entity>>();

            foreach (Path_Node<Entity> n in node_map.Values)
            {
                g_score[n] = Mathf.Infinity;
            }
            g_score[node_map[start]] = 0;

            Dictionary<Path_Node<Entity>, float> f_score = new Dictionary<Path_Node<Entity>, float>();

            foreach (Path_Node<Entity> n in node_map.Values)
            {
                f_score[n] = Mathf.Infinity;
            }
            f_score[node_map[start]] = Heuristic(node_map[start].data, end[0]);

            open_set.Enqueue(node_map[start], f_score[node_map[start]]);

            while (open_set.Count > 0)
            {
                Path_Node<Entity> current = open_set.Dequeue();

                if (end.Contains(current.data))
                {
                    ReconstructPath(came_from, current);
                    return;
                }
                closed_set.Add(current);
                foreach (Path_Edge<Entity> e in current.edges)
                {
                    if (closed_set.Contains(e.node))
                    {
                        continue;
                    }
                    if (e.modifier * computer(mover, e.node.data) <= 0)
                    {
                        closed_set.Add(e.node);
                        continue;
                    }

                    float tentative_g = g_score[current] + e.modifier * computer(mover, e.node.data);

                    if (open_set.Contains(e.node) == false)
                    {
                        open_set.Enqueue(e.node, tentative_g + Heuristic(e.node.data, end[0]));
                    }
                    else if (tentative_g >= g_score[e.node])
                    {
                        continue;
                    }

                    came_from[e.node] = current;
                    g_score[e.node] = tentative_g;
                    f_score[e.node] = tentative_g + Heuristic(e.node.data, end[0]);
                    //Debug.Log(f_score[e.node]);
                }
            }
        }

        float FullCostFunction(Entity curr)
        {
            return cost_function.CalculateOnEntity(curr) + Heuristic(curr, end_pos);
        }

        float Heuristic(Entity curr, Entity end)
        {
            return Position.SqrDist(curr.GetComponent<Position>("Position"), end.GetComponent<Position>("Position"));
        }

        void ReconstructPath(Dictionary<Path_Node<Entity>, Path_Node<Entity>> came_from, Path_Node<Entity> current)
        {
            Queue<Entity> rev_path = new Queue<Entity>();
            edge_path = new Queue<Path_Edge<Entity>>();
            end_pos = current.data;

            while (came_from.ContainsKey(current))
            {
                rev_path.Enqueue(current.data);
                edge_path.Enqueue(came_from[current].FindEdge(current));
                current = came_from[current];
            }
            rev_path.Enqueue(current.data);

            path = new Queue<Entity>(rev_path.Reverse());
            edge_path = new Queue<Path_Edge<Entity>>(edge_path.Reverse());
            
            path.Dequeue();
        }

        public float ComputeCurrentCost(Entity e, IBehavior b)
        {
            return b.CalculateOnEntity(e);
        }

        public Tuple<Position, float> DeQueue(IBehavior b)
        {
            Entity e = path.Dequeue();
            return new Tuple<Position, float>(e.GetComponent<Position>("Position"), b.CalculateOnEntity(e));
        }

        public Entity DeQueue()
        {
            return path.Dequeue();
        }

        public int Length()
        {
            if (path == null)
            {
                return 0;
            }
            return path.Count;
        }

        public void ClearPath()
        {
            if (path != null)
            {
                path.Clear();
                //            Debug.Log("Cleared Path");
            }
        }

        public override string ToString()
        {
            string s = "";
            uint i = 0;
            foreach (Entity e in path.ToArray())
            {
                s += $"Node {i}: {e.ToString()}\n";
                i += 1;
            }
            return s;
        }
    }

    
}

///// <summary>
///// Takes the camefrom dictionary and creates a queue of all the came from tiles that will be used for pathfinding
///// </summary>
///// <param name="cameFrom"></param>
///// <param name="current"></param>
//void ReconstructPath(Dictionary<Path_Node<Tile>,Path_Node<Tile>> cameFrom, Path_Node<Tile> current)
//{
//    Queue<Tile> genpath = new Queue<Tile>();
//    Queue<float> genpath_cost = new Queue<float>();

//    while (cameFrom.ContainsKey(current))
//    {
//        genpath_cost.Enqueue(cameFrom[current].FindEdge(current).cost);
//        genpath.Enqueue(current.data);
//        current = cameFrom[current];
//        //Debug.Log("Queued " + current.data.X + " " + current.data.Y + " for cost " + genpath_cost.Peek());
//    }
//    genpath.Enqueue(current.data);

//    //path =  new Queue<Tile>(genpath.Reverse());
//    path.Dequeue(); //Since the starting tile was always added to camefrom list (and will be first in this queue)

//    movement_cost_base = new Queue<float>(genpath_cost.Reverse());
//}

//    public Tile DeQueue()
//    {
//        if (Length() == 0)
//        {
//            return null; 
//        }
////        Debug.Log(path.Count);
//        return path.Dequeue();
//    }

//    public Tile checkNext()
//    {
//        if (Length() == 0)
//        {
//            return null;
//        }
//        return path.Peek();
//    }

//public Queue<Tuple<TilePosition,float>> GenMoverDataQueue(string entity_id)
//{
//    Queue<Tuple<TilePosition, float>> path_queue = new Queue<Tuple<TilePosition, float>>();
//    while (path.Count > 0)
//    {
//        //Debug.Log("Path count: " + path.Count);
//        //Debug.Log("Cost count: " + movement_cost_base.Count);
//        Tile t = path.Dequeue();
//        path_queue.Enqueue(new Tuple<TilePosition, float>(new TilePosition(entity_id, t.x, t.y, t.z), movement_cost_base.Dequeue()));
//    }

//    return path_queue;
//}

////Heuristic function, change to optimize
//float h(Path_Node<Tile> n, Tile end)
//{
//    return Mathf.Abs(n.data.x - end.x) + Mathf.Abs(n.data.y - end.y) + Mathf.Abs(n.data.z - end.z);
//}

//public Path_Astar(Path_TileGraph graph, Tile start, Tile end, bool roomfill = false)
//{
//    if (graph.tilemap.ContainsKey(start) == false)
//    {
//        Debug.LogError("Path_Astar -- Start Tile has no node");
//        return;
//    }
//    if (graph.tilemap.ContainsKey(end) == false)
//    {
//        Debug.LogError("Path_Astar -- End Tile Has no node");
//        return;
//    }

//    Dictionary<Tile, Path_Node<Tile>> tilemap = graph.tilemap;

//    List<Path_Node<Tile>> closedSet = new List<Path_Node<Tile>>();

//    SimplePriorityQueue<Path_Node<Tile>> openSet = new SimplePriorityQueue<Path_Node<Tile>>();

//    Dictionary<Path_Node<Tile>, Path_Node<Tile>> came_from = new Dictionary<Path_Node<Tile>, Path_Node<Tile>>();

//    Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float>();

//    foreach (Path_Node<Tile> n in tilemap.Values)
//    {
//        g_score[n] = Mathf.Infinity;
//    }
//    g_score[tilemap[start]] = 0;

//    Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float>();
//    foreach (Path_Node<Tile> n in tilemap.Values)
//    {
//        f_score[n] = Mathf.Infinity;
//    }
//    f_score[tilemap[start]] = h(tilemap[start], end);

//    openSet.Enqueue(tilemap[start], f_score[tilemap[start]]);

//    while (openSet.Count > 0)
//    {
//        Path_Node<Tile> current = openSet.Dequeue();

//        if (current == tilemap[end])
//        {
//            ReconstructPath(came_from, current);
//            return;
//        }

//        if (roomfill == false)
//        {
//            closedSet.Add(current);
//            foreach (Path_Edge<Tile> e in current.edges)
//            {
//                if (closedSet.Contains(e.node))
//                {
//                    continue;
//                }
//                if (e.cost == 0)
//                {
//                    closedSet.Add(e.node);
//                    continue;
//                }

//                float tentative_g = g_score[current] + e.cost;

//                if (openSet.Contains(e.node) == false)
//                {
//                    openSet.Enqueue(e.node, tentative_g + h(e.node, end));
//                }
//                else if (tentative_g >= g_score[e.node])
//                {
//                    continue;
//                }

//                came_from[e.node] = current;
//                g_score[e.node] = tentative_g;
//                f_score[e.node] = tentative_g + h(e.node, end);
//                //Debug.Log(f_score[e.node]);
//            }
//        }
//        else
//        {
//            closedSet.Add(current);
//            foreach (Path_Edge<Tile> e in current.edges)
//            {
//                if (closedSet.Contains(e.node))
//                {
//                    continue;
//                }
//                if (e.node.data.is_room_dividing)
//                {
//                    closedSet.Add(e.node);
//                    continue;
//                }

//                float tentative_g = g_score[current] + e.cost;

//                if (openSet.Contains(e.node) == false)
//                {
//                    openSet.Enqueue(e.node, tentative_g + h(e.node, end));
//                }
//                else if (tentative_g >= g_score[e.node])
//                {
//                    continue;
//                }

//                came_from[e.node] = current;
//                g_score[e.node] = tentative_g;
//                f_score[e.node] = tentative_g + h(e.node, end);
//            }
//        }
//    }
//}


///// <summary>
///// Computes the optimum path from one tile to another, using the Astar algorithm (a variant of Djkstra's Algorithm)
///// See: https://en.wikipedia.org/wiki/A*_search_algorithm
///// </summary>
///// <param name="graph"></param>
///// <param name="start"></param>
///// <param name="end"></param>
///// <param name="map"></param>
//public Path_Astar(Path_TileGraph graph, string start, List<string> end, IBehavior behavior)
//{
//    if (graph.node_map.ContainsKey(start) == false)
//    {
//        Debug.LogError("Path_Astar -- Start Tile has no node");
//        return;
//    }
//    if (graph.node_map.ContainsKey(end[0]) == false)
//    {
//        Debug.LogError("Path_Astar -- End Tile Has no node");
//        return;
//    }

//    Dictionary<string, Path_Node<string>> node_map = graph.node_map;

//    List<Path_Node<string>> closed_set = new List<Path_Node<string>>();

//    SimplePriorityQueue<Path_Node<string>> open_set = new SimplePriorityQueue<Path_Node<string>>();

//    Dictionary<Path_Node<string>, float> g_score = new Dictionary<Path_Node<string>, float>();

//    Dictionary<Path_Node<string>, Path_Node<string>> came_from = new Dictionary<Path_Node<string>, Path_Node<string>>();

//    foreach (Path_Node<string> n in node_map.Values)
//    {
//        g_score[n] = Mathf.Infinity;
//    }
//    g_score[node_map[start]] = 0;

//    Dictionary<Path_Node<string>, float> f_score = new Dictionary<Path_Node<string>, float>();

//    foreach (Path_Node<string> n in node_map.Values)
//    {
//        f_score[n] = Mathf.Infinity;
//    }
//    f_score[node_map[start]] = h(node_map[start], end[0], graph.map);

//    open_set.Enqueue(node_map[start], f_score[node_map[start]]);

//    while (open_set.Count > 0)
//    {
//        Path_Node<string> current = open_set.Dequeue();

//        if (end.Contains(current.data))
//        {
//            ReconstructPath(came_from, current);
//            return;

//        }
//        closed_set.Add(current);
//        foreach (Path_Edge<string> e in current.edges)
//        {
//            if (closed_set.Contains(e.node))
//            {
//                continue;
//            }
//            if (e.cost <= 0)
//            {
//                closed_set.Add(e.node);
//                continue;
//            }

//            //TODO: Change e.cost to Behavior(e)
//            float tentative_g = g_score[current] + e.cost;

//            if (open_set.Contains(e.node) == false)
//            {
//                open_set.Enqueue(e.node, tentative_g + h(e.node, end[0], graph.map));
//            }
//            else if (tentative_g >= g_score[e.node])
//            {
//                continue;
//            }

//            came_from[e.node] = current;
//            g_score[e.node] = tentative_g;
//            f_score[e.node] = tentative_g + h(e.node, end[0], graph.map);
//            //Debug.Log(f_score[e.node]);
//        }
//    }
//}
