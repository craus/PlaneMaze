using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Curse : Buff
{
    public GenericAttacker attacker;

    [SerializeField] private int incoming = 0;

    public override async Task Gain(int amount) {
        incoming = amount;
        Debug.LogFormat($"{this} gain {amount} curse");
        SoundManager.instance.gainCurse.Play();
    }

    public async Task Prepare() {
        await base.Gain(incoming);
        incoming = 0;
    }
}
