using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public static class Helpers
{
    public async static Task TeleportAway(Figure figure, int radius) {
        var destination = figure.location.Vicinity(maxDx: radius, maxDy: radius).Where(c => c.Free).Rnd();

        await figure.Move(destination, isTeleport: true, teleportAnimation: true);
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
        await Task.Delay((int)(1000 * seconds / Mathf.Pow(2, Player.instance.commands.Count)));
    }
}
