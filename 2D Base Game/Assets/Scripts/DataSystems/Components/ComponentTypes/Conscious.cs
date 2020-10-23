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
            goals.Add(FullGoalMap.instance["NoGoal"]);
            goal_targets.Add(goals[0], parent);
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
            goals.Add(FullGoalMap.instance["NoGoal"]);
            goal_targets.Add(goals[0], parent);
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

        public void SetGoals(Entity target, bool call_listeners = false)
        {
            if(call_listeners)
                listener.OperateBeforeOnComp();
            DjkistraGoal decision = new DjkistraGoal(FullGoalMap.instance.graph, goals[goals.Count - 1], parent, target);
            Dictionary<IGoal, Entity> target_map = decision.TargetMap();
            foreach (IGoal goal in decision.GetPath())
            {
                if (goals.Contains(goal))
                {
                    Debug.LogError("Cannot have a cyclic goal pattern");
                    return;
                }
            }
            foreach (IGoal goal in decision.GetPath())
            {
                goals.Add(goal);
                goal_targets.Add(goal, target_map[goal]);
                goal.Assign(parent, target_map[goal]);
            }
            if(call_listeners)
                listener.OperateAfterOnComp();
        }

        public void SetGoal(IGoal goal, Entity target)
        {
            listener.OperateBeforeOnComp();
            if (goal_targets.ContainsKey(goal))
            {
                Debug.LogError("Cannot have cyclic goals");
                return;
            }
            goals.Add(goal);
            goal_targets.Add(goal, target);
            goal.Assign(parent, target);
            listener.OperateAfterOnComp();
        }

        public bool CheckGoalsForward(bool call_listeners = false)
        {
            bool eval = false;
            if(call_listeners)
                listener.OperateBeforeOnComp();
            if (goals.Count > 0)
            {
                int i = 0;
                while(i < goals.Count)
                {
                    if (goals[i].Achieved(parent, goal_targets[goals[i]]))
                    {
                        eval = true;
                        goal_targets.Remove(goals[i]);
                        goals.RemoveAt(i);
                        while (i < goals.Count)
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
            if(call_listeners)
                listener.OperateAfterOnComp();
            return eval;
        }

        public void ReEvaluate()
        {
            listener.OperateBeforeOnComp();
            CheckGoalsForward();
            SetGoals(goal_targets[goals[goals.Count - 1]]);
            listener.OperateAfterOnComp();
        }

        public void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent
        {
            listener = (SubscriberEvent<Conscious>)Convert.ChangeType(subscriber, typeof(SubscriberEvent<Conscious>));
        }
    }
}

