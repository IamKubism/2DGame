﻿using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Psingine
{
    /// <summary>
    /// Similar to djikstra's algorithm, computes a most prefered path for a directed acyclic path (used for goal setting)
    /// </summary>
    public class DjkistraGoal
    {
        IGoal end;
        Queue<IGoal> path;
        Queue<Path_Edge<IGoal>> edge_path;
        Dictionary<Path_Node<IGoal>, Entity> target_map;

        /// <summary>
        /// Computes the optimum goal path on for a source entity when dealing with a target (Similar to Djikstra's Algorithm but there is no end given at the beginning since we know the ai graph is acyclic but don't know what the entity wants to do
        /// at the end)
        /// See: https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
        /// </summary>
        /// <param name="map"></param>
        /// <param name="start"></param>
        /// <param name="source"></param>
        /// <param name="inital_target"></param>
        public DjkistraGoal(Dictionary<IGoal,Path_Node<IGoal>> map, IGoal start, Entity source, Entity inital_target)
        {
            //if (map.ContainsKey(start) == false)
            //{
            //    Debug.LogError("Goal Map has no start");
            //    return;
            //}

            //Entity target = inital_target;
            //Dictionary<IGoal, Path_Node<IGoal>> node_map = map;
            //target_map = new Dictionary<Path_Node<IGoal>, Entity> { { node_map[start], inital_target } };
            //List<Path_Node<IGoal>> closed_set = new List<Path_Node<IGoal>>();
            //SimplePriorityQueue<Path_Node<IGoal>, int> open_set = new SimplePriorityQueue<Path_Node<IGoal>, int>();
            //Dictionary<Path_Node<IGoal>, int> score = new Dictionary<Path_Node<IGoal>, int>();
            //Dictionary<Path_Node<IGoal>, Path_Node<IGoal>> came_from = new Dictionary<Path_Node<IGoal>, Path_Node<IGoal>>();

            //foreach (Path_Node<IGoal> n in node_map.Values)
            //{
            //    score[n] = (1 << 30);
            //}

            //score[node_map[start]] = 0;
            //open_set.Enqueue(node_map[start], score[node_map[start]]);

            //while (open_set.Count > 0)
            //{
            //    Path_Node<IGoal> current = open_set.Dequeue();
            //    target = current.data.PassTarget(source, target_map[current]);

            //    if (current.edges.Count == 0)
            //    {
            //        ReconstructPath(came_from, current);
            //        return;
            //    }

            //    closed_set.Add(current);
            //    foreach (Path_Edge<IGoal> e in current.edges)
            //    {
            //        if (closed_set.Contains(e.node))
            //        {
            //            continue;
            //        }
            //        if (Mathf.CeilToInt(e.modifier) * e.node.data.Adversion(source, inital_target) <= 0 || e.node.data.Achieved(source,target))
            //        {
            //            continue;
            //        }

            //        int tentative_g = score[current] + Mathf.CeilToInt(e.modifier) * e.node.data.Adversion(source, target, current.data);

            //        if (open_set.Contains(e.node) == false)
            //        {
            //            open_set.Enqueue(e.node, tentative_g);
            //            target_map.Add(e.node, target);
            //        }
            //        else if (tentative_g >= score[e.node])
            //        {
            //            continue;
            //        }

            //        came_from[e.node] = current;
            //        target_map[e.node] = target;
            //        score[e.node] = tentative_g;
            //    }
            //}
            //path = new Queue<IGoal>();
            //Debug.LogWarning("Goal AI could not find a goal path");
        }

        /// <summary>
        /// For a finding a goal path from a start goal to an end goal
        /// </summary>
        /// <param name="map"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="source"></param>
        /// <param name="inital_target"></param>
        public DjkistraGoal(Dictionary<IGoal, Path_Node<IGoal>> map, IGoal start, IGoal end, Entity source, Entity inital_target)
        {
            //if (map.ContainsKey(start) == false)
            //{
            //    Debug.LogError("Goal Map has no start");
            //    return;
            //}

            //if(map.ContainsKey(end) == false)
            //{
            //    Debug.LogError("Goal Map has no end");
            //    return;
            //}

            //float Compute(Event e, Entity next_node)
            //{
            //    e.AddUpdate((e1) => { e1.SetParamValue("next_node", next_node, (n1, n2) => { return n2; }); }, 0);
            //    e.AddUpdates(next_node);
            //    e.Invoke(source);
            //    return e.GetParamValue<float>("Adversion");
            //}

            //Entity target = inital_target;
            //Dictionary<IGoal, Path_Node<IGoal>> node_map = map;
            //target_map = new Dictionary<Path_Node<IGoal>, Entity> { { node_map[start], inital_target } };
            //List<Path_Node<IGoal>> closed_set = new List<Path_Node<IGoal>>();
            //SimplePriorityQueue<Path_Node<IGoal>, int> open_set = new SimplePriorityQueue<Path_Node<IGoal>, int>();
            //Dictionary<Path_Node<IGoal>, int> score = new Dictionary<Path_Node<IGoal>, int>();
            //Dictionary<Path_Node<IGoal>, Path_Node<IGoal>> came_from = new Dictionary<Path_Node<IGoal>, Path_Node<IGoal>>();

            //foreach (Path_Node<IGoal> n in node_map.Values)
            //{
            //    score[n] = (1 << 30);
            //}

            //score[node_map[start]] = 0;
            //open_set.Enqueue(node_map[start], score[node_map[start]]);
            //Event current_event = new Event(EventManager.instance.GetEvent("goal_assessment"));
            //while (open_set.Count > 0)
            //{
            //    Path_Node<IGoal> current = open_set.Dequeue();
            //    target = current.data.PassTarget(source, target_map[current]);
            //    Event new_event = new Event();

            //    if (current == end)
            //    {
            //        ReconstructPath(came_from, current);
            //        return;
            //    }

            //    closed_set.Add(current);
            //    foreach (Path_Edge<IGoal> e in current.edges)
            //    {
            //        if (closed_set.Contains(e.node))
            //        {
            //            continue;
            //        }
            //        if (Mathf.CeilToInt(e.modifier) * e.node.data.Adversion(source, inital_target) <= 0 || e.node.data.Achieved(source,target))
            //        {
            //            continue;
            //        }

            //        int tentative_g = score[current] + Mathf.CeilToInt(e.modifier) * e.node.data.Adversion(source, target, current.data);

            //        if (open_set.Contains(e.node) == false)
            //        {
            //            open_set.Enqueue(e.node, tentative_g);
            //            target_map.Add(e.node, target);
            //        }
            //        else if (tentative_g >= score[e.node])
            //        {
            //            continue;
            //        }

            //        came_from[e.node] = current;
            //        target_map[e.node] = target;
            //        score[e.node] = tentative_g;
            //    }
            //}
            //path = new Queue<IGoal>();
            //Debug.LogWarning($"Goal AI could not find a goal path from {start.id()} to {end.id()}");
        }


        void ReconstructPath(Dictionary<Path_Node<IGoal>, Path_Node<IGoal>> came_from, Path_Node<IGoal> current)
        {
            Queue<IGoal> rev_path = new Queue<IGoal>();
            edge_path = new Queue<Path_Edge<IGoal>>();
            end = current.data;

            while (came_from.ContainsKey(current))
            {
                rev_path.Enqueue(current.data);
                edge_path.Enqueue(came_from[current].FindEdge(current));
                current = came_from[current];
            }
            rev_path.Enqueue(current.data);

            path = new Queue<IGoal>(rev_path.Reverse());
            edge_path = new Queue<Path_Edge<IGoal>>(edge_path.Reverse());

            path.Dequeue();
        }

        public List<IGoal> GetPath()
        {
            return path.ToList();
        }

        public Dictionary<IGoal, Entity> TargetMap()
        {
            Dictionary<IGoal, Entity> targets = new Dictionary<IGoal, Entity>();

            foreach (KeyValuePair<Path_Node<IGoal>, Entity> kv in target_map)
            {
                targets.Add(kv.Key.data, kv.Value);
            }

            return targets;
        }

        public void AppendTargetMap(Dictionary<IGoal, Entity> targets)
        {
            foreach (KeyValuePair<Path_Node<IGoal>, Entity> kv in target_map)
            {
                targets.Add(kv.Key.data, kv.Value);
            }
        }
    }
}

