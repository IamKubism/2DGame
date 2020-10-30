using Newtonsoft.Json;
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
        public struct Tile
        {
            [JsonProperty]
            public int x, y, z;
        }

        [JsonProperty]
        public SerializableVector3 disp_pos;


        [JsonProperty]
        public Tile tile;

        public SubscriberEvent subscriber { get; set; }


        /// <summary>
        /// Coordinates of this position
        /// </summary>
        public int x
        {
            get
            {
                return tile.x;
            }
        }
        /// <summary>
        /// Coordinates of this position
        /// </summary>
        public int y
        {
            get
            {
                return tile.y;
            }
        }
        /// <summary>
        /// Coordinates of this position
        /// </summary>
        public int z
        {
            get
            {
                return tile.z;
            }
        }

        delegate void ActionByRef<T1, T2>(ref T1 p1, ref T2 p2);
        delegate void ActionByRef<T1, T2, T3>(ref T1 p1, ref T2 p2, ref T3 p3);

        void UpdateState<TP1>(ActionByRef<Tile, TP1> proc, ref TP1 p1)
        { proc(ref tile, ref p1); OnUpdateState(); }
        void UpdateState<TP1, TP2>(ActionByRef<Tile, TP1, TP2> proc, ref TP1 p1, ref TP2 p2)
        { proc(ref tile, ref p1, ref p2); OnUpdateState(); }

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
        public Position(string entity_id, int x, int y, int z, Position p)
        {
            tile = new Tile
            {
                x = x,
                y = y,
                z = z,
            };
            disp_pos = new SerializableVector3(x, y, z);
        }

        public Position(int x, int y, int z, Position p)
        {
            tile = new Tile
            {
                x = x,
                y = y,
                z = z,
            };
            disp_pos = new SerializableVector3(x, y, z);
        }

        public Position(Position pos)
        {
            tile = pos.tile;
            disp_pos = pos.disp_pos;
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
            return $"Position: ({x},{y},{z}) with displaced vector {disp_pos.ToString()}";
        }

        /// <summary>
        /// Shifts something to the point (x,y,z)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void UpdateToNewPoint(int x, int y, int z, bool call_subscriber = false)
        {
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
            int[] px = new int[3] { x, y, z };
            UpdateState((ref Tile s, ref int[] r) =>
            {
                s.x = r[0];
                s.y = r[1];
                s.z = r[2];

            }, ref px);
            Vector3 dp = disp_pos;
            dp.x = x;
            dp.y = y;
            dp.z = z;
            disp_pos = dp;
            if (call_subscriber)
                subscriber.OperateAfterOnComp();
        }

        /// <summary>
        /// Shifts something to the position p
        /// </summary>
        /// <param name="p"></param>
        public void UpdateToNewPoint(Position p, bool call_subscriber = false)
        {
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
            UpdateState((ref Tile s, ref Position np) =>
            {
                s.x = p.x;
                s.y = p.y;
                s.z = p.z;

            }, ref p);
            Vector3 dp = disp_pos;
            dp.x = x;
            dp.y = y;
            dp.z = z;
            disp_pos = dp;
            if (call_subscriber)
                subscriber.OperateAfterOnComp();
        }

        /// <summary>
        /// Shifts something by (dx,dy,dz)
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        public void ShiftPos(int dx, int dy, int dz, bool call_subscriber = false)
        {
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
            int[] dp = new int[3] { dx, dy, dz };
            UpdateState((ref Tile s, ref int[] r) =>
            {
                s.x += r[0];
                s.y += r[1];
                s.z += r[2];
            }, ref dp);
            Vector3 dpn = disp_pos;
            dpn.x = x;
            dpn.y = y;
            dpn.z = z;
            disp_pos = dpn;
            if (call_subscriber)
                subscriber.OperateAfterOnComp();
        }

        public void SetDispPos(SerializableVector3 vec, bool call_subscriber = false)
        {
            if (call_subscriber)
                subscriber.OperateBeforeOnComp();
            disp_pos = vec;
            if (call_subscriber)
                subscriber.OperateAfterOnComp();
        }

        public void OnUpdateState()
        {
        }

        public static Entity ToCell(Position pos)
        {
            return World.instance.GetTileFromCoords(pos.p);
        }


        public bool Trigger(Event e)
        {
            bool eval = true;
            if (e.tags.Contains("Positional"))
            {
                e.AddUpdate(SetCurrTileParam, 0);
                e.AddUpdate(GetCurrTileParam, 1000);
            }
            return eval;
        }

        public void SetCurrTileParam(Event e)
        {
            e.SetParamValue("curr_tile", ToCell(this), (p1, p2) => { return p2; });
        }

        public void GetCurrTileParam(Event e)
        {
            UpdateToNewPoint((Entity)e.GetParamValue("curr_tile"), true);
        }

        public static implicit operator Position(Entity e)
        {
            return e.GetComponent<Position>();
        }

        public static implicit operator int[](Position p)
        {
            return p.p;
        }
    }
}