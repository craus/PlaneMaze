using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public GameObject sample;

    public List<GameObject> gameObjects;
    public List<bool> objectIsActive = new List<bool>();

    public Stopwatch stopwatch;

    public void Awake() {
        stopwatch = new Stopwatch();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            stopwatch.Restart();
            for (int i = 0; i < 10000; i++) {
                gameObjects.Add(Instantiate(sample).gameObject);
                objectIsActive.Add(true);
            }
            stopwatch.Stop();
            UnityEngine.Debug.LogFormat($"[{stopwatch.ElapsedMilliseconds}] Created 1000 bows; now we have {gameObjects.Count} bows");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            stopwatch.Restart();
            foreach (var go in gameObjects) {
                go.SetActive(true);
            }
            stopwatch.Stop(); 
            UnityEngine.Debug.LogFormat($"[{stopwatch.ElapsedMilliseconds}] go.SetActive(true); {gameObjects.Count} times");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            stopwatch.Restart();
            foreach (var go in gameObjects) {
                go.SetActive(false);
            }
            stopwatch.Stop();
            UnityEngine.Debug.LogFormat($"[{stopwatch.ElapsedMilliseconds}] go.SetActive(false); {gameObjects.Count} times");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            stopwatch.Restart();
            foreach (var go in gameObjects) {
                if (!go.activeSelf) go.SetActive(true);
            }
            stopwatch.Stop();
            UnityEngine.Debug.LogFormat($"[{stopwatch.ElapsedMilliseconds}]" +
                $" if (!go.activeSelf) go.SetActive(true); { gameObjects.Count} times");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            stopwatch.Restart();
            foreach (var go in gameObjects) {
                if (go.activeSelf) go.SetActive(false);
            }
            stopwatch.Stop();
            UnityEngine.Debug.LogFormat($"[{stopwatch.ElapsedMilliseconds}]" +
                $" if (!go.activeSelf) go.SetActive(false); { gameObjects.Count} times");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            stopwatch.Restart();
            for (int i = 0; i < gameObjects.Count; i++) {
                if (!objectIsActive[i]) {
                    gameObjects[i].SetActive(true);
                    objectIsActive[i] = true;
                }
            }
            stopwatch.Stop();
            UnityEngine.Debug.LogFormat($"[{stopwatch.ElapsedMilliseconds}]" +
                $" setActive(true) (bool var) { gameObjects.Count} times");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            stopwatch.Restart();
            for (int i = 0; i < gameObjects.Count; i++) {
                if (objectIsActive[i]) {
                    gameObjects[i].SetActive(false);
                    objectIsActive[i] = false;
                }
            }
            stopwatch.Stop();
            UnityEngine.Debug.LogFormat($"[{stopwatch.ElapsedMilliseconds}]" +
                $" setActive(false) (bool var) { gameObjects.Count} times");
        }
        if (Input.GetKeyDown(KeyCode.N)) {
            UnityEngine.Debug.LogFormat($"Active Objects: {gameObjects.Count(g => g.activeSelf)}");
        }
    }
}
