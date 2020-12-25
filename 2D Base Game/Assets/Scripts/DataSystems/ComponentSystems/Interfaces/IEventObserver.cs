using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public interface IEventObserver
    {
        bool Observe(Event e);
    }
}

