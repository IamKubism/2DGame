using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace HighKings
{
    public interface IBaseComponent
    {
        SubscriberEvent subscriber { get; set; }
        bool Trigger(Event e);
    }
}

