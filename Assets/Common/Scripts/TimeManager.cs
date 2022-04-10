using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RSG;

public class TimeManager : Singletone<TimeManager>
{
	static PromiseTimer promiseTimer = new PromiseTimer();

    static float lastUpdateTime = 0;

    public static float Time() {
        return UnityEngine.Time.time;
    }

    public void Update()
    {
        var seconds = Time() - lastUpdateTime;
        if (DebugManager.instance.slowAnimations)
        {
            seconds /= 100;
        }
        promiseTimer.Update(seconds);
        lastUpdateTime = Time();
    }

    public static IPromise Wait(float seconds) {
        return promiseTimer.WaitFor(seconds);
    }
}
