using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn), Serializable]
    public class RenderComponent : IBaseComponent
    {
        [JsonObject]
        public struct RenderState
        {
            [JsonProperty]
            public string layer_name;

            [JsonProperty]
            public string sprite_name;
        }

        [JsonProperty]
        public SerializableVector3 position;

        [JsonProperty]
        RenderState state;

        public string sprite_name
        {
            get
            {
                return state.sprite_name;
            }
        }

        public string layer_name
        {
            get
            {
                return state.layer_name;
            }
        }

        delegate void ActionByRef<T1, T2>(ref T1 p1, ref T2 p2);
        delegate void ActionByRef<T1, T2, T3>(ref T1 p1, ref T2 p2, ref T3 p3);
        public delegate void MyDelegate();

        void UpdateState<TP1>(ActionByRef<RenderState, TP1> proc, ref TP1 p1)
        { proc(ref state, ref p1); }
        void UpdateState<TP1, TP2>(ActionByRef<RenderState, TP1, TP2> proc, ref TP1 p1, ref TP2 p2)
        { proc(ref state, ref p1, ref p2); }

        public RenderComponent()
        {
            state = new RenderState();
        }

        public RenderComponent(RenderComponent r)
        {
            state = new RenderState
            {
                sprite_name = r.sprite_name,
                layer_name = r.layer_name
            };
        }

        public RenderComponent(Vector3 position, string layer_name, string sprite_name)
        {
            this.position = position;
            state = new RenderState { layer_name = layer_name, sprite_name = sprite_name };
        }

        public RenderComponent(string entity_id, Vector3 position, RenderComponent prot)
        {
            this.position = position;
            state = new RenderState { layer_name = prot.state.layer_name, sprite_name = prot.state.sprite_name };
        }

        public override string ToString()
        {
            return $"sprite: {state.sprite_name} layer: {state.layer_name} position: ({position.ToString()})";
        }

        public void SetStateValues(object vals)
        {
            RenderComponent comp = (RenderComponent)vals;
            UpdateState((ref RenderState s, ref RenderComponent over) =>
                {
                    s.sprite_name = over.state.sprite_name;
                    s.layer_name = over.state.layer_name;
                }, ref comp);
        }

        public void CopyData(object source)
        {
            RenderComponent s = (RenderComponent)source;
        }

        public object Maker(Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }

        public bool computable()
        {
            return false;
        }

        public string ComponentType()
        {
            return "RenderComponent";
        }

        public void OnUpdateState()
        {
            throw new NotImplementedException();
        }

        public void RegisterUpdateAction(Action<IBaseComponent> update)
        {

        }
    }
}



