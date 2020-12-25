using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public interface IEventComponent
    {
        string param_name {get; set;}
        void RegisterFunc();
        void UnRegisterFunc();
        void Call();
    }
}

