using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ContentSizeFitter make objects dirty when opened in editor. 
/// So one may want to activate it only in play mode start.
/// </summary>
public class EnableContentSizeFitterOnAwake : MonoBehaviour
{
    public void Awake() {
        GetComponent<ContentSizeFitter>().enabled = true;
    }
}
