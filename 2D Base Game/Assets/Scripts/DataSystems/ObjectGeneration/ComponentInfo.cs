using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ComponentInfo
    {
        [JsonProperty]
        public string component_name, component_type, system_location;

        [JsonProperty]
        public int load_priority;

        [JsonProperty]
        public bool variable;

        public ComponentInfo()
        {

        }

        public override string ToString()
        {
            return $"Name: {component_name}, Type: {component_type}, System Name: {system_location}, Load Priority: {load_priority}, Variable: {variable}";
        }
    }
}