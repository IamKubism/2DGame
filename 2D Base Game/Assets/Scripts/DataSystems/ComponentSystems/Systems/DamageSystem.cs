using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public enum DamageEvents
    {
        Attack,
        DoDamage,
        TakeDamage
    }


    public enum DamageParams
    {
        total_damage,
        blunt_damage,
        blunt_resistance,
        damage_type,
        damage_attacker,
        damage_defender
    }

    public class AttackSystem
    {
        public static AttackSystem instance;

        public AttackSystem()
        {
            if(instance == null)
            {
                instance = this;
            }

            EventManager.instance.GetPrototype(DamageEvents.Attack.ToString()).AddUpdate(MakeAndTakeAttackDamage, 100);
            //EventManager.instance.GetPrototype(DamageEvents.TakeDamage.ToString()).AddUpdate(ComputeAllDamage, 100);
        }

        public void MakeAndTakeAttackDamage(Event e)
        {
            Debug.Log("Attacking");
            if (!e.TryGetRelevantEntity(EventParams.invoking_entity.ToString(), out Entity attacker))
            {
                Debug.LogError("No attacking entity");
                return;
            }
            if (!attacker.TryGetComponent(out ActiveAttack attack))
            {
                Debug.LogError("Could not find attack");
                return;
            }
            if (!e.TryGetRelevantEntity(EventParams.target_entity.ToString(), out Entity defender))
            {
                Debug.LogError("No defending entity");
                return;
            }

            Event damage_event = Event.NewEvent(BaseEvents.make_damage);
            damage_event.TryGetTempEntity(EventParams.damage_entity.ToString(), out Entity damage_entity, true);
            damage_event.TryAddRelevantEntity(EventParams.attack_entity.ToString(), attack.attack);
            damage_event.TryAddRelevantEntity(EventParams.attacker_entity.ToString(), attacker);

            EventManager.instance.InvokeEvent(damage_event);

            Event take_damage = Event.NewEvent(BaseEvents.take_damage);
            take_damage.AddTempEntity(EventParams.damage_entity.ToString(), damage_entity, out damage_entity);
            take_damage.TryAddRelevantEntity(EventParams.target_entity.ToString(), defender);

            EventManager.instance.InvokeEvent(take_damage);


            //Entity attacker = e.GetParamValue<Entity>(EventParams.invoking_entity);
            //List<Entity> defenders = new List<Entity>();

            //foreach(Entity ent in e.GetParamValue<HashSet<Entity>>(EventParams.targets))
            //{
            //    if (ent.HasComponent(nameof(Health)) && ent != attacker)
            //    {
            //        defenders.Add(ent);
            //    }
            //}

            //Event do_damage = Event.NewEvent(DamageEvents.DoDamage);
            //do_damage.Invoke(attacker);

            //foreach(Entity ent in defenders)
            //{
            //    Event.NewEvent(do_damage, DamageEvents.TakeDamage).Invoke(ent);
            //}
        }

        //public void ComputeAllDamage(Event e)
        //{
        //    HashSet<string> types = e.GetParamValue<HashSet<string>>(DamageParams.damage_type);
        //    int total_damage = 0;
        //    foreach(string dam_type in types)
        //    {
        //        total_damage += (int)e.GetParamValue<DiceGroup>(dam_type);
        //    }
        //    e.SetParamValue(DamageParams.total_damage, total_damage, (d1, d2) => { return d1 + d2; });
        //}
    }
}

