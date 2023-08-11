﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Fog : Terrain, IMovable, IOnOccupyingUnitAttackedListener
{
    [SerializeField] private SpriteRenderer sprite;

    [SerializeField] private bool on = false;
    public bool On {
        get => on;
        set {
            on = value;
            UpdateSprite();
            CheckUnitsInvisibility();
        }
    }
    public float onProbability = 0.1f;
    public float changeProbability = 0.1f;

    public void OnGameStart() {
        CheckUnitsInvisibility();
    }

    private void CheckUnitsInvisibility() {
        var location = GetComponent<Figure>().Location;
        if (location != null) {
            foreach (var i in location.GetFigures<Invisibility>()) {
                i.Check();
            }
        }
    }

    public void Awake() {
        On = Rand.rndEvent(onProbability);

        new ValueTracker<bool>(() => On, v => On = v);

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
            {
                var victim = figure.GetComponent<Unit>();
                if (victim != null) {
                    victim.GetComponent<Invisibility>().Check();
                }
            }
        };

        GetComponent<Figure>().collide = async (to, figure) => {
            if (figure == null) {
                return;
            }
            if (!On) {
                return;
            }
            var victim = figure.GetComponent<Unit>();
            if (victim != null) {
                victim.GetComponent<Invisibility>().Check();
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