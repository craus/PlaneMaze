using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

namespace Common
{
    public class SetActive : Effect
    {
        [SerializeField] private List<GameObject> gameObjects;
        [SerializeField] private bool on;

        public override void Run()
        {
            gameObjects.ForEach(go => go.SetActive(on));
        }
    }
}
