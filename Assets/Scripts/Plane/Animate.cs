using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class Animate
{
    public async static Task Move(
        this Transform transform, 
        Vector3 endPosition, 
        float duration, 
        int steps = 7,
        float startPhase = 0,
        float endPhase = 1
    ) {
        var startPosition = transform.position;

        for (int i = 1; i <= steps; i++) {
            var phase = i * 1f / steps;
            if (phase < startPhase) {
                continue;
            }
            if (phase > endPhase) {
                break;
            }
            if (transform == null) {
                return;
            }
            transform.position = Vector3.Lerp(startPosition, endPosition, phase);
            await Task.Delay((int)(duration / steps * 1000));
        }
    }
}
