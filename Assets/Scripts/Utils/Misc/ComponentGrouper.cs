using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDB.Utils.Misc
{
    /// <summary>
    /// Amazing class that lets you group & hide components.
    /// Original code can be found here: https://discussions.unity.com/t/689391
    /// </summary>
    public class ComponentGrouper : MonoBehaviour
    {
        public List<ComponentGroup> groups = new List<ComponentGroup>();

        public ComponentGroup FindGroup(Component component)
        {
            return groups.FirstOrDefault(g => g.Components.Contains(component));
        }
    }
    
    [System.Serializable]
    public class ComponentGroup
    {
        public string          Name;
        public List<Component> Components = new List<Component>();

        public bool IsVisible  { get; set; } = true;
        public bool IsEditable { get; set; }
    }
}