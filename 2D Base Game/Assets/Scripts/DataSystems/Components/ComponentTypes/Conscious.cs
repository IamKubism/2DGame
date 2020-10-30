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
        public SubscriberEvent subscriber { get; set; }
        List<IGoal> goals;
        Dictionary<IGoal, Entity> goal_targets;
        List<Entity> goal_actions;

        public Conscious()
        {
            goals = new List<IGoal>();
            goal_targets = new Dictionary<IGoal, Entity>();
            goal_actions = new List<Entity>();
        }

        public Conscious(Entity parent)
        {
            goals = new List<IGoal>();
            goal_actions = new List<Entity>();
            goal_targets = new Dictionary<IGoal, Entity>();
            this.parent = parent;
            goals.Add(FullGoalMap.instance["NoGoal"]);
            goal_targets.Add(goals[0], parent);
        }

        public Conscious(Conscious conscious)
        {
            parent = conscious.parent;
            goal_actions = new List<Entity>();
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

        public void SetGoals(Entity target, bool call_subscribers = false)
        {
            if(call_subscribers)
                subscriber.OperateBeforeOnComp();
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
            if(call_subscribers)
                subscriber.OperateAfterOnComp();
        }

        public void SetGoal(IGoal goal, Entity target, bool call_subscribers = false)
        {
            if(call_subscribers)
                subscriber.OperateBeforeOnComp();
            if (goal_targets.ContainsKey(goal))
            {
                Debug.LogError("Cannot have cyclic goals");
                return;
            }
            goals.Add(goal);
            goal_targets.Add(goal, target);
            goal.Assign(parent, target);
            if(call_subscribers)
                subscriber.OperateAfterOnComp();
        }

        public bool CheckGoalsForward(bool call_subscribers = false)
        {
            bool eval = false;
            if(call_subscribers)
                subscriber.OperateBeforeOnComp();
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
            if(call_subscribers)
                subscriber.OperateAfterOnComp();
            return eval;
        }

        public void ReEvaluate()
        {
            subscriber.OperateBeforeOnComp();
            CheckGoalsForward();
            SetGoals(goal_targets[goals[goals.Count - 1]]);
            subscriber.OperateAfterOnComp();
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if (e.tags.Contains("AssignGoal"))
            {
            }
            if (e.tags.Contains("EvaluateGoals"))
            {

            }
            return eval;
        }

        public bool AddAssignmentProtocol(Event e)
        {
            bool eval = true;
            Entity target = e.GetParamValue<Entity>("target_entity");

            return eval;
        }

        public bool SetAGoal(Event e)
        {
            bool eval = false;

            return eval;
        }
    }
}

