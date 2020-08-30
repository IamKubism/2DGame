using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Reflection;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ComponentInfo
    {
        [JsonProperty]
        List<string> constructor_arg_names;

        public List<Type> constructor_args;

        [JsonProperty]
        public string component_name, component_type;

        [JsonProperty]
        public int load_priority;

        [JsonProperty]
        public bool variable;

        public bool errored = false;

        public object data;

        public ComponentInfo()
        {
            constructor_args = new List<Type>();
            constructor_arg_names = new List<string>();
            errored = false;
        }

        public ComponentInfo(ComponentInfo info)
        {
            component_name = info.component_name;
            component_type = info.component_type;
            load_priority = info.load_priority;
            variable = info.variable;
            errored = info.errored;
            constructor_args = new List<Type>(info.constructor_args);
            constructor_arg_names = new List<string>(info.constructor_arg_names);
            ConstructorInfo constructor = Type.GetType(component_type).GetConstructor(new Type[1] { info.data.GetType() });
            if(constructor == null)
            {
                Debug.LogError($"Could not find the copy constructor for {component_name}");
            }
            data = constructor.Invoke(new object[1] { info.data });
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            foreach(string s in constructor_arg_names)
            {
                Type t = Type.GetType(s);
                if(t == null)
                {
                    Debug.LogError($"Could not find the type {s} for component {component_name} for the constructor on index {constructor_arg_names.IndexOf(s)}, setting this component to default");
                    errored = true;
                    return;
                }
                constructor_args.Add(t);
            }
        }

        public void SetData(object o, List<FieldInfo> fields)
        {
            if(data == null)
            {
                data = o;
            } else
            {
                foreach(FieldInfo field in fields)
                {
                    field.SetValue(data, field.GetValue(o));
                }
            }
        }

        public IBaseComponent CreateComponent(object[] args)
        {
            if (errored)
            {
                return default;
            }

            ConstructorInfo constructor = variable ? data.GetType().GetConstructor(constructor_args.ToArray()) 
                                                   : data.GetType().GetConstructor(new Type[1] { data.GetType() });

            if (constructor == null)
            {
                string s = $"Could not find correct constructor for {component_name} of type {component_type} with arguments:";
                foreach (Type t in constructor_args)
                {
                    s += t.Name + " ";
                }
                Debug.LogError(s);
                return default;
            }

            object[] casted_args = args == null ? new object[0] : new object[args.Length];
            for(int i = 0; i < casted_args.Length; i += 1)
            {
                casted_args[i] = Convert.ChangeType(args[i], constructor_args[i]);
            }

            return (IBaseComponent)constructor.Invoke(casted_args);
        }

        public override string ToString()
        {
            string s = $"Name: {component_name}, Type: {component_type}, Load Priority: {load_priority}, Variable: {variable}, Value: {data.ToString()} with constructor args {{";
            foreach(Type t in constructor_args)
            {
                s += " " + t.ToString();
            }
            s += "}";
            return s;
        }
    }
}