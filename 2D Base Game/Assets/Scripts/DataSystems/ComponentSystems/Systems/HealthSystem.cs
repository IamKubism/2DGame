using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class HealthSystem
    {
        ComponentSubscriberSystem healths;

        public HealthSystem()
        {
            healths = MainGame.instance.GetSubscriberSystem<Health>();

            healths.RegisterOnAdded((entities) =>
            {
                healths.SubscribeAfterAction(entities,(e,b) => { OnDeath(e, (Health)b); }, "CheckDeath");
            });
        }

        public bool CheckDeath(Health health)
        {
            return health.curr_value <= 0;
        }

        public void OnDeath(Entity e, Health health)
        {
            if (CheckDeath(health))
            {
                e.RemoveFromAllSubscribers();
                EntityManager.instance.DestroyEntity(e);
            }
        }

        public static void Attack(Entity source, Entity target)
        {
        }

        public static void TakeDamage(Entity target, int amount)
        {
            target.GetComponent<BaseStatistic>("Health").IncrementCurr(-amount);
        }
    }
}

