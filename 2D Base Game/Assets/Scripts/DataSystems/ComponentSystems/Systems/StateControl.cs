using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Psingine
{
    public class StateControl
    {
        Dictionary<string, AIState> state_names;
        Dictionary<string, Observable> observation_names;
        List<AIState> all_states;
        List<Observable> all_observations;
        public static StateControl instance;

        public StateControl()
        {
            if(instance == null)
            {
                instance = this;
            }
            state_names = new Dictionary<string, AIState>();
            all_states = new List<AIState>();
            all_observations = new List<Observable>();
            observation_names = new Dictionary<string, Observable>();
            all_states.Add(new StateTesting.MeleeAttackAI());
            state_names.Add(all_states.Last().name, all_states.Last());
        }

        public AIState CreateNewState(JProperty data)
        {
            if (state_names.TryGetValue(data.Name, out AIState state))
            {
                Debug.LogWarning($"State: {data.Name} already exists");
                return state;
            }

            AIState new_state = new AIState(data);
            state_names.Add(new_state.name, new_state);
            new_state.id = all_states.Count;
            all_states.Add(new_state);
            
            return new_state;
        }

        public void ObserveAndTransition(Event e)
        {

        }

        public void TransitionEntity(Event e, AIState state)
        {
            Entity to_transition = e.GetParamValue<Entity>(EventParams.invoking_entity);
            if (to_transition.HasComponent("CurrState"))
            {
                
            }
        }
    }

    /// <summary>
    /// A class that represents any information an entity might use to decide what AI state it should transition to
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Observable
    {
        public int id;

        [JsonProperty]
        public string name;

        [JsonProperty]
        public string value_name;

        Event measurement; // TODO: Have to find some way of calculating the observation value based on the world state
        Event propagator; // TODO: Need some way of propogating information to other entities, so that they can interpret it

        public Observable(string name, int id)
        {
            this.name = name;
            this.id = id;
            value_name = "observable_" + name;
        }

        public Observable(JProperty obj)
        {
            name = obj.Name;
            value_name = "observable_" + name;

            if (obj.Value.Type != JTokenType.Object)
            {
                measurement = Event.NewEvent("NullEvent");
                propagator = Event.NewEvent("NullEvent");
                return;
            }

            if (((JObject)obj.Value).TryGetValue(nameof(measurement), out JToken obs_event))
            {
                measurement = Event.NewEvent(obs_event.Value<string>());
            }
            else
            {
                measurement = Event.NewEvent("NullEvent");
            }

            if (((JObject)obj.Value).TryGetValue(nameof(propagator), out JToken prop_event))
            {
                propagator = Event.NewEvent(obs_event.Value<string>());
            }
            else
            {
                propagator = Event.NewEvent("NullEvent");
            }
        }

        public (float, Event) Observe(Entity observer, Entity info_entity)
        {
            Event obs_event = new Event(measurement);

            float val = new Event(measurement).Invoke<float>(observer, value_name);
            return (val, obs_event);
        }
    }

    /// <summary>
    /// This is kind of like a new creation of the "job" thing we had earlier, but its generalized to anything. Gives an entity a stateless AI procedure to execute
    /// </summary>
    public class AIState
    {
        /// <summary>
        /// Event that is called when an entity gets this state
        /// </summary>
        public Event transition_event;

        /// <summary>
        /// Event that is called when the state's execution needs to be stopped, but not completed
        /// </summary>
        public Event interupt_event;

        /// <summary>
        /// Event that progresses the state's execution
        /// </summary>
        public Event continue_event;

        /// <summary>
        /// Event that is called on the completion of the event
        /// </summary>
        public Event done_event;

        public string name;
        public int id; //TODO: for faster querying we can set ids in an array of all states, at runtime we give the states their ids and then give all the references these ids instead of names of the transition states

        public AIState()
        {
            transition_event = Event.NewEvent("NullEvent");
            name = "NullState";
        }

        public AIState(string name, string event_name)
        {
            this.name = name;
            transition_event = Event.NewEvent(event_name);
        }

        public AIState(AIState prototype)
        {
            transition_event = new Event(prototype.transition_event);
            name = prototype.name;
        }

        public AIState(JProperty data)
        {
            name = data.Name;

            if (data.Value.Type != JTokenType.Object)
            {
                transition_event = Event.NewEvent("NullEvent");
                interupt_event = Event.NewEvent("NullEvent");
                continue_event = Event.NewEvent("NullEvent");
                done_event = Event.NewEvent("NullEvent");
                return;
            }

            if (((JObject)data.Value).TryGetValue(nameof(transition_event), out JToken t_event))
            {
                transition_event = Event.NewEvent(t_event.Value<string>());
            }
            else
            {
                transition_event = Event.NewEvent("NullEvent");
            }

            if (((JObject)data.Value).TryGetValue(nameof(interupt_event), out JToken i_event))
            {
                interupt_event = Event.NewEvent(i_event.Value<string>());
            }
            else
            {
                interupt_event = Event.NewEvent("NullEvent");
            }

            if (((JObject)data.Value).TryGetValue(nameof(continue_event), out JToken c_event))
            {
                continue_event = Event.NewEvent(c_event.Value<string>());
            }
            else
            {
                continue_event = Event.NewEvent("NullEvent");
            }

            if (((JObject)data.Value).TryGetValue(nameof(continue_event), out JToken d_event))
            {
                done_event = Event.NewEvent(d_event.Value<string>());
            }
            else
            {
                done_event = Event.NewEvent("NullEvent");
            }
        }
    }
}

/* Planning:
 *  Transition: 
 *      Start at current state
 *          -> Observes all observables that it can
 *              Certain components are the ones that observe
 *              World keeps track of observables: passes which ones the entity can observe
 *                  Ex: If there is no sound in range of the observer then the world doesn't pass that (so components don't process it)
 *          -> States process observable data
 *              -> Set preference
 *          -> Get the goal from preference data
 *              -> Set state + call transition event
 *
 *  How does preference work?
 *      Preference values are based on observables and the current state
 *      Observables have a value somewhere and is processed by the entity to get the full value to be computed on
 *      States process the observable values to get preference using some function on 
 *          Does this function depend on current state?
 *              This could be more dynamic but maybe overly complicated?
 *              Actually I think this might be necessary (ex: entity shouldn't try to eat while in combat).
 * 
 * 
 *  Where are observables kept?
 *      Maybe in world?
 *      Ex: Sound Amplitude
 *          Have some set of sound amplitude values for every tile?
 *          This is distinct from the tile, its just stored somewhere?
 *          Or is sound an entity?
 *              That gets weird because adding sound amplitudes could get weird?
 *              BUT:
 *                  Could reduce memory costs, and could include more information for a sound in a particular place (frequency decomposition ect)
 *                  Could have an area of effect, this gives entities in range some information for their 
 */