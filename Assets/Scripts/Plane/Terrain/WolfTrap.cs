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
            if (victim != null) {
                await Attack(victim);
            }
        };
    }

    private async Task Attack(Unit victim) {
        var ap = Instantiate(attackProjectileSample);
        ap.transform.position = victim.transform.position;
        await Task.Delay(100);
        await victim.GetComponent<Health>().Hit(damage);
        Destroy(ap);
    }
}
