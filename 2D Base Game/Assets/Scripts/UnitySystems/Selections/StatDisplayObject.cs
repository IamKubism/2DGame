using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighKings
{
    public class StatDisplayObject : MonoBehaviour
    {
        public GameObject name_panel;
        public GameObject value_object;
        public float selection_info_x_len;
        public float display_y_len;
        public Color bar_color;
        public float stat_value;
        public string stat_name;
        public Entity entity_target;

        public static StatDisplayObject MakeStatDisplayObject(GameObject selection_info, GameObject panel, Entity entity_target, string stat_name, float display_y_len)
        {
            StatDisplayObject panel_vals = panel.AddComponent<StatDisplayObject>();
            panel_vals.selection_info_x_len = selection_info.GetComponent<RectTransform>().sizeDelta.x;
            panel_vals.stat_name = stat_name;
            panel_vals.display_y_len = display_y_len;

            RectTransform panel_transform = panel.AddComponent<RectTransform>();
            panel_transform.SetParent(selection_info.transform);
            panel_transform.pivot = new Vector2(0,0);

            GameObject name_panel = panel_vals.name_panel = new GameObject($"{panel_vals.stat_name}_name_display");
            RectTransform name_panel_transform = name_panel.AddComponent<RectTransform>();

            GameObject value_object = panel_vals.value_object = new GameObject($"{panel_vals.stat_name}_value_display");
            RectTransform value_panel_transform = value_object.AddComponent<RectTransform>();



            return panel_vals;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

