using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BaseStatistic : IBaseComponent, IInspectorDisplay
    { 
        [JsonProperty]
        public string stat_name { get; protected set; }

        [JsonProperty]
        public int curr_value;

        [JsonProperty]
        public int base_value;

        public SubscriberEvent subscriber { get; set; }
        public InspectorData inspector_data
        {
            get {
                //Debug.Log("Got Inspector data for Base Statistic");
                return MainGame.instance.display_data[stat_name];
                }
            set {
            }
        }

        public BaseStatistic(string stat_name, int curr_value, int base_value, BaseStatistic s)
        {
            this.stat_name = stat_name;
            this.curr_value = curr_value;
            this.base_value = base_value;
        }

        //The clone constructor
        public BaseStatistic(BaseStatistic s)
        {
            stat_name = s.stat_name;
            curr_value = s.curr_value;
            base_value = s.base_value;
        }

        //If this is called its pretty much an error
        public BaseStatistic()
        {
            stat_name = "";
            curr_value = 0;
            base_value = 0;
        }

        public override string ToString()
        {
            return $"{stat_name}, curr: {curr_value}, base: {base_value}";
        }

        public void SetCurr(int f)
        {
            subscriber.OperateBeforeOnComp();
            curr_value = f;
            subscriber.OperateAfterOnComp();
        }

        public void IncrementCurr(int f)
        {
            subscriber.OperateBeforeOnComp();
            curr_value += f;
            subscriber.OperateAfterOnComp();
        }

        public void SetListener(SubscriberEvent subscriber) 
        {
            this.subscriber = subscriber;
        }

        public bool Trigger(Event e)
        {
            throw new System.NotImplementedException();
        }

        public bool SetSubscriberListener(Action<IBaseComponent> action, bool before_after)
        {
            throw new NotImplementedException();
        }

        public string DisplayText()
        {
            return $"{stat_name}: {curr_value} / {base_value}";
        }
    }
}

