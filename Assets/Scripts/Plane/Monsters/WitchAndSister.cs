using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class WitchAndSister : Monster, IInvisibilitySource
{
    public override bool HasSoul => false;
    public override bool Boss => true;

    public bool Invisible => (Player.instance.figure.Location.position - GetComponent<Figure>().Location.position).MaxDelta() > 2;

    public Witch witch;
    public Sister sister;

    public WitchAndSister Another => this == witch ? sister as WitchAndSister : witch;

    [SerializeField] private CursedSign cursedSignSample;
    [SerializeField] private Illusion illusionSample;

    [SerializeField] private List<Transform> targets;

    [SerializeField] private int teleportRadius = 8;
    [SerializeField] private int playerInvulnerabilityDuration = 2;
    [SerializeField] private float aggressiveTeleportProbability = 0.1f;

    [SerializeField] private List<Cell> chargedArea = null;
    [SerializeField] private int syncronizedAttackTime = int.MinValue;

    public override void Awake() {
        base.Awake();

        new ValueTracker<List<Cell>>(() => chargedArea, v => {
            chargedArea = v;
            UpdateIcons();
        });
    }

    public override async Task Hit(Attack attack) {
        await base.Hit(attack);
        if (alive) {
            attack.afterAttack.Add(async () => {
                await attack.from.GetComponent<Invulnerability>().Gain(playerInvulnerabilityDuration);
                await Helpers.TeleportAway(attack.from, teleportRadius);
            });
        }
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

    private async Task CreateIllusion(Cell destination) {
        Game.instance.GenerateFigure(destination, illusionSample);
    }

    private async Task ChargeAttack(IEnumerable<Cell> targetArea) {
        chargedArea = targetArea.ToList();
        UpdateIcons();
    }

    private IEnumerable<Cell> AttackArea(Vector2Int playerDelta) =>
        new List<Cell> {
            figure.Location.Shift(playerDelta/2),
            figure.Location.Shift(2*playerDelta/2),
            figure.Location.Shift(3*playerDelta/2),
        };

    protected bool SyncronizedAttackCondition =>
        PlayerDelta.MaxDelta() == 2 && PlayerDelta.MinDelta() == 0 &&
        figure.Location.Shift(PlayerDelta / 2).Free &&
        witch.alive && sister.alive &&
        figure.Location.Shift(3 * PlayerDelta / 2).Free &&
        figure.Location.Shift(4 * PlayerDelta / 2).Free;

    protected override async Task MakeMove() {
        // Execute Attack
        if (chargedArea != null && chargedArea.Count > 0) {
            Debug.LogFormat($"{this} attack charged area");
            await Task.WhenAll(chargedArea.SelectMany(c => c.GetFigures<Unit>()).ToList().Select(u => Attack(u)));
            chargedArea = null;
            UpdateIcons();
            return;
        }

        // Aggressive Teleport
        var aggressiveTeleportDestinations = Moves.
            Select(m => Player.instance.figure.Location.Shift(2 * m)).
            Where(cell =>
                cell.biome == Library.instance.darkrootForest &&
                cell.Free
            );
        if (
            (!witch.alive || !sister.alive) &&
            (Player.instance.figure.Location.position - figure.Location.position).MaxDelta() > 2 &&
            Player.instance.figure.Location.biome == Library.instance.darkrootForest &&
            aggressiveTeleportDestinations.Count() > 0
        ) {
            Debug.LogFormat($"{this} aggressive teleport roll");
            if (Rand.rndEvent(aggressiveTeleportProbability)) {
                Debug.LogFormat($"{this} aggressive teleport");
                await Helpers.Teleport(figure, aggressiveTeleportDestinations.Rnd());
                var illusionDestinations =
                    (aggressiveTeleportDestinations.Count() >= 2 ?
                    aggressiveTeleportDestinations.RndSelection(2) :
                    aggressiveTeleportDestinations).ToList();
                foreach (var cell in illusionDestinations) {
                    await CreateIllusion(cell);
                }
                await ChargeAttack(AttackArea(PlayerDelta));
                return;
            }
        }

        // Syncronized Attack
        if (SyncronizedAttackCondition) {
            Debug.LogFormat($"{this} syncronized teleport");
            syncronizedAttackTime = Game.instance.time;
            await Helpers.Teleport(Another.figure, figure.Location.Shift(4 * PlayerDelta / 2));
            await Task.WhenAll(
                ChargeAttack(AttackArea(PlayerDelta)),
                Another.ChargeAttack(AttackArea(PlayerDelta))
            );
            return;
        }
        if (Another.SyncronizedAttackCondition) {
            Debug.LogFormat($"{this} waits for syncronized attack from paired boss");
            return;
        }
        if (Another.syncronizedAttackTime == Game.instance.time) {
            Debug.LogFormat($"{this} skips move because of syncronized attack from paired boss");
            return;
        }

        // Create Illusion
        var acceptableMoves = Moves.Where(move => figure.Location.Shift(move).FreeAndNoWolfTrap).ToList();
        if (
            PlayerDelta.MaxDelta() <= 2 &&
            acceptableMoves.Count() >= 2 &&
            !figure.Location.Vicinity(4).Any(cell => cell.GetFigure<SampleTracker>(
                st => st.createdFromSample == illusionSample.GetComponent<SampleTracker>()
            ))
        ) {
            var oldLocation = figure.Location;
            var selectedMoves = acceptableMoves.RndSelection(2).ToList();
            var illusionDestination = figure.Location.Shift(selectedMoves[1]);
            Debug.LogFormat($"{this} creates illusion at {selectedMoves.ExtToString()}");
            await SmartWalk(selectedMoves[0]);
            await CreateIllusion(illusionDestination);
            var fog = oldLocation.GetFigure<Fog>();
            if (fog != null) fog.On = true;
            return;
        }

        // Curse player
        if (PlayerDelta.SumDelta() <= 1 && Player.instance.GetComponent<Curse>().Current == 0) {
            Debug.LogFormat($"{this} curses player");
            await Player.instance.GetComponent<Curse>().Gain(13);
            return;
        }

        // Flee (and probably create sign)
        if (PlayerDelta.MaxDelta() <= 2) {
            Debug.LogFormat($"{this} flee");
            var oldLocation = figure.Location;
            if (await SmartWalkOrFakeMove(-Helpers.StepAtDirection(PlayerDelta))) {
                Debug.LogFormat($"{this} flee success");
                if (
                    oldLocation.GetFigure<Terrain>(t => t.OccupiesTerrainPlace) == null && 
                    !oldLocation.Vicinity(4).Any(cell => cell.GetFigure<CursedSign>())
                ) {
                    Debug.LogFormat($"{this} creates cursed sign");
                    Game.instance.GenerateFigure(oldLocation, cursedSignSample);
                }
            }
            return;
        }

        // Wander randomly
        Debug.LogFormat($"{this} wanders randomly");
        await SmartWalkOrFakeMove(Moves.Rnd());
    }

    public override async Task Die() {
        await base.Die();
        if (!witch.alive && !sister.alive) {
            await Game.instance.Win();
        }
    }
}
