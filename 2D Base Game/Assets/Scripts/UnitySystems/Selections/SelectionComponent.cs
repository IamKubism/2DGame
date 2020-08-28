using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class SelectionComponent : IBaseComponent
    {
        public int priority;

        public SelectionComponent()
        {

        }

        public SelectionComponent(SelectionComponent s)
        {
            this.priority = s.priority;
        }
        
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

