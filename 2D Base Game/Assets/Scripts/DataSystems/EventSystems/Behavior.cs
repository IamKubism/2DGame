using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;

namespace Psingine
{
    public interface IComponentBehavior
    {
        void Trigger(Event e, IBaseComponent b);
    }

    public class DiceAddBehavior : IComponentBehavior
    {
        string dice_field;
        string param_name;
        string child_event_name = "";
        string child_type = "";
        int priority = 100;


        public DiceAddBehavior(JObject jobj)
        {
            dice_field = jobj.Value<string>("dice_field");
            if (jobj.TryGetValue("param_name", out JToken j))
            {
                param_name = j.Value<string>();
            }
            else
            {
                Debug.LogWarning("Could not find value for the param name that a behavior will modify");
                param_name = "NONE";
            }
            if (jobj.TryGetValue("child_event_name", out JToken p))
            {
                child_event_name = p.Value<string>();
            }
            if (jobj.TryGetValue("child_type", out JToken c))
            {
                child_type = c.Value<string>();
            }
            if (jobj.TryGetValue("priority", out JToken o))
            {
                priority = o.Value<int>();
            }
        }

        public DiceAddBehavior(DiceAddBehavior prot)
        {
            dice_field = prot.dice_field;
            param_name = prot.param_name;
            child_event_name = prot.child_event_name;
            child_type = prot.child_type;
            priority = prot.priority;
        }

        public void Trigger(Event e, IBaseComponent comp)
        {
            e.AddUpdate((ent) => AddDiceToParam(ent, comp), priority);
        }

        public void AddDiceToParam(Event e, IBaseComponent comp)
        {
            e.SetParamValue(param_name, (DiceGroup)comp.GetType().GetField(dice_field, PrototypeLoader.field_flags).GetValue(comp), (d1, d2) => { return d1 + d2; });
        }
    }
    public class IntAddBehavior : IComponentBehavior
    {
        string int_field;
        string param_name;
        string child_event_name = "";
        string child_type = "";
        int priority = 100;


        public IntAddBehavior(JObject jobj)
        {
            int_field = jobj.Value<string>("int_field");
            if (jobj.TryGetValue("param_name", out JToken j))
            {
                param_name = j.Value<string>();
            }
            else
            {
                Debug.LogWarning("Could not find value for the param name that a behavior will modify");
                param_name = "NONE";
            }
            if (jobj.TryGetValue("child_event_name", out JToken p))
            {
                child_event_name = p.Value<string>();
            }
            if (jobj.TryGetValue("child_type", out JToken c))
            {
                child_type = c.Value<string>();
            }
            if (jobj.TryGetValue("priority", out JToken o))
            {
                priority = o.Value<int>();
            }
        }

        public IntAddBehavior(IntAddBehavior prot)
        {
            int_field = prot.int_field;
            param_name = prot.param_name;
            child_event_name = prot.child_event_name;
            child_type = prot.child_type;
            priority = prot.priority;
        }

        public void Trigger(Event e, IBaseComponent comp)
        {
            e.AddUpdate((ent) => AddDiceToParam(ent, comp), priority);
        }

        public void AddDiceToParam(Event e, IBaseComponent comp)
        {
            e.SetParamValue(param_name, (int)comp.GetType().GetField(int_field, PrototypeLoader.field_flags).GetValue(comp), (d1, d2) => { return d1 + d2; });
        }
    }
    public class FloatAddBehavior : IComponentBehavior
    {
        string float_field;
        string param_name;
        string child_event_name = "";
        string child_type = "";
        int priority = 100;


        public FloatAddBehavior(JObject jobj)
        {
            float_field = jobj.Value<string>("float_field");
            if (jobj.TryGetValue("param_name", out JToken j))
            {
                param_name = j.Value<string>();
            }
            else
            {
                Debug.LogWarning("Could not find value for the param name that a behavior will modify");
                param_name = "NONE";
            }
            if (jobj.TryGetValue("child_event_name", out JToken p))
            {
                child_event_name = p.Value<string>();
            }
            if (jobj.TryGetValue("child_type", out JToken c))
            {
                child_type = c.Value<string>();
            }
            if (jobj.TryGetValue("priority", out JToken o))
            {
                priority = o.Value<int>();
            }
        }

