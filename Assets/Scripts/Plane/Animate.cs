using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
            await Helpers.Delay(duration / steps);
        }
    }

    public async static Task Rotate(
        this Transform transform,
        Quaternion endRotation,
        float duration,
        int steps = 7,
        float startPhase = 0,
        float endPhase = 1
    ) {
        var startRotation = transform.rotation;

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
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, phase);
            await Helpers.Delay(duration / steps);
        }
    }

    public async static Task RotateBy(
        this Transform transform,
        Quaternion rotation,
        float duration,
        int times = 1,
        int steps = 7,
        float startPhase = 0,
        float endPhase = 1
    ) {
        for (int i = 0; i < times; i++) {
            await Rotate(transform, transform.rotation * rotation, duration / times, steps, startPhase, endPhase);
        }
    }

    public async static Task Zoom(
        this Transform transform,
        Vector3 endZoom,
        float duration,
        int steps = 7,
        float startPhase = 0,
        float endPhase = 1
    ) {
        var startZoom = transform.localScale;

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
            transform.localScale = Vector3.Lerp(startZoom, endZoom, phase);
            await Helpers.Delay(duration / steps);
        }
    }

    public async static Task RadialFill(
        this Image image,
        float endFillAmount,
        float duration,
        int steps = 7,
        float startPhase = 0,
        float endPhase = 1
    ) {
        var startFillAmount = image.fillAmount;

        for (int i = 1; i <= steps; i++) {
            var phase = i * 1f / steps;
            if (phase < startPhase) {
                continue;
            }
            if (phase > endPhase) {
                break;
            }
            if (image == null) {
                return;
            }
            image.fillAmount = Mathf.Lerp(startFillAmount, endFillAmount, phase);
            await Helpers.Delay(duration / steps);
        }
    }
}
