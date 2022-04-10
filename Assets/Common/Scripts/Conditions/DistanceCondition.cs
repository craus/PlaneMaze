using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Common
{
    public class DistanceCondition : BoolValueProvider
    {
        [SerializeField] private Component m_firstObject;
        [SerializeField] private ComponentProvider firstObjectProvider;
        private Component firstObject => firstObjectProvider != null ? firstObjectProvider.Value : m_firstObject;

        [SerializeField] private Component m_secondObject;
        [SerializeField] private ComponentProvider secondObjectProvider;
        private Component secondObject => secondObjectProvider != null ? secondObjectProvider.Value : m_secondObject;

        [SerializeField] private bool less = true;
        [SerializeField] private float threshold;

        private float SqrDistance => Vector3.SqrMagnitude(firstObject.transform.position - secondObject.transform.position);

        public override bool Value => less ? (SqrDistance < threshold) : (SqrDistance > threshold);
    }
}
