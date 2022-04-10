using RSG;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    public class Timer : FloatValueProvider
    {
        [SerializeField] private float fasterTimersMultiplier = 3;
        [SerializeField] private bool m_Multiplier = true;
        [SerializeField] private float startTime = float.NegativeInfinity;
        [SerializeField] private float finishTime = float.PositiveInfinity;

        [SerializeField] private float pausedTime;
        [SerializeField] private bool paused;

        private float CurrentTime => paused ? pausedTime : TimeManager.Time();

        public override float Value => (finishTime - CurrentTime) * fasterTimersMultiplier;

        public float TimeSpent => (CurrentTime - startTime) * fasterTimersMultiplier;

        public UnityEvent onTimeout;
        public bool once;
        public bool spent;

        Promise<bool> timeout;

        public void Pause()
        {
            pausedTime = CurrentTime;
            paused = true;
        }

        public void Unpause()
        {
            paused = false;
        }

        public void Awake()
        {
            if (m_Multiplier)
            {
                fasterTimersMultiplier = 3;
            }
            else
            {
                fasterTimersMultiplier = 1;
            }
        }

        public void StartFrom(float duration)
        {
            startTime = CurrentTime;
            finishTime = CurrentTime + duration / fasterTimersMultiplier;
            if (timeout != null)
            {
                timeout.Resolve(false);
            }
            timeout = new Promise<bool>();
        }

        public IPromise<bool> RunFrom(float duration)
        {
            StartFrom(duration);
            return timeout;
        }

        void Timeout()
        {
            if (!once || !spent)
            {
                onTimeout.Invoke();
                spent = true;
            }

            if (timeout != null)
            {
                timeout.Resolve(true);
                timeout = null;
            }
        }
        protected override void Update()
        {
            base.Update(); 
            if (Value < 0)
            {
                Timeout();
            }
        }
    }
}