using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Sister : WitchAndSister
{
    protected override void PlayDeathSound() => SoundManager.instance.sisterDeath.Play();

    protected override async Task<bool> CursePlayer() {
        if (PlayerDelta.SumDelta() <= 1 && Player.instance.GetComponent<Confusion>().Current == 0) {
            Debug.LogFormat($"{this} confuses player");
            await Player.instance.GetComponent<Confusion>().Gain(2);
            return true;
        }
        return false;
    }
}
