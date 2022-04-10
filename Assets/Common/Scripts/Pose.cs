using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class Pose
{
    public Transformation transformation;

    public MapStringTransformation childrenTransformations = new MapStringTransformation();

    public Pose(Transform t)
    {
        transformation = new Transformation(t);
        t.Children().ForEach(c =>
        {
            SaveTransformations(c, childrenTransformations, "");
        });
    }

    private void SaveTransformations(Transform t, MapStringTransformation mp, string prefix)
    {
        if (!t.gameObject.activeSelf)
        {
            return;
        }
        mp[prefix + t.name] = new Transformation(t);
        t.Children().ForEach(c =>
        {
            SaveTransformations(c, mp, prefix + t.name + "/");
        });
    }

    public void ApplyToTransform(Transform t)
    {
        transformation.ApplyToTransform(t);
        childrenTransformations.ForEach(p =>
        {
            var child = t.Find(p.Key);
            if (child != null)
            {
                p.Value.ApplyToTransform(child);
                DebugManager.LogFormat("transformation set: {0}", p.Key);
            }
            else
            {
                DebugManager.LogFormat("No transform: {0}", p.Key);
            }
        });
    }
}