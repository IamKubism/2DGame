using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    /// <summary>
    /// Class that keeps track of all possible goal chains that entities can execute
    /// </summary>
    public class FullGoalMap
    {
        public static FullGoalMap instance;
        public Dictionary<IGoal,Path_Node<IGoal>> graph;
        Dictionary<string, IGoal> goals_by_id;
        public Dictionary<string, List<string>> prototype_sub_maps;

        public FullGoalMap()
        {
            if(instance == null)
            {
                instance = this;
            } else
            {
                Debug.LogWarning("Full goal map is trying to be constructed twice");
            }
            graph = new Dictionary<IGoal, Path_Node<IGoal>>();
            goals_by_id = new Dictionary<string, IGoal>();
            prototype_sub_maps = new Dictionary<string, List<string>>();
        }

        public void RegisterMap(List<ItemVector<IGoal,List<IGoal>>> goals)
        {
            foreach(ItemVector<IGoal, List<IGoal>> g in goals)
            {
                if(graph.ContainsKey(g.a) == false)
                {
                    Debug.LogWarning($"Goal: {g.a.id()} was not in the node map, this could cause problems");
                    graph.Add(g.a, new Path_Node<IGoal>(g.a));
                    goals_by_id.Add(g.a.id(), g.a);

                }
                foreach (IGoal p in g.b)
                {
                    if (graph.ContainsKey(p))
                    {
                        graph[p].MakeNewEdge(graph[g.a], 1);
                    } else
                    {
                        Debug.LogWarning($"Could not find the goal node {p.id()}, not adding edge");
                    }
                }
            }
        }

        public void RegisterMap(List<ItemVector<IGoal,List<string>>> goals)
        {
            foreach(ItemVector<IGoal, List<string>> g in goals)
            {
                if(graph.ContainsKey(g.a) == false)
                {
                    Debug.LogWarning($"Goal: {g.a.id()} was not in the node map, this could cause problems");
                    graph.Add(g.a, new Path_Node<IGoal>(g.a));
                    goals_by_id.Add(g.a.id(), g.a);
                }
                foreach (string p in g.b)
                {
                    if (graph.ContainsKey(goals_by_id[p]))
                    {
                        graph[goals_by_id[p]].MakeNewEdge(graph[g.a], 1);
                    } else
                    {
                        Debug.LogWarning($"Could not find the goal node {p}, not adding edge");
                    }
                }
            }
        }

        public void AddToMap(List<IGoal> goals)
        {
            foreach(IGoal g in goals)
            {
                if (graph.ContainsKey(g))
                {
                    Debug.LogWarning($"Tried to add goal {g.id()} twice, there is likely a problem in the load files");
                } else
                {
                    graph.Add(g, new Path_Node<IGoal>(g));
                    goals_by_id.Add(g.id(), g);
                }
            }
        }

        public bool HasGoal(IGoal g)
        {
            return graph.ContainsKey(g);
        }

        public IGoal this[string key]
        {
            get => goals_by_id[key];
        }

        public Path_Node<IGoal> this[IGoal key]
        {
            get => graph[key];
        }
    }
}

