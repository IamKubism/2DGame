using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HighKings
{
    public class InspectorDisplay : MonoBehaviour
    {
        public string display_name;
        public string component_name;
        public string component_type;
        public int default_position;
        public int position;
        public string display_type;

        void Awake()
        {
            default_position = 100;
            position = 100;
        }

        public void CopyData(InspectorData id)
        {
            display_name = id.display_name;
            component_name = id.component_name;
            default_position = id.default_position;
            position = id.position;
            display_type = id.display_type;
            component_type = id.component_type;
        }
    }

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
    }
}
