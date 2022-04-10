using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CopyTransform : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform self;

    public void Update()
    {
        if (self == null) self = transform;
        self.position = target.position;
        self.rotation = target.rotation;
        // TODO do something with scale
    }
}