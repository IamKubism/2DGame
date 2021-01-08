using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public interface IBehavior
    {
        /// <summary>
        /// The way that you can add two computed values together on an entity
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        float Sum(float f1, float f2);

        /// <summary>
        /// Compute a number with the information stored in an entity
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        float CalculateOnEntity(Entity e);
    }
}

