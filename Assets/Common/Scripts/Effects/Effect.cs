using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public abstract class Effect : MonoBehaviour
    {
        public abstract void Run();

        [ContextMenu("Run")]
        public void RunViaMenu()
        {
            Run();
        }
    }
}
