using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace HighKings
{
    public class StaticMethodCallType
    {
        MethodInfo method;

        public StaticMethodCallType()
        {
        }

        public StaticMethodCallType(StaticMethodCallType parent)
        {
            method = parent.method;
        }

        public void Invoke()
        {
            method.Invoke(null, null);
        }

        public void Invoke(object[] pars)
        {
            method.Invoke(null, pars);
        }
    }
}

