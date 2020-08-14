using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public interface IBehavior
    {
        float Sum(float f1, float f2);
        float CalculateOnEntity(Entity e);
    }
}

