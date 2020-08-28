using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateFlag : IBaseComponent
    {
        [JsonProperty]
        public bool flag;
        
        public UpdateFlag()
        {
            flag = false;
        }

        public UpdateFlag(bool flag)
        {
            this.flag = flag;
        }
    }

}
