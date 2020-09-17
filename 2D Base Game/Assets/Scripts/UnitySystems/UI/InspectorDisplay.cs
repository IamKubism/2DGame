using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class InspectorDisplay
    {
        [JsonProperty]
        public string display_name;

        [JsonProperty]
        public string component_name;

        [JsonProperty]
        public int default_position;

        public int position;

        [JsonProperty]
        public string type;

        public InspectorDisplay()
        {
            default_position = 100;
            position = 100;
        }
    }
}
