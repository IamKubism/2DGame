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

    public static class BaseComponentUtils
    {
        public static ComponentSubscriberSystem GetSubscriberSystem(string component_name)
        {
            return MainGame.instance.GetSubscriberSystem(component_name);
        }

        public static ComponentSubscriberSystem GetSubscriberSystem<T>() where T : IBaseComponent
        {
            return MainGame.instance.GetSubscriberSystem<T>();
        }
    }
}

