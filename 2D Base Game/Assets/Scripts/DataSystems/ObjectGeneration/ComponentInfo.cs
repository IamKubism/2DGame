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
        public string component_name, component_type, system_location;

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
            if(fields == null || fields?.Count == 0)
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

        public IBaseComponent CopyData(object[] args)
        {
            if (errored)
            {
                return default;
            }

            ConstructorInfo constructor = data.GetType().GetConstructor(constructor_args.ToArray());
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
            return $"Name: {component_name}, Type: {component_type}, System Name: {system_location}, Load Priority: {load_priority}, Variable: {variable}";
        }
    }
}