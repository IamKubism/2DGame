using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;

namespace HighKings
{
    public class InspectorDisplay : MonoBehaviour
    {
        public string display_name;
        public string component_name;
        public string component_type;
        public int position;
        public string display_type;

        void Awake()
        {
            position = 100;
        }

        public void CopyData(InspectorData id)
        {
            display_name = id.display_name;
            component_name = id.component_name;
            position = id.position;
            display_type = id.display_type;
            component_type = id.component_type;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class InspectorData
    {
        [JsonProperty]
        public string display_name;

        [JsonProperty]
        public string component_name;

        [JsonProperty]
        public string component_type;

        [JsonProperty]
        public int default_position;

        public int position;

        [JsonProperty]
        public string display_type;

        public InspectorData()
        {
            display_name = component_name = component_type = display_type = "NULLERROR";
            position = default_position = 1000;
        }

        public InspectorData(JProperty prop)
        {
            component_name = prop.Name;
            component_type = PrototypeLoader.GenerateTypeName(prop.Value.Value<string>("type"), prop.Value.Value<string>("namespace"));
            JToken insp_prop = prop.Value["inspector_display"];
            default_position = insp_prop.Value<int>("default_position");
            display_name = insp_prop.Value<string>("display_name");
            display_type = insp_prop.Value<string>("display_type");
        }
    }
}
