using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Fire : Terrain, IAttacker
{
    public int damage = 1;

    public GameObject attackProjectileSample;

    public virtual bool CanAffect(Unit unit) => !unit.FireImmune && unit.Vulnerable;

    public override void Awake() {
        base.Awake();

        GetComponent<Figure>().collideEnd = async (from, figure) => {
            if (figure == null) {
                return;
            }
            var victim = figure.GetComponent<Unit>();
            if (victim != null) {
                OnLeave();
            }
        };
    }

    protected virtual void OnLeave() {
        gameObject.SetActive(false);
        GetComponent<Figure>().OnDestroy();
    }

    public async Task Die() {
        SoundManager.instance.wolftrapAttack.Play();
        Destroy(gameObject);
    }
}
