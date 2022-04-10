using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Vector2 dragPoint;

    private Vector2 WorldMousePoint => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    private void RestoreCameraPosition(Vector2 desiredWorldMousePoint) {
        Camera.main.transform.position = (Camera.main.transform.position.xy() - (WorldMousePoint - desiredWorldMousePoint)).withZ(Camera.main.transform.position.z);
    }

    public void Update() {
        if (Input.GetMouseButtonDown(1)) {
            dragPoint = WorldMousePoint;
        }
        if (Input.GetMouseButton(1)) {
            RestoreCameraPosition(dragPoint);
        }

        var oldWorldMousePoint = WorldMousePoint;
        Camera.main.orthographicSize *= Mathf.Pow(1.25f, -Input.mouseScrollDelta.y);
        RestoreCameraPosition(oldWorldMousePoint);
    }
}
