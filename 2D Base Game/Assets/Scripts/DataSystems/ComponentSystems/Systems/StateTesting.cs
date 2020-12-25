using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Psingine
{
    public enum AIStateTestingEvents { MeleeTargetSelect, MeleeAttackEvent, Combat }
    public enum CombatParams { AttackTarget }

    /// <summary>
    /// Class for some prototyping of how ai will work
    /// </summary>
    public class StateTesting
    {
        public class MeleeAttackAI : AIState
        {
            FloatMinMax cool_down;

            public MeleeAttackAI()
            {
                continue_event = Event.NewEvent(AIStateTestingEvents.MeleeAttackEvent);
                continue_event.AddUpdate(StatelessForm, 100);
                transition_event = Event.NewEvent(AIStateTestingEvents.MeleeTargetSelect);
                transition_event.AddUpdate(SetAttackTarget, 100);
                name = "MeleeState";
                cool_down = new FloatMinMax(-1f, 1f, -0.1f);
                EventManager.instance.GetPrototype(AIStateTestingEvents.Combat.ToString()).AddUpdate(SetAttackTarget, 100);
            }

            void RemoveCombat(Event e)
            {

            }

            void SetAttackTarget(Event ev)
            {
                ev.GetParamValue<Entity>(EventParams.invoking_entity).FullSetComponent(nameof(AttackTarget), new AttackTarget(ev.GetParamValue<HashSet<Entity>>(EventParams.targets).First().entity_string_id));
            }

            void StatelessForm(Event e)
            {
                //FIXME This is shit and should be put elsewhere, its trying to encapsulate a cooldown type thing so the entity can't constantly attack
                if (cool_down.IsUnderMin() == false)
                {
                    cool_down.curr += e.GetParamValue<float>(EventParams.time_dt);
                    return;
                }

                Entity invoker = e.GetParamValue<Entity>(EventParams.invoking_entity.ToString());

                if (cool_down.IsOverMax())
                {
                    cool_down.Reset(1f, -0.1f);
                    cool_down.curr = -1f;
                    done_event.Invoke(invoker);
                    return;
                }

                if (!invoker.TryGetComponent(out AttackTarget targ_name))
                {
                    return;
                }

                Entity target = EntityManager.instance.GetEntityFromId(targ_name.target);

                if (MathFunctions.SqrDist(invoker.GetComponent<Position>().p, target.GetComponent<Position>().p) <= 2)
                {
                    Event do_damage = Event.NewEvent(e, DamageEvents.DoDamage.ToString());
                    do_damage.Invoke(invoker);
                    cool_down.curr = 0f;
                }
                else if (Movers.instance.TryGetTarget(invoker, out Entity move_to) == false || MathFunctions.SqrDist(move_to.GetComponent<Position>().p, target.GetComponent<Position>().p) > 2) //TODO? maybe shouldn't check target always
                {
                    Event move_to_target = Event.NewEvent(MovementEvents.SetMoveDestination.ToString());
                    HashSet<Entity> tiles = new HashSet<Entity>(World.instance.GetTilesAroundEntity(target, 0, 2));
                    move_to_target.SetParamValue(EventParams.targets.ToString(), tiles, (h1, h2) => { return h2; });
                    move_to_target.Invoke(invoker);
                }
            }

            float AttackPref(Event e)
            {
                float f = 1f;
                return f;
            }
        }
    }
}

