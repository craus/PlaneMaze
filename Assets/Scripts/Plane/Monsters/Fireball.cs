using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Fireball : Monster
{
    public override bool Flying => true;
    public override bool Vulnerable => false;
    public override bool OccupiesPlace => false;
    public override int Money => 0;
    public override bool HasSoul => false;

    public Vector2Int currentDirection;
    public Transform spriteParent;

    protected override void PlayDeathSound() => SoundManager.instance.explode.Play();

    public override void Awake() {
        base.Awake();
        UpdateSprite();

        new ValueTracker<Vector2Int>(() => currentDirection, v => {
            currentDirection = v; UpdateSprite();
        });

        GetComponent<Figure>().collide = async (to, figure) => {
            if (figure == null) {
                return;
            }
            if (figure.GetComponent<Unit>() != null && figure.GetComponent<Unit>().OccupiesPlace) {
                await Explode();
            }
        };
    }

    public void UpdateSprite() {
        spriteParent.transform.rotation = Quaternion.Euler(0, 0, currentDirection.PolarAngle());
    }

    private void TryFire(Cell location) {
        if (location.GetFigure<Terrain>() != null) return;
        Game.GenerateFigure(location, Library.instance.fire);
    }

    public override void PlayAttackSound() {
        // do nothing
    }

    private async Task Explode() {
        var victim = figure.Location.GetFigure<Unit>(u => u != this && u.OccupiesPlace);

        await Helpers.RunAnimation(Library.instance.explosionSample, transform);

        if (victim != null) {
            await Attack(victim);
        }

        foreach (var cell in figure.Location.Vicinity(1).Where(c => c.Free && c.figures.Count == 0)) {
            TryFire(cell);
        }

        await Die();
    }

    public async Task CheckExplode() {
        var target = figure.Location.GetFigure<Unit>(u => u != this && u.OccupiesPlace);
        if (target != null) {
            await Explode();
        }
    }

    protected override async Task MakeMove() {
        if (figure.Location.Shift(currentDirection).Wall) {
            await Explode();
            return;
        }
        await figure.Move(figure.Location.Shift(currentDirection));
        await CheckExplode();
    }
}
