using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Common
{
    public class OnUnhover : Trigger, IPointerExitHandler
    {
        public void OnPointerExit(PointerEventData eventData) {
            Run();
        }

        public void OnDisable() {
            Run();
        }
    }
}
