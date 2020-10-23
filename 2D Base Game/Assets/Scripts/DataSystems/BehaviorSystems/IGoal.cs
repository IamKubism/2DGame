using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    /// <summary>
    /// Might be conflating goals with tasks here
    /// </summary>
    public interface IGoal
    {
        /// <summary>
        /// TODO: Figure out the way this should be called
        /// </summary>
        /// <returns></returns>
        string id();
        bool Achieved(Entity source, Entity target);
        void Cancel(Entity source, Entity target);
        void Assign(Entity source, Entity target);
        Entity PassTarget(Entity source, Entity initial_target);
        bool IsAchievable(Entity source, Entity target);
        int Adversion(Entity source, Entity target);
        int Adversion(Entity source, Entity target, IGoal prev_goal);
    }
}
