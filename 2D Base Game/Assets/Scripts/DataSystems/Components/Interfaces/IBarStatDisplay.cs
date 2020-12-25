using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Psingine
{
    public interface IInspectorDisplay
    {
        InspectorData inspector_data { get; set; }
        string DisplayText();
    }

}

