using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Sister : WitchAndSister
{
    protected override void PlayDeathSound() => SoundManager.instance.sisterDeath.Play();
}
