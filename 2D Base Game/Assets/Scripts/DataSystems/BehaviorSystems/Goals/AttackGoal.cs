using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class AttackGoal : IGoal
    {
        public Entity attack;

        public bool Achieved(Entity source, Entity target)
        {
            throw new System.NotImplementedException();
        }

        public int Adversion(Entity source, Entity target)
        {
            throw new System.NotImplementedException();
        }

        public int Adversion(Entity source, Entity target, IGoal prev_goal)
        {
            throw new System.NotImplementedException();
        }

        public void Assign(Entity source, Entity target)
        {
            throw new System.NotImplementedException();
        }

        public void Cancel(Entity source, Entity target)
        {
            throw new System.NotImplementedException();
        }

        public string id()
        {
            throw new System.NotImplementedException();
        }

        public bool IsAchievable(Entity source, Entity target)
        {
            throw new System.NotImplementedException();
        }

        public Entity PassTarget(Entity source, Entity initial_target)
        {
            throw new System.NotImplementedException();
        }
    }
}

