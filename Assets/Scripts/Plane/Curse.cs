using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Curse : Buff
{
    public GenericAttacker attacker;

    public override async Task Gain(int amount) {
        await base.Gain(amount);
        Debug.LogFormat($"{this} gain {amount} curse");
        SoundManager.instance.gainCurse.Play();
    }
}
