using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : Singletone<FPSCounter>
{
    private System.Diagnostics.Stopwatch stopwatch;
    public DateTime lastFPSStatsSaved;

    public long longestFrameLastSecond;
    public long longestFramePreviousSecond;
    public long longFramesLastSecond;
    public long longFramesPreviousSecond;
    public long framesLastSecond;
    public long framesPreviousSecond;

    public void Start() {
        stopwatch = new System.Diagnostics.Stopwatch();
    }

    public void Update() {
        if (DateTime.Now > lastFPSStatsSaved + TimeSpan.FromSeconds(1)) {
            if (longestFrameLastSecond > 16) {
                Debug.LogFormat($"{DateTime.Now.ToString(@"h\:mm\:ss")} Longest frame last second: {longestFrameLastSecond} ms");
            }
            if (longFramesLastSecond > 0) {
                Debug.LogFormat($"{DateTime.Now.ToString(@"h\:mm\:ss")} Long frames last second: {longFramesLastSecond}");
            }
            if (framesLastSecond < 60) {
                Debug.LogFormat($"{DateTime.Now.ToString(@"h\:mm\:ss")} Total frames last second: {framesLastSecond}");
            }
            longestFramePreviousSecond = longestFrameLastSecond;
            longestFrameLastSecond = 0;
            lastFPSStatsSaved = DateTime.Now;
            longFramesPreviousSecond = longFramesLastSecond;
            longFramesLastSecond = 0;
            framesPreviousSecond = framesLastSecond;
            framesLastSecond = 0;
        }
        stopwatch.Stop();
        longestFrameLastSecond = Math.Max(longestFrameLastSecond, stopwatch.ElapsedMilliseconds);
        if (stopwatch.ElapsedMilliseconds > 16) {
            longFramesLastSecond++;
        }
        framesLastSecond++;
        stopwatch.Restart();
    }
}
