using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace HighKings
{
    /// <summary>
    /// Might be conflating goals with tasks here
    /// </summary>
    public interface IGoal
    {
        string id { get; set; }
        bool Achieved(Entity source, Entity target);
        bool Achieved(Event e);
        bool Adversion(Event e);
        bool Adversion(Entity source, Entity target, IGoal prev_goal);
        Entity PassTarget(Entity source, Entity entity);
    }

    public class AchievementProgress : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        List<Tuple<string, bool>> achieve_bools;

        public AchievementProgress()
        {
            achieve_bools = new List<Tuple<string, bool>>();
        }

        public AchievementProgress(AchievementProgress g, bool reset = false)
        {
            achieve_bools = new List<Tuple<string, bool>>(g.achieve_bools);
            if (reset)
            {
                for(int i = 0; i < achieve_bools.Count; i += 1)
                {
                    achieve_bools[i] = new Tuple<string, bool>(achieve_bools[i].Item1, false);
                }
            }
        }

        public AchievementProgress(JObject obj)
        {
            achieve_bools = new List<Tuple<string, bool>>();
            foreach(JProperty p in obj["with_bools"])
            {
                achieve_bools.Add(new Tuple<string, bool>(p.Name, p.Value<bool>()));
            }
            foreach(JToken p in obj["without_bools"])
            {
                achieve_bools.Add(new Tuple<string, bool>(p.Value<string>(), false));
            }
        }

        public bool Trigger(Event e)
        {
            bool eval = true;
            if (e.tags.Contains("decision"))
            {
                e.AddUpdate(AddDecisionSet, 0);
            }
            e.AddUpdate(AddGoalAssessParams, 0);
            e.AddUpdate(AddAchievementBools, 0);
            e.AddUpdate(SetAchieveBools, 200);
            return eval;
        }

        void AddAchievementBools(Event e)
        {
            foreach(Tuple<string,bool> tb in achieve_bools)
            {
                e.SetParamValue(tb.Item1, tb.Item2, (b1, b2) => { return b1 && b2; });
            }
        }

        void AddGoalAssessParams(Event e)
        {
            e.SetParamValue("adversion", 1, MathFunctions.NegativeRespectingSum);
        }

        void AddDecisionSet(Event e)
        {
            e.SetParamValue("decisions", new WeightedEventSet(), WeightedEventSet.Add);
        }

        void SetAchieveBools(Event e)
        {
            bool eval = true;
            for(int i = 0; i < achieve_bools.Count; i += 1)
            {
                Tuple<string, bool> kb = achieve_bools[i];
                achieve_bools[i] = new Tuple<string, bool>(kb.Item1, e.GetParamValue<bool>(kb.Item1));
                eval &= achieve_bools[i].Item2;
            }
            e.SetParamValue("curr_goal_achieved", eval, (b1, b2) => { return b2; });
        }
    }

    public class WeightedEventSet
    {
        List<Event> actions;
        int total_pref;

        public WeightedEventSet()
        {
            actions = new List<Event>();
            total_pref = 0;
        }

        public WeightedEventSet(WeightedEventSet d)
        {
            actions = new List<Event>(d.actions);
            total_pref = d.total_pref;
        }

        public WeightedEventSet(Event e)
        {
            actions = new List<Event>();
            AddAction(e);
        }

        public void AddAction(Event e)
        {
            if(e.GetParamValue<int>("preference") <= 0)
            {
                return;
            }
            int before = total_pref;
            total_pref += e.GetParamValue<int>("preference");
            e.SetParamValue("total_preference", before+e.GetParamValue<int>("preference"), (i1,i2) => { return i2; });
            actions.Add(e);
        }

        public static Event RandomSelect(WeightedEventSet d)
        {
            if (d.actions.Count == 0)
            {
                return null;
            }
            int place = UnityEngine.Random.Range(0, d.total_pref+1);
            for (int i = 0; i < d.actions.Count; i += 1)
            {
                if (place <= d.actions[i].GetParamValue<int>("total_preference"))
                {
                    return d.actions[i];
                }
            }
            return d.actions[0];
        }

        public static Event HighSelect(WeightedEventSet d)
        {
            if(d.actions.Count == 0)
            {
                return null;
            }
            int max_pref = 0;
            List<Event> to_return = new List<Event>();
            foreach(Event e in d.actions)
            {
                int pref = e.GetParamValue<int>("preference");
                if (max_pref < pref)
                {
                    max_pref = pref;
                    to_return = new List<Event> { e };
                }
                if(max_pref == pref)
                {
                    to_return.Add(e);
                }
            }
            if(to_return.Count == 0)
            {
                return null;
            }
            int i = UnityEngine.Random.Range(0, to_return.Count);
            return to_return[i];
        }

        public static WeightedEventSet Add(WeightedEventSet d1, WeightedEventSet d2)
        {
            WeightedEventSet d3 = new WeightedEventSet(d1);
            foreach (Event e in d2.actions)
            {
                d3.AddAction(new Event(e));
            }
            return d3;
        }
    }
}
