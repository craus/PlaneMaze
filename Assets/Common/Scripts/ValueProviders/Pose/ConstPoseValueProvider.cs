using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConstPoseValueProvider : PoseValueProvider
{
    public Pose pose;

    public override Pose Value => pose;
}
