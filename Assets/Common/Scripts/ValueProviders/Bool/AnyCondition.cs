using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnyCondition : AggregateCondition {
    protected override bool BoolValue => Arguments.Any(a => a);
}
