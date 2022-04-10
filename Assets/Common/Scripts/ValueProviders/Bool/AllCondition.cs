using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AllCondition : AggregateCondition {
    protected override bool BoolValue => Arguments.All(b => b);
}
