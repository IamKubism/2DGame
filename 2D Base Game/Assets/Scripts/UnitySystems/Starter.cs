using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HighKings
{
    [CreateAssetMenu]
    public class Starter : ScriptableObject
    {
        public SpriteManager sprite_manager;
        public PrototypeLoader prototyper;

        private void OnEnable()
        {
            
        }
    }

}
