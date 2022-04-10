using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using RSG;
using System;

public class Waiter
{
	Promise ready = new Promise();
	int queueCount = 0;

	void OneReady() {
		Debug.LogFormat("one ready");
		--queueCount;
		Check();
	}

	public void Wait(IPromise wait) {
		++queueCount;
		Debug.LogFormat("wait");
		wait.Then(() => OneReady());
	}

	public IPromise Ready() {
		return ready;
	}

	public void Check() {
		if (ready.CurState == PromiseState.Pending && queueCount == 0) {
			Debug.LogFormat("resolve");
			ready.Resolve();
		}
	}

	public static Waiter Wait() {
		return new Waiter();
	}

	/// <summary>
	/// Wait the specified procedureWithCallback.
	/// </summary>
	/// <returns>The wait.</returns>
	/// <param name="procedureWithCallback">Procedure with callback.</param>
	public static IPromise Wait(Action<Action<IPromise>> procedureWithCallback) {
		var waiter = new Waiter();
		Debug.LogFormat("new waiter");
		procedureWithCallback(waiter.Wait);
		waiter.Check();
		return waiter.Ready();
	}
}