        public FloatAddBehavior(FloatAddBehavior prot)
        {
            float_field = prot.float_field;
            param_name = prot.param_name;
            child_event_name = prot.child_event_name;
            child_type = prot.child_type;
            priority = prot.priority;
        }

        public void Trigger(Event e, IBaseComponent comp)
        {
            e.AddUpdate((ent) => AddDiceToParam(ent, comp), priority);
        }

        public void AddDiceToParam(Event e, IBaseComponent comp)
        {
            e.SetParamValue(param_name, (float)comp.GetType().GetField(float_field, PrototypeLoader.field_flags).GetValue(comp), (d1, d2) => { return d1 + d2; });
        }
    }
    public class SetEventParamBehavior : IComponentBehavior
    {
        string field_name;
        string param_name;
        string child_event_name = "";
        string child_type = "";
        int priority = 100;

        public SetEventParamBehavior(JObject jobj, IBaseComponent comp)
        {
            field_name = jobj.Value<string>("field_name");
            if (jobj.TryGetValue("param_name", out JToken j))
            {
                param_name = j.Value<string>();
            }
            else
            {
                Debug.LogWarning("Could not find value for the param name that a behavior will modify");
                param_name = "NONE";
            }
            if (jobj.TryGetValue("child_event_name", out JToken p))
            {
                child_event_name = p.Value<string>();
            }
            if (jobj.TryGetValue("child_type", out JToken c))
            {
                child_type = c.Value<string>();
            }
            if (jobj.TryGetValue("priority", out JToken o))
            {
                priority = o.Value<int>();
            }
        }

        public SetEventParamBehavior(SetEventParamBehavior prot)
        {
            field_name = prot.field_name;
            param_name = prot.param_name;
            child_event_name = prot.child_event_name;
            child_type = prot.child_type;
            priority = prot.priority;
        }

        public void Trigger(Event e, IBaseComponent comp)
        {
            e.AddUpdate((ent) => SetParam(ent, comp), priority);
        }

        public void SetParam(Event e, IBaseComponent comp)
        {
            e.SetParamValue(param_name, comp.GetType().GetField(field_name, PrototypeLoader.field_flags).GetValue(comp), (d1, d2) => { return d2; });
        }
    }
    public class SetComponentFieldBehavior : IComponentBehavior
    {
        string field_name;
        string param_name;
        string child_event_name = "";
        string child_type = "";
        int priority = 100;

        public SetComponentFieldBehavior(JObject jobj, IBaseComponent comp)
        {
            field_name = jobj.Value<string>("field_name");
            if (jobj.TryGetValue("param_name", out JToken j))
            {
                param_name = j.Value<string>();
            }
            else
            {
                Debug.LogWarning("Could not find value for the param name that a behavior will modify");
                param_name = "NONE";
            }
            if (jobj.TryGetValue("child_event_name", out JToken p))
            {
                child_event_name = p.Value<string>();
            }
            if (jobj.TryGetValue("child_type", out JToken c))
            {
                child_type = c.Value<string>();
            }
            if (jobj.TryGetValue("priority", out JToken o))
            {
                priority = o.Value<int>();
            }
        }

        public SetComponentFieldBehavior(SetComponentFieldBehavior prot)
        {
            field_name = prot.field_name;
            param_name = prot.param_name;
            child_event_name = prot.child_event_name;
            child_type = prot.child_type;
            priority = prot.priority;
        }

        public void Trigger(Event e, IBaseComponent comp)
        {
            e.AddUpdate((ent) => SetParam(ent, comp), priority);
        }

        public void SetParam(Event e, IBaseComponent comp)
        {
            comp.GetType().GetField(field_name, PrototypeLoader.field_flags).SetValue(comp, e.GetParamValue<object>(param_name));
        }
    }

}

