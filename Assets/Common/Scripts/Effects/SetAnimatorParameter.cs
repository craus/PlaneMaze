using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

namespace Common
{
    public class SetAnimatorParameter : Effect
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string parameter;
        [SerializeField] private bool boolValue;

        public override void Run()
        {
            var parameterType = animator.parameters.Where(p => p.name == parameter).First().type;
            if (parameterType == AnimatorControllerParameterType.Trigger)
            {
                animator.SetTrigger(parameter);
            }
            if (parameterType == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter, boolValue);
            }
        }
    }
}
