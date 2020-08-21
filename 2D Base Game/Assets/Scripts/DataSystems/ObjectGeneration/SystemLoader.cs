using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

namespace HighKings
{
    public class SystemLoader
    {
        public static SystemLoader instance;
        public Dictionary<string, object> systems;
        public Priority_Queue.SimplePriorityQueue<SystemInfo> queue;

        public SystemLoader()
        {
            if(instance != null)
            {
                Debug.LogError("SystemLoader instance is not null");
            } else
            {
                instance = this;
            }
            systems = new Dictionary<string, object>();
            queue = new Priority_Queue.SimplePriorityQueue<SystemInfo>();
        }

        public void AppendSystemList(string json_text, JsonParser parser)
        {
            List<SystemInfo> infos = parser.ParseString<List<SystemInfo>>(json_text);

            foreach (SystemInfo info in infos)
            {
                queue.Enqueue(info, info.load_priority);
            }
        }

        public void CreateSystem(SystemInfo info, object[] args)
        {
            ConstructorInfo constructor = info.system_type.GetConstructor(info.arg_types.ToArray());
            if(constructor == null)
            {
                string debug_text = "Could not find constructor for system " + info.system_type.ToString() +  $": {info.name}," + " with arguments: {";
                foreach (Type t in info.arg_types)
                {
                    debug_text += " " + t.ToString() + ",";
                }
                debug_text += "}";
                Debug.LogError(debug_text);
                return;
            }
            if (systems.ContainsKey(info.name))
            {
                Debug.LogError($"System {info.name} of type {info.system_type.ToString()} already created.");
                return;
            }
            systems.Add(info.name, constructor.Invoke(args));
            Debug.Log("Added system: " + info.name);
        }

        public void MakeAllLoadedSystems(Dictionary<string, object[]> system_args)
        {
            while (queue.Count > 0)
            {
                SystemInfo info = queue.Dequeue();
                CreateSystem(info, system_args[info.name]);
            }
        }

    }
}
