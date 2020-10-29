using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

namespace HighKings
{
    public class SelectionStatInfoManager : MonoBehaviour
    {
        public GameController game_controller;
        public List<GameObject> active_stats;
        public Dictionary<string, GameObject> active_displays;
        public Vector2 stat_bar_size;
        public Vector2 padding;
        public GameObject stat_prefab;
        public List<InspectorData> display_queue;
        public List<string> display_string_queue;
        public string active_entity;
        public Dictionary<string, Action<GameObject,InspectorData,object>> display_creators;

        void Awake()
        {
            active_stats = new List<GameObject>();
            display_queue = new List<InspectorData>();
            active_displays = new Dictionary<string, GameObject>();
        }

        public void SetDisplayQueue()
        {
            foreach(InspectorData id in MainGame.instance.display_data.Values)
            {
                display_string_queue.Add(id.component_name);
                AddToDisplayQueue(id);
            }
        }

        public void AddToDisplayQueue(InspectorData id)
        {
            int i = 0;
            while (i < display_queue.Count)
            {
                if (id.default_position <= display_queue[i].default_position)
                {
                    display_queue.Insert(i, id);
                    return;
                }
                i += 1;
            }
            display_queue.Add(id);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void MakeDisplays(Entity e)
        {
            if(active_entity != null && active_entity != "")
            {
                for(int i = active_stats.Count; i > 0; i -= 1)
                {
                    GameObject obj = active_stats[i - 1];
                    Entity ent = active_entity;
                    ent.GetComponent(display_string_queue[i - 1]).subscriber.RemoveAfterAction("InspectorDisplayReset");
                    active_stats.RemoveAt(i - 1);
                    Destroy(obj);
                }
            }
            if(e == null)
            {
                active_entity = "";
                return;
            }
            active_entity = e;
            for (int j = 0; j < display_string_queue.Count; j += 1)
            {
                if (e.HasComponent(display_string_queue[j]))
                {
                    MakeDisplay(stat_prefab, e, e.GetComponent(display_string_queue[j]));
                }
            }
        }

        public void MakeStatDisplays(Entity e)
        {
            if(active_entity != "NULL")
            {
                for (int i = active_stats.Count; i > 0; i -= 1)
                {
                    GameObject obj = active_stats[i - 1];
                    MainGame.instance.GetSubscriberSystem(obj.GetComponent<InspectorDisplay>().component_name).UnsubscribeAfterAction(active_entity, "UpdateStatDisplay");
                    active_stats.Remove(obj);
                    active_displays.Remove("Display"+obj.GetComponent<InspectorDisplay>().component_name);
                    Destroy(obj);
                }
            }
            if(e == null)
            {
                active_entity = "NULL";
                return;
            }
            for(int j = 0; j < display_queue.Count; j += 1)
            {
                Debug.Log("Making display");
                active_entity = e.entity_string_id;
                if (e.HasComponent(display_queue[j].component_name))
                {
                    active_stats.Add(MakeStatBarDisplay(stat_prefab, e, e.GetComponent<BaseStatistic>(display_queue[j].component_name), display_queue[j]));
                }
            }
        }

        public GameObject MakeStatBarDisplay(GameObject prefab, Entity e, BaseStatistic stat, InspectorData id)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.AddComponent<InspectorDisplay>().CopyData(id);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = stat_bar_size;
            rect.position = new Vector2(transform.position.x + padding.x, transform.position.y - active_stats.Count * (rect.sizeDelta.y + padding.y) - padding.y);
            obj.GetComponent<Text>().text = $"{id.display_name}: {stat.curr_value} / {stat.base_value}";

            obj.SetActive(true);

            Debug.Log(stat.stat_name);
            MainGame.instance.GetSubscriberSystem(stat.stat_name).SubscribeAfterAction(new List<Entity> { e }, ResetCompDisplay, "UpdateStatDisplay" );

            Debug.Log($"Made {stat.stat_name} bar");

            return obj;
        }

        public GameObject MakeDisplay(GameObject prefab, Entity e, IBaseComponent o)
        {
            GameObject obj = Instantiate(prefab, transform);
            IInspectorDisplay disp = (IInspectorDisplay)o;

            obj.AddComponent<InspectorDisplay>().CopyData(disp.inspector_data);
            //if(!display_creators.TryGetValue(disp.inspector_data.display_type, out Action<GameObject,InspectorData,object> action))
            //{
            //    Debug.LogWarning($"Could not find inspector display type: {disp.inspector_data.display_type}");
            //    Destroy(obj);
            //    return null;
            //}
            //action?.Invoke(obj, disp.inspector_data, o);
            active_stats.Add(obj);
            obj.SetActive(true);
            e.GetComponent(disp.inspector_data.component_name).subscriber.RegisterAfterAction("InspectorDisplayReset",(b) => ResetCompDisplayBar(obj,b));
            RectTransform rect_transform = obj.GetComponent<RectTransform>() != null ? obj.GetComponent<RectTransform>() : obj.AddComponent<RectTransform>();
            rect_transform.sizeDelta = stat_bar_size;
            //Debug.Log(active_stats.Count);
            rect_transform.position = new Vector2(transform.position.x + padding.x, transform.position.y - (active_stats.Count - 1) * (rect_transform.sizeDelta.y + padding.y) - padding.y);
            ResetCompDisplayBar(obj, o);

            return obj;
        }


        void ResetCompDisplay(Entity e, IBaseComponent stat)
        {
            BaseStatistic b_stat = (BaseStatistic)stat;
            foreach(GameObject obj in active_stats)
            {
                if(obj.GetComponent<InspectorDisplay>().component_name == b_stat.stat_name)
                {
                    obj.GetComponent<Text>().text = $"{obj.GetComponent<InspectorDisplay>().display_name}: {b_stat.curr_value} / {b_stat.base_value}";
                }
            }
        }

        Action<Entity, IBaseComponent> DisplayTypeReset(string display_type, string comp_name, Entity e_t, Type t)
        {
            MethodInfo method = GetType().GetMethod("ResetCompDisplay" + display_type, new Type[2] { typeof(Entity), t });
            
            if (method == null)
            {
                Debug.LogWarning($"Could not find method ResetCompDisplay{display_type} with type param {t.ToString()}");
                return null;
            }

            void action(Entity e, IBaseComponent o) { method.Invoke(this, new object[2] { e, o }); }
            e_t.GetComponent(comp_name).subscriber.RegisterAfterAction("ResetCompDisplay", (b) => { action(e_t, b); });

            return action;
        }

        void ResetCompDisplayBar(Entity e, Health h)
        {
            GameObject g = active_displays["DisplayHealthBar"];
            g.GetComponent<Text>().text = $"Health: {h.curr_value} / {h.base_value}";
        }

        void ResetCompDisplayBar(GameObject o, IBaseComponent b)
        {
            o.GetComponent<Text>().text = ((IInspectorDisplay)b).DisplayText();
        }

    }
}

