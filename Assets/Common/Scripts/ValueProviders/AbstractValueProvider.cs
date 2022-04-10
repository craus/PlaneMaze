using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class AbstractValueProvider : MonoBehaviour {
    public virtual object ObjectValue {
        get {
            return null;
        }
    }
}
