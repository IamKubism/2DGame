using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class HealthSystem
    {
        ComponentSubscriber<BaseStatistic> healths;

        public HealthSystem()
        {
            healths = MainGame.instance.GetSubscriberSystem<BaseStatistic>("health");

            healths.RegisterOnAdded((entities) =>
            {
                healths.SubscribeAfterAction(entities, OnDeath, "CheckDeath");
            });
        }

        public bool CheckDeath(BaseStatistic health)
        {
            return health.curr_value <= 0;
        }

        public void OnDeath(Entity e, BaseStatistic health)
        {
            if (CheckDeath(health))
            {
                e.RemoveFromAllSubscribers();
                EntityManager.instance.DestroyEntity(e);
            }
        }

        public static void Attack(Entity source, Entity target)
        {
            source.GetComponent<BaseStatistic>("Health").IncrementCurr(-10);
        }
    }
}

