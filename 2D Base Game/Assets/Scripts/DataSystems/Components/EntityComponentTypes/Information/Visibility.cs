using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Psingine
{
    public class Visibility : IBaseComponent
    {
        public SubscriberEvent subscriber { get; set; }

        public Visibility()
        {

        }

        public Visibility(Visibility vis)
        {

        }

        public Visibility(JObject obj)
        {

        }

        public bool Trigger(Event e)
        {
            return true;
        }

        public void CopyData(IBaseComponent comp)
        {
            throw new System.NotImplementedException();
        }
    }

}
