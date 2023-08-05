using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public static class Helpers
{
    public static float animationSpeed = 1;

    public async static Task TeleportAway(Figure figure, int radius) {
        var destination = figure.Location.Vicinity(maxDx: radius, maxDy: radius).Where(c => c.Free).Rnd();

        if (
            figure.GetComponent<BlackMage>() != null ||
            figure.GetComponent<Lich>() != null ||
            figure.GetComponent<Player>() != null
        ) {
            SoundManager.instance.teleport.Play();
        }
        Game.Debug($"{figure} teleports to {destination}");

        _ = PlayTeleportExitAnimation(destination);

        await figure.Move(destination, isTeleport: true, teleportAnimation: true);
    }

    public static async Task PlayTeleportExitAnimation(Cell destination) {
        var anim = UnityEngine.Object.Instantiate(Library.instance.teleportExit);
        anim.transform.position = destination.transform.position;
        await anim.transform.RotateBy(Quaternion.Euler(0, 0, -90), 0.5f, times: 4, steps: 7);
        UnityEngine.Object.Destroy(anim.gameObject);
    }

    public static Cell RayCast(Cell startPosition, Vector2Int delta, Func<Cell, bool> free = null, Func<Cell, bool> target = null, int distance = 99) {
        free ??= c => c.Free;
        target ??= c => false;

        var current = startPosition;
        for (int i = 0; i < distance; i++) {
            current = current.Shift(delta);
            if (target(current)) {
                return current;
            }
            if (!free(current)) {
                return null;
            }
        }
        return null;
    }

    public static async Task Delay(float seconds) {
        await DelayManager.Delay(seconds / Mathf.Pow(2, Player.instance != null ? Player.instance.commands.Count : 0) / animationSpeed / 2);
    }

    public static Vector2Int StepAtDirection(this Vector2Int v) {
        if (v == Vector2Int.zero) {
            return v;
        }
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y)) {
            v.y = 0;
        } else if (Mathf.Abs(v.x) < Mathf.Abs(v.y)) {
            v.x = 0;
        } else {
            if (Rand.rndEvent(0.5f)) {
                v.x = 0;
            } else {
                v.y = 0;
            }
        }
        v /= (int)v.magnitude;
        return v;
    }

    public static Vector2Int StepAtDirectionDiagonal(this Vector2Int v) {
        if (v.x > 0) v.x = 1;
        if (v.x < 0) v.x = -1;
        if (v.y > 0) v.y = 1;
        if (v.y < 0) v.y = -1;
        return v;
    }
}
