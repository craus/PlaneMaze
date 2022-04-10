using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using RSG;

namespace Common
{
    public abstract class Process : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_onFinished;
        public UnityEvent onFinished => m_onFinished;

        public void Run()
        {
            RunProcess().Then(completed =>
            {
                if (completed)
                {
                    onFinished.Invoke();
                }
                
            }).Done();
        }

        public IPromise<bool> RunProcessPublic()
        {   
            return RunProcess().Then(completed =>
            {
                if (completed)
                {
                    onFinished.Invoke();
                }
                return Promise<bool>.Resolved(completed); 
            });
        }

        protected abstract IPromise<bool> RunProcess();

        public virtual void Interrupt()
        {

        }
    }
}
