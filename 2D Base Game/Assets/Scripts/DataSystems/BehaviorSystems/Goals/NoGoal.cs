using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class NoGoal : IGoal
    {
        public NoGoal()
        {
        }

        public bool Achieved(Entity source, Entity target)
        {
            return false;
        }

        public void Assign(Entity source, Entity target)
        {
        }

        public void RegisterAchievedAction(EntityAction action)
        {
        }

        public void RegisterCancelAction(EntityAction action)
        {
        }

        public void RegisterOnAssignedAction(EntityAction action)
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

        public Entity PassTarget(Entity source, Entity initial_target)
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

        public int Adversion(Entity source, Entity target)
        {
            return 1;
        }

        public int Adversion(Entity source, Entity target, IGoal prev_goal)
        {
            return 1;
        }

        public void UnRegisterAchievedAction(EntityAction action)
        {
        }

        public void UnRegisterCancelAction(EntityAction action)
        {
        }

        public void UnRegisterOnAssignedAction(EntityAction action)
        {
        }

    }
}

