using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HighKings
{
    /// <summary>
    /// A component that tracks the goals that an entity wants to do in order of their hierarchal order.
    /// </summary>
    public class Conscious : IBaseComponent
    {
        Entity parent;
        SubscriberEvent<Conscious> listener;
        List<IGoal> goals;
        Dictionary<IGoal, Entity> goal_targets;

        public Conscious()
        {
            goals = new List<IGoal>();
            goal_targets = new Dictionary<IGoal, Entity>();
        }

        public Conscious(Entity parent)
        {
            goals = new List<IGoal>();
            goal_targets = new Dictionary<IGoal, Entity>();
            this.parent = parent;
        }

        public Conscious(Conscious conscious)
        {
            parent = conscious.parent;
            goals = new List<IGoal>(conscious.goals);
            goal_targets = new Dictionary<IGoal, Entity>(conscious.goal_targets);

        }

        public Conscious(Entity parent, Conscious conscious)
        {
            this.parent = parent;
            goals = new List<IGoal>(conscious.goals);
            goal_targets = new Dictionary<IGoal, Entity>(conscious.goal_targets);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="prop"></param>
        public Conscious(JProperty prop)
        {
            goals = new List<IGoal>();
            goal_targets = new Dictionary<IGoal, Entity>();
        }

        public void SetGoals(IGoal goal, Entity target)
        {
            listener.OperateBeforeOnComp();
            Path_Node<IGoal> goal_node = FullGoalMap.instance[goal];
            List<IGoal> new_path = new List<IGoal>();
            while(goal_node.edges.Count > 0)
            {
                List<Path_Node<IGoal>> edges = new List<Path_Node<IGoal>>();
                int max_pref = 0;

                foreach (Path_Edge<IGoal> e in goal_node.edges)
                {
                    if (e.node.data.Preference(parent,target) >= max_pref)
                    {
                        if (e.node.data.Preference(parent, target) == max_pref)
                        {
                            edges.Add(e.node);
                        }
                        else
                        {
                            edges = new List<Path_Node<IGoal>> { e.node };
                            max_pref = e.node.data.Preference(parent, target);
                        }
                    }
                }

                if(edges.Count == 0)
                {
                    return;
                }

                goal_node = edges[UnityEngine.Random.Range(0, edges.Count)];
                new_path.Add(goal_node.data);
            }
            goals.AddRange(new_path);
            listener.OperateAfterOnComp();
        }

        public void CheckGoalsForward()
        {
            listener.OperateBeforeOnComp();
            if (goals.Count > 0)
            {
                int i = 0;
                while(i < goals.Count)
                {
                    if (goals[i].Achieved(parent, goal_targets[goals[i]]))
                    {
                        while(i < goals.Count)
                        {
                            goals[i].Cancel(parent, goal_targets[goals[i]]);
                            goal_targets.Remove(goals[i]);
                            goals.RemoveAt(i);
                        }
                    } else
                    {
                        i += 1;
                    }
                }
            }
            listener.OperateAfterOnComp();
        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<Conscious>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<Conscious>));
        }
    }
}

