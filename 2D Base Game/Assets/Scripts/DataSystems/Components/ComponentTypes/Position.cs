﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Position : IBaseComponent
    {
        [JsonObject(MemberSerialization.OptIn)]
        public struct State
        {
            [JsonProperty]
            public int x, y, z;
            [JsonProperty]
            public SerializableVector3 disp_pos;
        }

        [JsonProperty]
        public State state;

        /// <summary>
        /// Identification of the thing that moves
        /// </summary>
        [JsonProperty]
        public readonly string entity_string_id;

        /// <summary>
        /// Coordinates of this position
        /// </summary>
        public int x
        {
            get
            {
                return state.x;
            }
        }

        /// <summary>
        /// Coordinates of this position
        /// </summary>
        public int y
        {
            get
            {
                return state.y;
            }
        }


        /// <summary>
        /// Coordinates of this position
        /// </summary>
        public int z
        {
            get
            {
                return state.z;
            }
        }

        public SerializableVector3 disp_pos
        {
            get
            {
                return state.disp_pos;
            }
        }

        delegate void ActionByRef<T1, T2>(ref T1 p1, ref T2 p2);
        delegate void ActionByRef<T1, T2, T3>(ref T1 p1, ref T2 p2, ref T3 p3);

        void UpdateState<TP1>(ActionByRef<State, TP1> proc, ref TP1 p1)
        { proc(ref state, ref p1); OnUpdateState(); }
        void UpdateState<TP1, TP2>(ActionByRef<State, TP1, TP2> proc, ref TP1 p1, ref TP2 p2)
        { proc(ref state, ref p1, ref p2); OnUpdateState(); }

        public int[] p
        {
            get
            {
                return new int[3] { x, y, z };
            }
        }

        public Vector3 t_r
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        /// <summary>
        /// Default constuctor, for json serialization only
        /// </summary>
        public Position()
        {

        }

        /// <summary>
        /// Creating a new tile position from scratch
        /// </summary>
        /// <param name="entity_id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Position(string entity_id, int x, int y, int z)
        {
            entity_string_id = entity_id;
            state = new State
            {
                x = x,
                y = y,
                z = z,
                disp_pos = new SerializableVector3(x, y, z)
            };
        }

        public Position(int x, int y, int z)
        {
            state = new State
            {
                x = x,
                y = y,
                z = z,
                disp_pos = new SerializableVector3(x, y, z)
            };
        }

        /// <summary>
        /// Gets the vector of p and scales it by f
        /// </summary>
        /// <param name="f"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Vector3 operator *(float f, Position p)
        {
            return f * (Vector3)p.disp_pos;
        }

        /// <summary>
        /// Finds the square distance between two tile positions
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int SqrDist(Position p1, Position p2)
        {
            return MathFunctions.SqrDist(p1.p, p2.p);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Finds if two tile positions have the same integer coordinates
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator ==(Position p1, Position p2)
        {
            return (p1.x == p2.x) && (p1.y == p2.y) && (p1.z == p2.z);
        }

        /// <summary>
        /// Finds if two tile positions do not have the same integer coordinates
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(Position p1, Position p2)
        {
            return !((p1.x == p2.x) && (p1.y == p2.y) && (p1.z == p2.z));
        }

        public override string ToString()
        {
            return $"{entity_string_id} at ({x},{y},{z}) with displaced vector {disp_pos.ToString()}";
        }

        /// <summary>
        /// Shifts something to the point (x,y,z)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void UpdateToNewPoint(int x, int y, int z)
        {
            int[] px = new int[3] { x, y, z };
            UpdateState((ref State s, ref int[] r) =>
            {
                s.x = r[0];
                s.y = r[1];
                s.z = r[2];
                Vector3 dp = disp_pos;
                dp.x = x;
                dp.y = y;
                dp.z = z;
                s.disp_pos = dp;
            }, ref px);

        }

        /// <summary>
        /// Shifts something to the position p
        /// </summary>
        /// <param name="p"></param>
        public void UpdateToNewPoint(Position p)
        {
            UpdateState((ref State s, ref Position np) =>
            {
                s.x = p.x;
                s.y = p.y;
                s.z = p.z;
                Vector3 dp = disp_pos;
                dp.x = x;
                dp.y = y;
                dp.z = z;
                s.disp_pos = dp;
            }, ref p);

        }

        /// <summary>
        /// Shifts something by (dx,dy,dz)
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        public void ShiftPos(int dx, int dy, int dz)
        {
            int[] dp = new int[3] { dx, dy, dz };
            UpdateState((ref State s, ref int[] r) =>
            {
                s.x += r[0];
                s.y += r[1];
                s.z += r[2];
                Vector3 dpn = disp_pos;
                dpn.x = x;
                dpn.y = y;
                dpn.z = z;
                s.disp_pos = dpn;
            }, ref dp);
        }

        public void SetDispPos(SerializableVector3 vec)
        {
            UpdateState((ref State s, ref SerializableVector3 r) =>
            {
                s.disp_pos = r;
            }, ref vec);
        }

        public bool computable()
        {
            return true;
        }

        public string ComponentType()
        {
            return "Position";
        }

        public void OnUpdateState()
        {
        }

        public void RegisterUpdateAction(Action<IBaseComponent> update)
        {

        }
    }
}