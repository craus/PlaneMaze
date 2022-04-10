using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TransformationSaver : MonoBehaviour
{
    public Transform root;
    public ConstTransformationValueProvider container;
    public bool global = true;

    [ContextMenu("Save")]
    public void Save()
    {
        container.transformation = new Transformation(root, global);
#if UNITY_EDITOR
        EditorUtility.SetDirty(container);
#endif
    }

    [ContextMenu("Load")]
    public void Load()
    {
        container.Value.ApplyToTransform(root, global);
    }
}
