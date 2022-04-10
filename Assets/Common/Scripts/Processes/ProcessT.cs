using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using RSG;

namespace Common
{
    public abstract class ProcessT<T> : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_onFinished;
        public UnityEvent onFinished => m_onFinished;

        public virtual void Run(T parameter)
        {
            RunProcess(parameter).Then(completed =>
            {
                if (completed) onFinished.Invoke();
            });
        }

        public abstract IPromise<bool> RunProcess(T parameter);

        public virtual void Interrupt()
        {

        }
    }
}
