using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Fire : Terrain, IAttacker, IMovable
{
    public int damage = 1;

    public int lifetime;

    public GameObject attackProjectileSample;

    public SpriteRenderer sprite;

    public virtual bool CanAffect(Unit unit) => !unit.FireImmune && unit.Vulnerable;
    public override bool Scaring(Unit unit) => CanAffect(unit);

    public override void Awake() {
        base.Awake();

        lifetime = Rand.rnd(16, 32);
        UpdateSprite();

        GetComponent<Figure>().collideEnd = async (figure) => {
            if (figure == null) {
                return;
            }
            var victim = figure.GetComponent<Unit>();
            if (victim != null) {
                OnLeave();
            }
        };

        new ValueTracker<int>(() => lifetime, v => { lifetime = v; UpdateSprite(); });
    }

    protected virtual void OnLeave() {
        gameObject.SetActive(false);
        GetComponent<Figure>().OnDestroy();
    }

    public async virtual Task Move() {
        lifetime--;
        if (lifetime == 0) {
            gameObject.SetActive(false);
            GetComponent<Figure>().OnDestroy();
        }
        UpdateSprite();
    }

    private void UpdateSprite() {
        sprite.color = sprite.color.withAlpha(lifetime == 1 ? 0.5f : 1);
    }
}
