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

        public SystemLoader(Dictionary<string, object> systems)
        {
            if(instance != null)
            {
                Debug.LogError("SystemLoader instance is not null and tried to re-create it");
            } else
            {
                instance = this;
            }
            this.systems = systems;
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
            object[] casted_args = new object[args.Length];
            for(int i = 0; i < args.Length; i += 1)
            {
                casted_args[i] = Convert.ChangeType(args[i], info.arg_types[i]);
            }
            systems.Add(info.name, constructor.Invoke(casted_args));
        }

        public void MakeAllLoadedSystems(Dictionary<string, object[]> system_args)
        {
            while (queue.Count > 0)
            {
                SystemInfo info = queue.Dequeue();
                if (system_args.ContainsKey(info.name))
                    CreateSystem(info, system_args[info.name]);
                else
                    CreateSystem(info, new object[0] { });
            }
        }

    }
}
