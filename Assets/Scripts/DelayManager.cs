using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DelayManager : Singletone<DelayManager>
{
    public static PriorityQueue<Weighted<TaskCompletionSource<bool>>> queue = 
        new PriorityQueue<Weighted<TaskCompletionSource<bool>>>();

    public static Task Delay(float duration) {
        var newTask = new TaskCompletionSource<bool>();
        queue.Enqueue(new Weighted<TaskCompletionSource<bool>>(newTask, Time.time+duration));
        return newTask.Task;
    }

    public void Update() {
        while (queue.Count > 0 && queue.Peek().weight < Time.time) {
            queue.Dequeue().to.SetResult(true);
        }
    }
}
