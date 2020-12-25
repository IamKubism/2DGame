using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Psingine
{
    public interface IBaseComponent
    {
        //TODO: give these int ids for better indexing in component lists?
        SubscriberEvent subscriber { get; set; }
        bool Trigger(Event e);
        void CopyData(IBaseComponent comp);
    }

    public interface IAIComponent
    {
        void Pref(Event e);
        void Achieved(Event e);
    }
}

