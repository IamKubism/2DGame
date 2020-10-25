using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ComponentInfo
    {
        [JsonProperty]
        public string component_name;

        public bool errored = false;

        public object data;

        public ComponentInfo()
        {
            errored = false;
        }

        public ComponentInfo(ComponentInfo info)
        {
            component_name = info.component_name;
            errored = info.errored;
            ConstructorInfo constructor = info.data.GetType().GetConstructor(new Type[1] { info.data.GetType() });
            if (constructor == null)
            {
                Debug.LogError($"Could not find the copy constructor for {component_name}");
                errored = true;
            }
            data = constructor.Invoke(new object[1] { info.data });
        }

        public ComponentInfo(JProperty comp_json, object generated_comp)
        {
            component_name = comp_json.Name;
            data = generated_comp;
        }

        public ComponentInfo OverwriteComponentInfo(JProperty prop, object generated_obj)
        {
            ComponentInfo component_info = new ComponentInfo(this);

            component_info.data = component_info.data.GetType().GetConstructor(new Type[1] { component_info.data.GetType() }).Invoke(new object[1] { component_info.data });

            if(prop.Value["data"] != null)
            {
                List<FieldInfo> fields = new List<FieldInfo>();
                List<JToken> toks = prop.Value["data"].ToList();
                foreach(JProperty p in toks)
                {
                    FieldInfo f = generated_obj.GetType().GetField(p.Name, PrototypeLoader.field_flags);
                    if(f == null)
                    {
                        Debug.LogError($"Could not find field {p.Name} for component {component_info.component_name}");
                        continue;
                    }
                    fields.Add(f);
                }
                foreach(FieldInfo f in fields)
                {
                    f.SetValue(component_info.data, f.GetValue(generated_obj));
                }
            }

            return component_info;
        }

        public override string ToString()
        {
            string s = $"Name: {component_name}, Value: {data.ToString()}";
            return s;
        }
    }
}