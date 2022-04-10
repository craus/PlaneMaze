using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cheats : Singletone<Cheats>
{
    public bool hacked = false;
    public bool cheating = false;

    public static bool on => instance.cheating;

    public void Start() {
        if (Extensions.InUnityEditor()) {
            hacked = true;
            cheating = true;
        }
        currentHackKey = hack[0];
    }

    public List<KeyCode> hack = new List<KeyCode>() {
        KeyCode.H,
        KeyCode.A,
        KeyCode.C,
        KeyCode.K
    };

    public KeyCode currentHackKey;

    public void CheckHack() {
        if (Input.GetKeyDown(currentHackKey)) {
            if (currentHackKey == hack.Last()) {
                hacked ^= true;
                GameLog.Message($"hacked: {hacked}");
                cheating ^= true;
                GameLog.Message($"cheating: {cheating}");
                currentHackKey = hack[0];
            } else {
                currentHackKey = hack.Next(currentHackKey);
            }
        } else if (Input.anyKeyDown) {
            currentHackKey = hack[0];
        }
    }

    public void Update() {
        CheckHack();
        if (hacked) {
            if (Input.GetKeyDown(KeyCode.F5)) {
                cheating ^= true;
                GameLog.Message($"cheating: {cheating}");
            }
        }
    }
}
