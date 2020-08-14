using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class SelectionInfo : IBaseComponent
    {
        public int priority;
        public string desc_fields;
        /* Ex of what I want for desc_fields: (gives the description with references to other fields / methods)
         * "[Entity:entity_string_id] [comp:Position] at ([field:x],[field:y],[field:z]) [comp:RenderComponent]and\n[method:ToString]"
         * Requires a Parse Script (maybe can just be implemented elsewhere and then 
         */
        public string display_name;
        /* Ex of display_name:
         * Same type of thing as above
         */

        public string ComponentType()
        {
            return "SelectionInfo";
        }

        public bool computable()
        {
            return true;
        }

        public void OnUpdateState()
        {
            throw new System.NotImplementedException();
        }

        public void RegisterUpdateAction(Action<IBaseComponent> update)
        {

        }
    }

}
