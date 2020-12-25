using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Psingine
{
    public class GraphUtilities
    {
        public GraphUtilities()
        {
        }

        /// <summary>
        /// Thought this might be a nice utility, should primarily be used for small graphs (cause a lot of hashing is involved). Creates a graph from a set of items and ids
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node_ids"></param>
        /// <param name="edges_by_id"></param>
        /// <returns></returns>
        public static void AddToGraph<T>(Dictionary<T, Path_Node<T>> parent_graph, Dictionary<string, T> node_ids, List<Tuple<string, string>> edges_by_id)
        {
            foreach (T t in node_ids.Values)
            {
                if(parent_graph.ContainsKey(t) == false)
                    parent_graph.Add(t, new Path_Node<T>(t));
            }

            foreach(Tuple<string,string> edge in edges_by_id)
            {
                if(node_ids.ContainsKey(edge.Item1) && node_ids.ContainsKey(edge.Item2) == false)
                {
                    Debug.LogError($"Tried to add an edge with incorrect ids {edge.Item1} to {edge.Item2}");
                    continue;
                }
                parent_graph[node_ids[edge.Item1]].MakeNewEdge(parent_graph[node_ids[edge.Item2]], 1f);
            }
        }
    }
}

