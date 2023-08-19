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

    public override bool OccupiesTerrainPlace => base.OccupiesTerrainPlace && On;

    [SerializeField] private bool on = false;
    public bool On {
        get => on;
        set {
            if (
                value == false ||
                figure.Location != null &&
                !figure.Location.GetFigure<Figure>(f => f != GetComponent<Figure>())
            ) {
                on = value;
                UpdateSprite();
                CheckUnitsInvisibility();
            }
        }
    }
    public float onProbability = 0.1f;
    public float changeProbability = 0.1f;

    public void OnGameStart() {
        On = Rand.rndEvent(onProbability);
        CheckUnitsInvisibility();
    }

    private void CheckUnitsInvisibility() {
        var location = GetComponent<Figure>().Location;
        if (location == null) {
            return;
        }
        foreach (var i in location.GetFigures<Invisibility>()) {
            i.Check();
        }
        if (location.GetFigure<Player>() != null) {
            Player.instance.GlobalInvisibilityCheck();
        }
    }

    public override void Awake() {
        base.Awake();

        new ValueTracker<bool>(() => On, v => {
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
                    On = false;
                }
            }
        };
    }

    private void UpdateSprite() {
        sprite.enabled = On;
    }

    public async Task Move() {
        if (Rand.rndEvent(changeProbability)) {
            if (Rand.rndEvent(onProbability) != On) {
                On ^= true;
            }
        }
    }

    public void OnOccupyingUnitAttacked(Unit victim) {
        On = false;
    }
}
