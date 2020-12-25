using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public interface IRequirement<T>
    {
        bool Satisfies(T t);
    }
}

