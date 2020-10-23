using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    /// <summary>
    /// Class that keeps track of all possible goal chains that entities can execute
    /// </summary>
    public class FullGoalMap
    {
        public static FullGoalMap instance;
        public Dictionary<IGoal,Path_Node<IGoal>> graph;
        public Dictionary<string, IGoal> goals_by_id;

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
        }

        public void AddGoal(IGoal goal)
        {
            if (graph.ContainsKey(goal))
            {
                Debug.LogError($"Tried to add goal: {goal.id()} twice");
                return;
            }
            graph.Add(goal, new Path_Node<IGoal>(goal));
            goals_by_id.Add(goal.id(), goal);
        }

        public void AddEdgesToGraph(List<Tuple<string,string>> edges)
        {
            if (graph == null)
            {
                graph = new Dictionary<IGoal, Path_Node<IGoal>>();
            } else
            {
                GraphUtilities.AddToGraph(graph, goals_by_id, edges);
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

        public override string ToString()
        {
            string s = "FullGoalMap: ";
            foreach(Path_Node<IGoal> gn in graph.Values)
            {

            }
            return s;
        }
    }
}

// Ideas on how I want this shit to work:
// 