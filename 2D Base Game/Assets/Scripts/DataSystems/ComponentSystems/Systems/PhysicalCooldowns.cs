using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    /// <summary>
    /// TODO
    /// System that manages the cooldowns of all physical actions of entities, and calls their action queues. This is a generalization of the movement system to any action
    /// </summary>
    public class PhysicalCooldowns : IUpdater
    {
        class QueuedActions
        {
            List<ItemVector<Entity, EntityAction>> next_actions;
            FloatMinMax curr_progress;

            public QueuedActions()
            {
                next_actions = new List<ItemVector<Entity, EntityAction>>();
                curr_progress = new FloatMinMax();
            }

            public void InvokeNext(Entity source)
            {
                next_actions[0].b.Invoke(source, next_actions[0].a);
                curr_progress.Reset();
            }
        }

        Dictionary<Entity, QueuedActions> action_queue;


        public void Update(float dt)
        {

        }
    }

}
