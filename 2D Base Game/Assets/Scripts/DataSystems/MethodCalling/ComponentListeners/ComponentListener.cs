using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings {
    public class ComponentListener<T>
    {
        string id;
        Action<T, object[]> action;
        object[] args;

        public ComponentListener(string id,Action<T, object[]> action, object[] args)
        {
            this.id = id;
            this.action = action;
            this.args = args;
        }

        public ComponentListener(string id, Action<T> action)
        {
            this.id = id;
            this.action = (t,args) => action(t);
            args = new object[0];
        }

        public void ResetAction(Action<T> action)
        {
            this.action = (t, args) => action(t);
        }

        public void ChangeArgs(object[] args)
        {
            this.args = args;
        }

        public void Invoke(T t)
        {
            action.DynamicInvoke(t, args);
        }

        public void Invoke(T t, object[] args)
        {
            action.DynamicInvoke(t, args);
        }
    }
}