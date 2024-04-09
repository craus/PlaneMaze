using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Figure))]
public class Poison : Buff
{
    public Image amountIcon;

    public override void UpdateIcons() {
        base.UpdateIcons();
        amountIcon.fillAmount = Current / 16f;
    }

    public override async Task Spend(int amount) {
        var old = Current;
        await base.Spend(amount);
        if (old > 0 && Current == 0 && GetComponent<Unit>().Vulnerable && !GetComponent<Unit>().PoisonImmune) {
            await Helpers.RunAnimation(Library.instance.poisonDamageSample, transform);
            await GetComponent<Health>().Hit(1);
        }
    }
}
