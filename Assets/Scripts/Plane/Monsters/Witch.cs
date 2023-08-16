using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Witch : WitchAndSister
{
    protected override void PlayDeathSound() => SoundManager.instance.witchDeath.Play();
}
