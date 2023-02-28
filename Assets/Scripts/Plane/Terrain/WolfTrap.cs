using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class WolfTrap : MonoBehaviour
{
    public int damage = 1;

    public GameObject attackProjectileSample;

    public void Awake() {
        GetComponent<Figure>().collide = async (from, figure) => {
            var victim = figure.GetComponent<Unit>();
            if (victim != null && !victim.Flying) {
                await Attack(victim);
            }
        };
    }

    private async Task Attack(Unit victim) {
        if (!Game.CanAttack(null, victim)) {
            return;
        }
        SoundManager.instance.wolftrapAttack.Play();

        var ap = Instantiate(attackProjectileSample);
        ap.transform.position = victim.transform.position;
        await Helpers.Delay(0.1f);
        await victim.Hit(new Attack(GetComponent<Figure>().location, victim.figure.location, damage));
        Destroy(ap);
    }
}
