using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Psingine
{
    [MoonSharpUserData]
    public class AIPosition : IBaseComponent, IAIComponent
    {
        public int max_sqr = 1;
        public int min_sqr = 0;
        public SubscriberEvent subscriber { get; set; }

        public AIPosition()
        {

        }

        public AIPosition(JObject data)
        {
            if(data.TryGetValue("max_sqr", out JToken max))
            {
                max_sqr = max.Value<int>();
            }
            if(data.TryGetValue("min_sqr", out JToken min))
            {
                min_sqr = min.Value<int>();
            }
        }

        public AIPosition(AIPosition dist)
        {
            max_sqr = dist.max_sqr;
            min_sqr = dist.min_sqr;
        }

        public bool Trigger(Event e)
        {
            if(e.type == AIEvents.GetPref.ToString())
            {
                e.AddUpdate(Pref, (1 << 7));
            }
            if(e.type == AIEvents.EvalGoal.ToString())
            {
                e.AddUpdate(Achieved, (1 << 8));
            }
            return true;
        }

        public void Pref(Event e)
        {
            Position p = e.GetParamValue<Entity>(EventParams.invoking_entity).GetComponent<Position>();
            Position t = e.GetParamValue<Entity>(AIParams.goal_target).GetComponent<Position>();

            float dist_sqr = MathFunctions.SqrDist(p.p, t.p);
            float pref = 1f;

            //If the point is not in the desired area, then the pref is the inverse square of the entity's distance to the area
            //The formula used for the distance is easily found from minimization using lagrange multipliers or geometry
            if (dist_sqr > max_sqr || dist_sqr < min_sqr)
            {
                float dk_sqr = max_sqr;
                if (dist_sqr < min_sqr)
                {
                    dk_sqr = min_sqr;
                }
                float d2 = dist_sqr + dk_sqr - 2 * Mathf.Sqrt(dist_sqr * dk_sqr);
                pref = 1f / d2;
            }
            e.SetParamValue(AIParams.priority, pref, (f1, f2) => { return f1 * f2; });
        }

        public void Achieved(Event e)
        {
            bool eval = false;
            int[] c = e.GetParamValue<Entity>(EventParams.invoking_entity).GetComponent<Position>().p;
            foreach (Entity i in e.GetParamValue<HashSet<Entity>>(EventParams.targets))
            {
                int dist = MathFunctions.SqrDist(c, i.GetComponent<Position>().p);
                if (dist > min_sqr && dist < max_sqr)
                {
                    eval = true;
                    break;
                }
            }
            e.SetParamValue(AIParams.goal_complete, eval, (b1, b2) => { return b1 && b2; });
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new System.NotImplementedException();
        }
    }
}

