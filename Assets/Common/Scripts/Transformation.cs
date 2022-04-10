using UnityEngine;
using System;

[Serializable]
public class Transformation
{
    public Vector3 position;
    public Quaternion rotation;

    public static Transformation Lerp(Transformation a, Transformation b, float t)
    {
        return new Transformation(Vector3.Lerp(a.position, b.position, t), Quaternion.Lerp(a.rotation, b.rotation, t));
    }

    public Transformation(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public Transformation(Transform t, bool global = false)
    {
        if (global)
        {
            position = t.position;
            rotation = t.rotation;
        }
        else
        {
            position = t.localPosition;
            rotation = t.localRotation;
        }
    }

    public void ApplyToTransform(Transform t, bool global = false)
    {
        if (global)
        {
            t.position = position;
            t.rotation = rotation;
        }
        else
        {
            t.localPosition = position;
            t.localRotation = rotation;
        }
    }
}