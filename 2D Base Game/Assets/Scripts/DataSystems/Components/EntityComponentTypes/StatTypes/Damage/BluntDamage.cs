using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Psingine
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BluntDamage : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        public Dice d;

        public BluntDamage()
        {
            d = new Dice();
        }

        public BluntDamage(BluntDamage prot)
        {
            d = new Dice(prot.d);
        }

        public BluntDamage(JObject p)
        {
            d = new Dice();
            if(p["d"] != null)
            {
                d = new Dice(p.Value<string>("d"));
            }
        }

        /// <summary>
        /// TODO: This kind of thing could be for sure defined in some text document
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool Trigger(Event e)
        {
            bool eval = true;
            if (e.type == "DoDamage")
            {
                e.AddUpdate(AddDamageType, 10);
            }
            return eval;
        }

        void AddDamageType(Event e)
        {
            e.SetParamValue(DamageParams.blunt_damage, new DiceGroup(d), (d1, d2) => { return d1 + d2; });
            e.SetParamValue(DamageParams.damage_type, new HashSet<string> { DamageParams.blunt_damage.ToString() }, (d1, d2) => { d1.UnionWith(d2); return d1; });
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new NotImplementedException();
        }
    }

    //TODO: There should be a better way to find if a system should interact with an event
    public class MakeBluntDamage : IEventObserver
    {
        public static MakeBluntDamage instance;
        EventAlterer my_alterer;

        public MakeBluntDamage()
        {
            if(instance == null)
            {
                instance = this;
            }
            my_alterer = new EventAlterer(Compute, 100, new string[] { BaseEvents.make_damage.ToString() });

            EventManager.instance.AddObserver(new string[]{ BaseEvents.make_damage.ToString() }, instance);
        }

        public bool Observe(Event e)
        {
            e.AddUpdate(Compute, 20);

            return true;
        }

        public void Compute(Event e)
        {
            e.TryGetTempEntity(EventParams.damage_entity.ToString(), out Entity damage, true);

            if (e.TryGetRelevantEntity(EventParams.attack_entity.ToString(), out Entity attack))
            {
                if (attack.TryGetComponent(out BluntDamage dam))
                {
                    BluntAttackDamage bdam = (BluntAttackDamage)damage.TryAddComponent(nameof(BluntAttackDamage), new BluntAttackDamage(dam));
                }
            }
            if (e.TryGetRelevantEntity(EventParams.attacker_entity.ToString(), out Entity attacker))
            {
                int mod = 0;
                if (attacker.TryGetComponent(out Strength strength))
                {
                    mod = strength.value;
                }
                if (damage.TryGetComponent(out BluntAttackDamage dam))
                {
                    dam.d *= mod;
                }

            }
        }
    }

    public class CalculateBluntResistance : IEventObserver
    {
        public static CalculateBluntResistance instance;
        EventAlterer my_alterer;

        public CalculateBluntResistance()
        {
            if (instance == null)
            {
                instance = this;
            }
            my_alterer = new EventAlterer(Compute, 100, new string[] { BaseEvents.take_damage.ToString() });

            EventManager.instance.AddObserver(new string[] { BaseEvents.take_damage.ToString() }, instance);
        }

        public bool Observe(Event e)
        {
            e.AddUpdate(my_alterer);
            return true;
        }

        /// <summary>
        /// If there is blunt damage, this ensures it gets reduced by a number. TODO: Probably there is a better way to check if it should do this but currently it seems chill
        /// </summary>
        /// <param name="e"></param>
        public void Compute(Event e)
        {
            if (e.TryGetTempEntity(EventParams.damage_entity.ToString(), out Entity damage_entity))
            {
                if (e.TryGetRelevantEntity(EventParams.target_entity.ToString(), out Entity defender))
                {
                    if (defender.TryGetComponent(out BluntResistance res))
                    {
                        if (damage_entity.TryGetComponent(out BluntAttackDamage bdam))
                        {
                            bdam.d.AddConst(-res.res_val);
                        }
                    }
                }
            }
        }
    }

    public class CalculateBluntDamageDone : IEventObserver
    {
        public static CalculateBluntDamageDone instance;
        EventAlterer my_alterer;

        public CalculateBluntDamageDone()
        {
            if (instance == null)
            {
                instance = this;
            }
            my_alterer = new EventAlterer(Compute, 100, new string[] { BaseEvents.take_damage.ToString() });
            EventManager.instance.AddObserver(new string[] { BaseEvents.take_damage.ToString() }, instance);
        }

        public void Compute(Event e)
        {
            if (e.TryGetRelevantEntity(EventParams.target_entity.ToString(), out Entity defender))
            {
                if (e.TryGetTempEntity(EventParams.damage_entity.ToString(), out Entity damage_ent))
                {
                    if (damage_ent.TryGetComponent(out BluntAttackDamage dam))
                    {
                        if (defender.TryGetComponent(out Health h))
                        {
                            h.IncrementHealth(-dam.d);
                        }
                    }
                }
            }
        }

        public bool Observe(Event e)
        {
            e.AddUpdate(my_alterer);
            return true;
        }
    }

    public class Strength : IBaseComponent
    {
        public SubscriberEvent subscriber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int value;

        public Strength()
        {
            value = 0;
        }

        public Strength(Strength prot)
        {
            value = prot.value;
        }

        public Strength(JObject p)
        {
            if (p["value"] != null)
            {
                value = p["value"].Value<int>();
            }
        }

        public void CopyData(IBaseComponent comp)
        {
        }

        public bool Trigger(Event e)
        {
            throw new NotImplementedException();
        }
    }

    public class BluntAttackDamage : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }
        public Dice d;

        public BluntAttackDamage(BluntDamage bd)
        {
            d = new Dice(bd.d);
        }

        public void CopyData(IBaseComponent comp)
        {
            d = new Dice(((BluntAttackDamage)comp).d);
        }

        public bool Trigger(Event e)
        {
            if (e.HasTag(BaseEvents.take_damage))
            {
                e.AddUpdate(AddDamage, 10);
            }
            return true;
        }

        void AddDamage(Event e)
        {
            Entity damage_entity = e.GetParamValue<Entity>(EventParams.damage_entity);
            if (damage_entity.TryGetComponent(out TotalDamage dam))
            {
                dam.total_damage += d;
            } else
            {
                damage_entity.TryAddComponent(nameof(TotalDamage), new TotalDamage(d));
            }
        }
    }

    public class TotalDamage : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        public int total_damage;

        public TotalDamage(Dice d)
        {
            total_damage = d;
        }

        public void CopyData(IBaseComponent comp)
        {
            total_damage = ((TotalDamage)comp).total_damage;
        }

        public bool Trigger(Event e)
        {
            if (e.HasTag(BaseEvents.take_damage))
            {
                e.AddUpdate(DoDamage, 100);
            }
            return true;
        }

        //Coupling tho
        void DoDamage(Event e)
        {
            Entity to_be_damaged = e.GetParamValue<Entity>(EventParams.target_entity);
            if(to_be_damaged.TryGetComponent(out Health health))
            {
                health.IncrementHealth(-total_damage);
            } else
            {
                Debug.LogWarning("Tried to damage health of entity with no health");
            }
        }

    }
}

