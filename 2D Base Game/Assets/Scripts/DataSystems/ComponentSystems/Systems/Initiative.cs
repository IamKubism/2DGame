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
    public class Initiative : IUpdater
    {
        Dictionary<Entity, List<ComponentSubscriberSystem>> updates;
        Dictionary<Entity, FloatMinMax> entities_to_update;
        public void Update(float dt)
        {
            throw new NotImplementedException();
        }
    }

}
