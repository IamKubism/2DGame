using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class HealthSystem
    {
        ComponentSubscriberSystem<BaseStatistic> healths;

        public HealthSystem()
        {
            healths = MainGame.instance.GetSubscriberSystem<BaseStatistic>("Health");

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
            target.GetComponent<BaseStatistic>("Health").IncrementCurr(-source.GetComponent<BaseStatistic>("Strength").curr_value);
        }

        public static void TakeDamage(Entity target, int amount)
        {
            target.GetComponent<BaseStatistic>("Health").IncrementCurr(-amount);
        }
    }
}

