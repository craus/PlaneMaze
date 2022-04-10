using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Interpolate : StringValueProvider
{
    public List<AbstractValueProvider> args;
    public string format = "";
    public override string Value => string.Format(format, args.Select(avp => avp.ObjectValue).ToArray());
}
