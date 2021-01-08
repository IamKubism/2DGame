using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SystemInfo
    {
        [JsonProperty]
        public string name;

        [JsonProperty]
        public string type;

        [JsonProperty]
        public string _namespace;

        [JsonProperty]
        public int load_priority;

        [JsonProperty]
        public Dictionary<string, string[][]> constructor_args;

        public List<Type> arg_types;
        public Type system_type;

        public SystemInfo()
        {
            constructor_args = new Dictionary<string, string[][]>();
            arg_types = new List<Type>();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            string[][] types = constructor_args["constructor_types"];
            int i = 0;
            foreach(string[] t in types)
            {
                //Debug.Log($"{t[0]} {t[1]}");
                switch (t[0]) {
                    case "Default":
                        arg_types.Add(Type.GetType(GetType().Namespace + "." + t[1]));
                        break;
                    case "none":
                        arg_types.Add(Type.GetType(t[1]));
                        break;
                    default:
                        arg_types.Add(Type.GetType(t[0] + "." + t[1]));
                        break;
                }
                if(arg_types[i] == null)
                {
                    Debug.LogError($"Type of {t[0]}.{t[1]} could not be found");
                }
                i += 1;
            }
            switch (_namespace)
            {
                case "Default":
                    system_type = Type.GetType(GetType().Namespace + "." + type);
                    break;
                case "":
                    system_type = Type.GetType(type);
                    break;
                default:
                    system_type = Type.GetType(_namespace + "." + type);
                    break;
            }
            if (system_type == null)
            {
                Debug.LogError($"System type {_namespace}.{type} could not be found");
            }
        }
    }
}

