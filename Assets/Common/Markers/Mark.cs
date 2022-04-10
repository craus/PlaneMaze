using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Common
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Common/Mark")]
    public class Mark : ScriptableObject
    {
        public IEnumerable<Component> objects => FindObjectsOfType<Marker>().Where(m => m.mark == this);

        public Marker target;
    }
}