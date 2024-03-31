using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Witch : WitchAndSister
{
    protected override void PlayDeathSound() => SoundManager.instance.witchDeath.Play();

    protected override async Task<bool> CursePlayer() {
        if (PlayerDelta.SumDelta() <= 1 && Player.instance.GetComponent<Curse>().Current == 0) {
            Debug.LogFormat($"{this} curses player");
            await Player.instance.GetComponent<Curse>().Gain(13);
            return true;
        }
        return false;
    }
}
