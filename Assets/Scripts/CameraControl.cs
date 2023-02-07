using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraControl : Singletone<CameraControl>
{
    public Vector2 dragPoint;

    private Vector2 WorldMousePoint => Camera.main.ScreenToWorldPoint(Input.mousePosition);

    public bool followPlayer;
    public float lerpHalfLife = 1;

    public bool followPoint;
    public Vector3 pointToFollow;
    public float followDistance = 10;

    private void RestoreCameraPosition(Vector2 desiredWorldMousePoint) {
        Camera.main.transform.position = (Camera.main.transform.position.xy() - (WorldMousePoint - desiredWorldMousePoint)).withZ(Camera.main.transform.position.z);
    }

    public Vector3 MoveTo(Vector3 from, Vector3 to, float distance) {
        if (Vector3.Distance(from, to) < distance) {
            return to;
        }
        return from + (to - from).normalized * distance;
    }

    public void Update() {
        if (Input.GetMouseButtonDown(1)) {
            dragPoint = WorldMousePoint;
        }
        if (Input.GetMouseButton(1)) {
            RestoreCameraPosition(dragPoint);
        }

        var oldWorldMousePoint = WorldMousePoint;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize * Mathf.Pow(1.25f, -Input.mouseScrollDelta.y), 1, 50);
        RestoreCameraPosition(oldWorldMousePoint);

        if (Game.instance.player && followPlayer && !Input.GetMouseButton(1)) {
            Camera.main.transform.position = Vector3.Lerp(
                Game.instance.player.transform.position.Change(z: Camera.main.transform.position.z),
                Camera.main.transform.position, 
                Mathf.Pow(0.5f, Time.deltaTime / lerpHalfLife)
            );
        }

        if (followPoint && !Input.GetMouseButton(1)) {

            Camera.main.transform.position = Vector3.Lerp(
                MoveTo(pointToFollow.Change(z: Camera.main.transform.position.z), Camera.main.transform.position, Camera.main.orthographicSize/2),
                Camera.main.transform.position,
                Mathf.Pow(0.5f, Time.deltaTime / lerpHalfLife)
            );
        }
    }

    public void TeleportToPlayer() {
        Camera.main.transform.position = Game.instance.player.transform.position.Change(z: Camera.main.transform.position.z);
    }
}
