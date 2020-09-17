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
        public List<InspectorDisplay> display_queue;

        void Awake()
        {
            active_stats = new List<GameObject>();
            display_queue = new List<InspectorDisplay>();
        }

        public void SetDisplayQueue()
        {
            foreach(InspectorDisplay id in MainGame.instance.display_data.Values)
            {
                AddToDisplayQueue(id);
            }
        }

        public void AddToDisplayQueue(InspectorDisplay id)
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
            active_stats = new List<GameObject>();
            for(int j = 0; j < display_queue.Count; j += 1)
            {
                if (e.HasComponent(display_queue[j].component_name))
                {
                    active_stats.Add(MakeStatBarDisplay(stat_prefab, e.GetComponent<BaseStatistic>(display_queue[j].component_name), display_queue[j]));
                }
            }
        }

        public GameObject MakeStatBarDisplay(GameObject prefab, BaseStatistic stat, InspectorDisplay id)
        {
            GameObject obj = Instantiate(prefab, transform);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = stat_bar_size;
            rect.position = new Vector2(transform.position.x + padding.x, transform.position.y - active_stats.Count * (rect.sizeDelta.y + padding.y) - padding.y);
            obj.GetComponent<Text>().text = $"{id.display_name}: {stat.curr_value} / {stat.base_value}";

            obj.SetActive(true);

            Debug.Log($"Made {stat.stat_name} bar");

            return obj;
        }
    }
}

