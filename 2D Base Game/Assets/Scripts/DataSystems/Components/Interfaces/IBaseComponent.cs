using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace HighKings
{
    public interface IBaseComponent
    {
        void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent;
        bool Trigger(Event e);
    }

    //public static class BaseCompSys
    //{
    //    static BindingFlags field_flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;

    //    public static void SetListener(object c, SubscriberEvent<T> subscriber)
    //    {
    //        FieldInfo f = c.GetType().GetField("listener", field_flags);
    //    }
    //}
}

