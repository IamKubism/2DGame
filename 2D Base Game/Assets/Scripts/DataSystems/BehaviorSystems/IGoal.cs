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
        Entity GiveTarget(Entity source, Entity initial_target);
        IGoal GetParentGoal();
        bool IsAchievable(Entity source, Entity target);
        void OnAchieved(Entity source, Entity target);
        int Preference(Entity source, Entity target);
        int Preference(Entity source, Entity target, IGoal prev_goal);
        void AssignAchievedAction(EntityAction action);
        void AssignCancelAction(EntityAction action);
        void AssignOnAssignedAction(EntityAction action);
        void UnAssignAchievedAction(EntityAction action);
        void UnAssignCancelAction(EntityAction action);
        void UnAssignOnAssignedAction(EntityAction action);
    }
}
