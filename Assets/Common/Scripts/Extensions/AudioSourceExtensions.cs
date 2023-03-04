using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class AudioSourceExtensions
{
    public static void PlayBackwards(this AudioSource a) {
        a.timeSamples = a.clip.samples - 1;
        a.Play();
    }
}