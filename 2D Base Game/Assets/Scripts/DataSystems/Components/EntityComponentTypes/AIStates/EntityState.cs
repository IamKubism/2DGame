using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public class EntityTarget : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public string target;

        public bool Trigger(Event e)
        {
            if(e.type == AIEvents.GetCurrGoalData.ToString())
            {
                e.AddUpdate((a) => { a.SetParamValue(AIParams.goal_target, target, (s1, s2) => { return s2; }); }, 0);
            }
            return true;
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new System.NotImplementedException();
        }
    }

    public class CurrState : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public AIState state;
        public string my_entity;

        public CurrState()
        {
            
        }

        public CurrState(CurrState prot)
        {
            state = new AIState(prot.state);
        }

        public CurrState(Entity e, CurrState prot)
        {
            my_entity = e;
            state = new AIState(prot.state);
        }

        public CurrState(JObject obj)
        {
        }

        public bool Trigger(Event e)
        {
            new Event(state.continue_event).Invoke(my_entity);
            return true;
        }

        public void SetState(AIState state)
        {
            this.state = new AIState(state);
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new System.NotImplementedException();
        }
    }
}
