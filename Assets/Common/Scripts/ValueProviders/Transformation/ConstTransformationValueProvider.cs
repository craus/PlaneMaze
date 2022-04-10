using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConstTransformationValueProvider : TransformationValueProvider
{
    public Transformation transformation;

    public override Transformation Value => transformation;
}
