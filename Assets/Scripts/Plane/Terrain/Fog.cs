using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Fog : Terrain, IMovable
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
}
