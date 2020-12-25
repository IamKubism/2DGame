using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Psingine {
    public class ComponentListener
    {
        string id;
        Action<IBaseComponent, object[]> action;
        object[] args;

        public ComponentListener(string id,Action<IBaseComponent, object[]> action, object[] args)
        {
            this.id = id;
            this.action = action;
            this.args = args;
        }

        public ComponentListener(string id, Action<IBaseComponent> action)
        {
            this.id = id;
            this.action = (t,args) => action(t);
            args = new object[0];
        }

        public void ResetAction(Action<IBaseComponent> action)
        {
            this.action = (t, args) => action(t);
        }

        public void ChangeArgs(object[] args)
        {
            this.args = args;
        }

        public void Invoke(IBaseComponent t)
        {
            action.DynamicInvoke(t, args);
        }

        public void Invoke(IBaseComponent t, object[] args)
        {
            action.DynamicInvoke(t, args);
        }
    }
}