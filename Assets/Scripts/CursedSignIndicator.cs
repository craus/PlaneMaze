using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CursedSignIndicator : Singletone<CursedSignIndicator>
{
    public Animator animator;
    public string warningBool;
    public string dangerBool;
    public string attackTrigger;

    public void Update() {
        if (Game.instance == null) {
            return;
        }
        var cnt = Game.instance.GetComponent<CursedSignCounter>().cursedSignCount;
        if (cnt <= 0) {
            animator.SetBool(warningBool, false);
            animator.SetBool(dangerBool, false);
        } else if (cnt < CursedSignCounter.Max) {
            animator.SetBool(warningBool, true);
            animator.SetBool(dangerBool, false);
        } else {
            animator.SetBool(warningBool, true);
            animator.SetBool(dangerBool, true);
        }
    }

    public void Attack() {
        animator.SetTrigger(attackTrigger);
    }
}
