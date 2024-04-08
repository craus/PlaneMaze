using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Archdevil : Monster
{
    public override bool HasSoul => false;
    public override bool Boss => true;
    public override bool FireImmune => true;

    [SerializeField] private HellGate hellGateSample;
    [SerializeField] private TeleportTrap teleportTrapSample;

    [SerializeField] private List<Transform> targets;
    [SerializeField] private List<Cell> chargedArea = null;

    public override void Awake() {
        base.Awake();

        new ValueTracker<List<Cell>>(() => chargedArea, v => {
            chargedArea = v;
            UpdateIcons();
        });
    }

    public override async Task Hit(Attack attack) {
        await base.Hit(attack);
        attack.afterAttack.Add(async () => {
            await GetComponent<Invulnerability>().Gain(3);
        });
    }

    private void UpdateIcons() {
        if (chargedArea == null) {
            targets.ForEach(t => t.gameObject.SetActive(false));
            return;
        }
        for (int i = 0; i < targets.Count; i++) {
            if (i < chargedArea.Count()) {
                targets[i].gameObject.SetActive(true);
                targets[i].position = chargedArea[i].transform.position.Change(z: targets[i].position.z);
            } else {
                targets[i].gameObject.SetActive(false);
            }
        }
    }

    public override void PlayAttackSound() => SoundManager.instance.monsterRangedAttack.Play();
    protected override void PlayDeathSound() => SoundManager.instance.archDevilDeath.Play();

    private async Task ChargeAttack(IEnumerable<Cell> targetArea) {
        SoundManager.instance.chargeMagicAttack.Play();
        chargedArea = targetArea.ToList();
        UpdateIcons();
    }

    private IEnumerable<Cell> DirectAttackArea(Vector2Int playerDelta) =>
        new List<Cell> {
            figure.Location.Shift(playerDelta),
            figure.Location.Shift(2*playerDelta),
            figure.Location.Shift(3*playerDelta),
            figure.Location.Shift(4*playerDelta),
        };

    private IEnumerable<Cell> VerticalLine(int x) {
        for (int dy = -4; dy <= 4; dy++) {
            yield return figure.Location.board.GetCell(x, figure.Location.position.y + dy);
        }
    }

    private IEnumerable<Cell> HorizontalLine(int y) {
        for (int dx = -4; dx <= 4; dx++) {
            yield return figure.Location.board.GetCell(figure.Location.position.x + dx, y);
        }
    }

    private IEnumerable<IEnumerable<Cell>> PossibleFireLines() {
        var aa = Vector2Int.Min(Player.instance.figure.Location.position, figure.Location.position);
        var bb = Vector2Int.Max(Player.instance.figure.Location.position, figure.Location.position);
        for (int x = aa.x + 1; x < bb.x; x++) {
            yield return VerticalLine(x);
        }
        for (int y = aa.y + 1; y < bb.y; y++) {
            yield return HorizontalLine(y);
        }
    }

    private async Task MakePrimaryMove() {
        // Execute Attack
        if (chargedArea != null && chargedArea.Count > 0) {
            Debug.LogFormat($"{this} attack charged area");
            await Task.WhenAll(
                chargedArea.SelectMany(c => c.GetFigures<Unit>(u => !u.FireImmune)).ToList().Select(u => Attack(u)).
                Concat(chargedArea.Select(FakeAttack)).
                Concat(chargedArea.Select(IgniteIfFree))
            );
            if (chargedArea.Count() == 9) {
                // switch to attack phase 2
                await ChargeAttack(figure.Location.Vicinity(2));
            } else {
                chargedArea = null;
            }
            UpdateIcons();
            return;
        }

        // Charge direct attack
        if (PlayerDelta.MinDelta() == 0 && PlayerDelta.MaxDelta() <= 4) {
            var playerDelta = Helpers.StepAtDirection(PlayerDelta);
            await ChargeAttack(DirectAttackArea(playerDelta));
            return;
        }

        // Create HellGate
        if (PlayerDelta.MaxDelta() <= 4 && !figure.Location.Vicinity(4).Any(cell => cell.GetFigure<HellGate>())) {
            var targetCell = figure.Location.Vicinity(4).Where(cell => cell.Free && cell.figures.Count == 0).Rnd();
            if (targetCell != null) {
                Game.GenerateFigure(targetCell, hellGateSample);
                return;
            }
        }

        // Create TeleportTrap
        if (PlayerDelta.MaxDelta() <= 4 && !figure.Location.Vicinity(4).Any(cell => cell.GetFigure<TeleportTrap>())) {
            var targetCell = Helpers.Diagonals(figure.Location)
                .Where(cell => cell.Free && cell.figures.Count == 0)
                .MinBy(cell => Vector2Int.Distance(cell.position, Player.instance.figure.Location.position));

            if (targetCell != null) {
                Game.GenerateFigure(targetCell, teleportTrapSample);
                return;
            }
        }

        // Create firewall
        if (PlayerDelta.MaxDelta() <= 4) {
            var targetFireLine = PossibleFireLines().ToList().rnd();
            if (targetFireLine != null) {
                await Task.WhenAll(targetFireLine.Select(IgniteIfFree));
            }
            return;
        }

        // Wander randomly
        Debug.LogFormat($"{this} wanders randomly");
        for (int i = 0; i < 1; i++) {
            if (Rand.rndEvent(0.5f)) {
                var delta = Helpers.Moves.Rnd();
                if (!await TryAttack(delta)) {
                    if (!await SmartWalk(delta)) {
                        await figure.FakeMove(delta);
                    }
                }
            }
        }
    }

    protected override async Task MakeMove() { // Slow debug calls
        await MakePrimaryMove();
        if (movesSinceLastHit == 1) {
            await ChargeAttack(figure.Location.Vicinity(1));
        }
    }

    private void TryFire(Cell location) {
        if (location.GetFigure<Terrain>() != null) return;
        Game.GenerateFigure(location, Library.instance.fire);
    }

    private async Task IgniteIfFree(Cell cell) {
        if (cell.Free && cell.figures.Count == 0) {
            TryFire(cell);
        }
    }

    public override async Task Die() {
        await base.Die();
        var win = Game.instance.Win();
    }
}
