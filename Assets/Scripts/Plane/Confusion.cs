using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Confusion : Buff
{
    public override async Task Gain(int amount) {
        SoundManager.instance.gainConfusion.Play();
        await base.Gain(amount);
    }
}
