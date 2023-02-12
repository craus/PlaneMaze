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
        await figure.Move(destination, isTeleport: true);
    }
}
