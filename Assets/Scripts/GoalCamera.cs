using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoalCamera : Singletone<GoalCamera>
{
    public Cell goal;
    public new Camera camera;

    public void Update() {
        if (Game.instance.gem) {
            camera.transform.position = Game.instance.gem.transform.position.Change(z: camera.transform.position.z);
        }
    }
}
