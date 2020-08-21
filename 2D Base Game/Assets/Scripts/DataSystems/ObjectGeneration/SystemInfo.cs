using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

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
            foreach(string[] t in types)
            {
                switch (t[0]) {
                    case "Default":
                        arg_types.Add(Type.GetType(GetType().Namespace + "." + t[1]));
                        break;
                    case "None":
                        arg_types.Add(Type.GetType(t[1]));
                        break;
                    default:
                        arg_types.Add(Type.GetType(t[0] + "." + t[1]));
                        break;
                }
            }
            switch (_namespace)
            {
                case "Default":
                    system_type = Type.GetType(GetType().Namespace + "." + type);
                    break;
                case "None":
                    system_type = Type.GetType(type);
                    break;
                default:
                    system_type = Type.GetType(_namespace + "." + type);
                    break;
            }
        }
    }
}

