using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PoseSaver : MonoBehaviour
{
    public Transform root;
    public ConstPoseValueProvider container;

    [ContextMenu("Save")]
    public void Save()
    {
        container.pose = new Pose(root);
#if UNITY_EDITOR
        EditorUtility.SetDirty(container);
#endif
    }

    [ContextMenu("Load")]
    public void Load()
    {
        container.Value.ApplyToTransform(root);
    }
}
