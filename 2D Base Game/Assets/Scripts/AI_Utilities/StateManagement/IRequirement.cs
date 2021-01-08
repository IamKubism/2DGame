using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public interface IRequirement<T>
    {
        bool Satisfies(T t);
    }
}

