using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlaneMaze
{
    public class Cheats : global::Cheats
    {
        public static new Cheats instance => global::Cheats.instance as Cheats;

        public bool trueSight = false;

        public override void Update() {
            base.Update();
            if (hacked) {
                if (Input.GetKeyDown(KeyCode.T)) {
                    trueSight ^= true;
                    Player.instance.GlobalInvisibilityCheck();
                    GameLog.Message($"trueSight: {trueSight}");
                }
            }
        }
    }
}