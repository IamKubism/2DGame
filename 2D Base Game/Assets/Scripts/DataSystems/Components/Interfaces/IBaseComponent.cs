using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public interface IBaseComponent
    {
        /// <summary>
        /// Not sure if this is actually gonna be used
        /// </summary>
        /// <returns></returns>
        bool computable();
        string ComponentType();
        void OnUpdateState();
        void RegisterUpdateAction(Action<IBaseComponent> update);
    }
}

