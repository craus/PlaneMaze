using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class PlayAnimationEffect : Effect
    {
        [SerializeField] private new Animation animation;

        public override void Run()
        {
            animation.Play();
        }
    }
}
