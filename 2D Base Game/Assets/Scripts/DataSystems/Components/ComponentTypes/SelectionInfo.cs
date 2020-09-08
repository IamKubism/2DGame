using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class SelectionInfo
    {
        public string display_type { get; protected set; }
        public ISelectionInfoType info;

        public SelectionInfo()
        {

        }

        public SelectionInfo(SelectionInfo s_inf)
        {
            display_type = s_inf.display_type;
            info = s_inf.info;
        }

        
    }
}
