using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using RSG;

public class UndestroyableSound : MonoBehaviour
{
	public AudioSource audioSource;

	public float delay = 10;

	public void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	public void Play() {
		transform.SetParent(null);
		audioSource.Play();
		TimeManager.Wait(delay).Then(() => Destroy(gameObject)).Done();
	}
}
