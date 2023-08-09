using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Fog : Terrain, IMovable, IOnOccupyingUnitAttackedListener
{
    [SerializeField] private SpriteRenderer sprite;

    public bool on = false;
    public float onProbability = 0.1f;
    public float changeProbability = 0.1f;

    public void Awake() {
        on = Rand.rndEvent(onProbability);
        UpdateSprite();

        new ValueTracker<bool>(() => on, v => {
            on = v;
            UpdateSprite();
        });

        GetComponent<Figure>().collideEnd = async (to, figure) => {
            if (figure == null) {
                return;
            }
            if (GetComponent<Figure>().Location.Neighbours().Contains(to)) {
                var victim = figure.GetComponent<Unit>();
                if (victim != null) {
                    on = false;
                    UpdateSprite();
                }
            }
        };
    }

    private void UpdateSprite() {
        sprite.enabled = on;
    }

    public async Task Move() {
        if (Rand.rndEvent(changeProbability)) {
            if (Rand.rndEvent(onProbability) != on) {
                on ^= true;
                UpdateSprite();
            }
        }
    }

    public void OnOccupyingUnitAttacked(Unit victim) {
        on = false;
        UpdateSprite();
    }
}
