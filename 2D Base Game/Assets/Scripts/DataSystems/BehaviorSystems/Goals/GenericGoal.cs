using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class GenericGoal : IGoal
    {
        string _id;
        public int pref_sqr_dist_max;
        public int pref_sqr_dist_min;
        List<EntityAction> on_assigned;
        List<EntityAction> on_cancelled;
        List<EntityAction> on_achieved;

        public GenericGoal(string _id)
        {
            this._id = _id;
            on_assigned = new List<EntityAction>();
            on_cancelled = new List<EntityAction>();
            on_achieved = new List<EntityAction>();
        }

        public GenericGoal(JProperty prop)
        {
            _id = prop.Name;
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
                foreach (EntityAction act in on_achieved)
                {
                    act.Invoke(source, target);
                }
            }
            return eval;
        }

        public void Assign(Entity source, Entity target)
        {
            foreach (EntityAction act in on_assigned)
            {
                act.Invoke(source, target);
            }
        }

        public void Cancel(Entity source, Entity target)
        {
            foreach (EntityAction act in on_cancelled)
            {
                act.Invoke(source, target);
            }
        }

        public Entity PassTarget(Entity source, Entity initial_target)
        {
            return initial_target;
        }

        public string id()
        {
            return _id;
        }

        public bool IsAchievable(Entity source, Entity target)
        {
            //TODO
            return true;
        }

        public int Adversion(Entity source, Entity target)
        {
            throw new System.NotImplementedException();
        }

        public int Adversion(Entity source, Entity target, IGoal prev_goal)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterAchievedAction(EntityAction action)
        {
            if (on_assigned.Contains(action) == false)
            {
                on_assigned.Add(action);
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

        public void UnRegisterAchievedAction(EntityAction action)
        {
            if (on_assigned.Contains(action))
            {
                on_assigned.Remove(action);
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

        public Dictionary<string, Tuple<int, int>> ValuesToPass(Entity source, Entity target, IGoal prev_goal)
        {
            throw new NotImplementedException();
        }
    }

    public static class GenericRequirementFunctions
    {
        public static bool SourceStatCheck(Entity entity, Entity target, string stat_id, int min, int max)
        {
            BaseStatistic stat = entity.GetComponent<BaseStatistic>(stat_id);
            return stat.curr_value >= min && stat.curr_value <= max;
        }

        public static bool TargetStatCheck(Entity entity, Entity target, string stat_id, int min, int max)
        {
            BaseStatistic stat = target.GetComponent<BaseStatistic>(stat_id);
            return stat.curr_value >= min && stat.curr_value <= max;
        }

        public static bool PositionCheck(Entity source, Entity target, int sqr_min_dist, int sqr_max_dist)
        {
            int dist = MathFunctions.SqrDist(source.GetComponent<Position>("Position").p, target.GetComponent<Position>("Position").p);
            return dist <= sqr_max_dist && dist >= sqr_min_dist;
        }
    }

    //public class GenericRequirement
    //{
    //    public enum Gate
    //    {
    //        AND,
    //        OR,
    //        XOR
    //    }

    //    GenericRequirement sub_req;
    //    Gate gate;
    //    Func<Entity, Entity, bool> this_check;

    //    public bool Evaluate(Entity source, Entity target)
    //    {
    //        bool temp = this_check(source, target);
    //        if(sub_req == null)
    //        {
    //            return temp;
    //        }
    //        switch (gate)
    //        {
    //                case Gate.AND:
    //                temp &= sub_req.Evaluate(source, target);
    //                break;
    //                case Gate.OR:
    //                temp |= sub_req.Evaluate(source, target);
    //                break;
    //                case Gate.XOR:
    //                temp ^= sub_req.Evaluate(source, target);
    //                break;
    //        }
    //        return temp;
    //    }
    //}
}
