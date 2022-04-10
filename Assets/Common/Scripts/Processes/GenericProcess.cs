using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using RSG;

namespace Common
{
    public class GenericProcess : Process
    {
        [SerializeField] private UnityEvent m_onStarted;
        public UnityEvent onStarted => m_onStarted;

        [SerializeField] private UnityEvent m_onInterrupted;
        public UnityEvent onInterrupted => m_onInterrupted;

        private Promise<bool> process;

        protected override IPromise<bool> RunProcess()
        {
            if (process != null)
            {
                process.Resolve(false);
            }
            process = new Promise<bool>();
            onStarted.Invoke();
            return process;
        }

        public virtual void Finish()
        {
            if (process == null)
            {
                return;
            }

            process.Resolve(true);
            process = null;
        }

        public override void Interrupt()
        {
            if (process == null)
            {
                return;
            }
            base.Interrupt();

            process.Resolve(false);
            process = null;
            onInterrupted.Invoke();
        }
    }
}
