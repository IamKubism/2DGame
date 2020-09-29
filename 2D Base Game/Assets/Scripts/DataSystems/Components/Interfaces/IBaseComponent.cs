using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public interface IBaseComponent
    {
        void SetListener<T>(SubscriberEvent<T> subscriber) where T : IBaseComponent;
    }
}

