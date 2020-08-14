using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class SelectionComponent : MonoBehaviour, IBaseComponent
    {
        public string entity_string_id;
        public string display_name;
        public string type_id;
        public int priority;

        public string ComponentType()
        {
            return "SelectionComponent";
        }

        public bool computable()
        {
            return false;
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

