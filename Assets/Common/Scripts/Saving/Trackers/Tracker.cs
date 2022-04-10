using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Tracker
{
	public static event Action<Object> onSave;

	public static void Track<T>(Action<T> setValue, Func<T> getValue) {
		new ValueTracker<T>(setValue, getValue);
	}

	public static Object Save() {
		var save = new Object();
		onSave.Invoke(save);
		return save;
	}

//	public static Object Track() {
//	}
}