using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class MoveGoal : IGoal
    {
        string _id;
        public int pref_sqr_dist_max;
        public int pref_sqr_dist_min;
        List<EntityAction> on_assigned;
        List<EntityAction> on_cancelled;
        List<EntityAction> on_achieved;

        public MoveGoal(JProperty prop)
        {
            _id = prop.Name;
            pref_sqr_dist_max = prop.Value<int>("max_dist");
            pref_sqr_dist_min = prop.Value<int>("min_dist");
            on_assigned = new List<EntityAction>();
            on_cancelled = new List<EntityAction>();
            on_achieved = new List<EntityAction>();
        }

        public bool Achieved(Entity source, Entity target)
        {
            int dist = MathFunctions.SqrDist(source.GetComponent<Position>("Position").p, target.GetComponent<Position>("Position").p);
            bool eval = dist >= pref_sqr_dist_min && dist <= pref_sqr_dist_max;
            if (eval)
            {
                foreach(EntityAction act in on_achieved)
                {
                    act.Invoke(source, target);
                }
            }
            return eval;
        }

        public void Assign(Entity source, Entity target)
        {
            Movers.instance.MoverPathMaker(World.instance.GetTilesAroundEntity(target, pref_sqr_dist_min, pref_sqr_dist_max), source);
            foreach(EntityAction act in on_assigned)
            {
                act.Invoke(source, target);
            }
        }

        public void Cancel(Entity source, Entity target)
        {
            Movers.CancelMove(source, target);
            foreach (EntityAction act in on_cancelled)
            {
                act.Invoke(source, target);
            }
        }

        public void RegisterAchievedAction(EntityAction action)
        {
            if (on_achieved.Contains(action) == false)
            {
                on_achieved.Add(action);
            }
        }

        public void RegisterCancelAction(EntityAction action)
        {
            if (on_cancelled.Contains(action) == false)
            {
                on_cancelled.Add(action);
            }
        }

        public void RegisterOnAssignedAction(EntityAction action)
        {
            if (on_assigned.Contains(action) == false)
            {
                on_assigned.Add(action);
            }
        }

        public Entity PassTarget(Entity source, Entity initial_target)
        {
            return initial_target;
        }

        public Dictionary<string, Tuple<int, int>> ValuesToPass(Entity source, Entity target, IGoal prev_goal)
        {
            Dictionary<string,Tuple<int, int>> to_pass = new Dictionary<string,Tuple<int, int>>();

            return to_pass;
        }


        public string id()
        {
            return _id;
        }

        public bool IsAchievable(Entity source, Entity target)
        {
            return true;
        }

        public int Adversion(Entity source, Entity target)
        {
            int dist = MathFunctions.SqrDist(source.GetComponent<Position>("Position").p, target.GetComponent<Position>("Position").p);
            return Mathf.Max(-pref_sqr_dist_min + dist, pref_sqr_dist_max - dist);
        }

        public int Adversion(Entity source, Entity target, IGoal prev_goal)
        {
            int dist = MathFunctions.SqrDist(source.GetComponent<Position>("Position").p, target.GetComponent<Position>("Position").p);
            return Mathf.Max(-pref_sqr_dist_min + dist, pref_sqr_dist_max - dist);
        }

        public void UnRegisterAchievedAction(EntityAction action)
        {
            if (on_achieved.Contains(action))
            {
                on_achieved.Remove(action);
            }
        }

        public void UnRegisterCancelAction(EntityAction action)
        {
            if (on_cancelled.Contains(action))
            {
                on_cancelled.Remove(action);
            }
        }

        public void UnRegisterOnAssignedAction(EntityAction action)
        {
            if (on_assigned.Contains(action))
            {
                on_assigned.Remove(action);
            }
        }
    }
}
