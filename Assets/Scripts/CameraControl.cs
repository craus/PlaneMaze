using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Vector2 dragPoint;

    public void Update() {
        if (Input.GetMouseButtonDown(1)) {
            dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(1)) {
            Vector2 worldMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position = (Camera.main.transform.position.xy() - (worldMousePoint - dragPoint)).withZ(Camera.main.transform.position.z);
        }
    }
}
