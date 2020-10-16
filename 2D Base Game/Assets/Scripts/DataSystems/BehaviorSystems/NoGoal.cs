using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class NoGoal : IGoal
    {




        public bool Achieved(Entity source, Entity target)
        {
            return false;
        }

        public void Assign(Entity source, Entity target)
        {
            source.GetComponent<Conscious>("Conscious").SetGoals(this, source);
        }

        public void AssignAchievedAction(EntityAction action)
        {
        }

        public void AssignCancelAction(EntityAction action)
        {
        }

        public void AssignOnAssignedAction(EntityAction action)
        {
        }

        public void Cancel(Entity source, Entity target)
        {
            Debug.LogError("CANNOT CANCEL NOGOAL");
        }

        public IGoal GetParentGoal()
        {
            return default;
        }

        public Entity GiveTarget(Entity source, Entity initial_target)
        {
            return source;
        }

        public string id()
        {
            return "NoGoal";
        }

        public bool IsAchievable(Entity source, Entity target)
        {
            return true;
        }

        public void OnAchieved(Entity source, Entity target)
        {
        }

        public int Preference(Entity source, Entity target)
        {
            return 1;
        }

        public int Preference(Entity source, Entity target, IGoal prev_goal)
        {
            return 1;
        }

        public void UnAssignAchievedAction(EntityAction action)
        {
        }

        public void UnAssignCancelAction(EntityAction action)
        {
        }

        public void UnAssignOnAssignedAction(EntityAction action)
        {
        }
    }
}

