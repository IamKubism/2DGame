using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HighKings
{
    public class SelectionStatInfoManager : MonoBehaviour
    {
        public GameController game_controller;
        public List<GameObject> active_stats;
        public Vector2 stat_bar_size;
        public Vector2 padding;
        public GameObject stat_prefab;
        public List<InspectorData> display_queue;
        public Entity active_entity;

        void Awake()
        {
            active_stats = new List<GameObject>();
            display_queue = new List<InspectorData>();
        }

        public void SetDisplayQueue()
        {
            foreach(InspectorData id in MainGame.instance.display_data.Values)
            {
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

        public void MakeStatDisplays(Entity e)
        {
            if(active_entity != null)
            {
                for (int i = active_stats.Count; i > 0; i -= 1)
                {
                    GameObject obj = active_stats[i - 1];
                    MainGame.instance.GetSubscriberSystem<BaseStatistic>(obj.GetComponent<InspectorDisplay>().component_name).UnsubscribeAfterAction(new List<Entity> { active_entity }, "UpdateStatDisplay");
                    active_stats.Remove(obj);
                    Destroy(obj);
                }
            }
            active_entity = e;
            if(e == null)
            {
                return;
            }
            for(int j = 0; j < display_queue.Count; j += 1)
            {
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

            MainGame.instance.GetSubscriberSystem<BaseStatistic>(stat.stat_name).SubscribeAfterAction(new List<Entity> { e }, ResetCompDisplay, "UpdateStatDisplay" );

            Debug.Log($"Made {stat.stat_name} bar");

            return obj;
        }

        void ResetCompDisplay(Entity e, BaseStatistic stat)
        {
            foreach(GameObject obj in active_stats)
            {
                if(obj.GetComponent<InspectorDisplay>().component_name == stat.stat_name)
                {
                    obj.GetComponent<Text>().text = $"{obj.GetComponent<InspectorDisplay>().display_name}: {stat.curr_value} / {stat.base_value}";
                }
            }
        }
    }
}

