using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HighKings
{
    public class KeyBoardController : MonoBehaviour
    {
        public GameController game_controller;
        public Dictionary<KeyCode, Action> key_actions;

    // Start is called before the first frame update
        void Start()
        {
            key_actions = new Dictionary<KeyCode, Action>();
            key_actions.Add(KeyCode.Space, () => { game_controller.SetTimeMod(0f); });
            key_actions.Add(KeyCode.Alpha1, () => { game_controller.SetTimeMod(1f); });
            key_actions.Add(KeyCode.Alpha2, () => { game_controller.SetTimeMod(2f); });
            key_actions.Add(KeyCode.Alpha3, () => { game_controller.SetTimeMod(3f); });
        }

        // Update is called once per frame
        void Update()
        {
            KeyCode k = KeyDown();
            if(k != default && key_actions.ContainsKey(k))
            {
                key_actions[k]?.Invoke();
            }
        }

        KeyCode KeyDown()
        {
            foreach(KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyUp(keycode))
                {
                    return keycode;
                }
            }
            return default;
        }
    }
}
