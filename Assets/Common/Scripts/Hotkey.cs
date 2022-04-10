using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Hotkey : MonoBehaviour
{
    public UnityEvent onPress;

    public KeyCode key;

    public void Update() {
        if (Input.GetKeyDown(key)) {
            onPress.Invoke();
        }
    }
}