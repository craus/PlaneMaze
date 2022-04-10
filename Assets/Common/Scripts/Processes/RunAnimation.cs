using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using RSG;

namespace Common
{
    public class RunAnimation : Process
    {
        public Animator animator;
        public string parameterName;

        protected override IPromise<bool> RunProcess()
        {
            throw new System.NotImplementedException();
        }
    }
}
