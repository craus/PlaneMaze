using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WitchAndSister : Monster, IInvisibilitySource
{
    public override bool HasSoul => false;
    public override bool Boss => true;

    public static int CursedSignMinDistance => 2;

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
    [SerializeField] private float defensiveTeleportProbability = 0.25f;
    [SerializeField] private int defensiveTeleportMaxDistance = 2;

    [SerializeField] private List<Cell> chargedArea = null;
    [SerializeField] private int syncronizedAttackTime = int.MinValue;
    [SerializeField] private int syncronizedNonAttackTime = int.MinValue;

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
            TryGenerateCursedSign(attack.from.Location);
            await attack.from.GetComponent<Invulnerability>().Gain(playerInvulnerabilityDuration);
            await Helpers.TeleportAway(attack.from, teleportRadius);
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

    private async Task<Illusion> CreateIllusion(Cell destination) {
        SoundManager.instance.summonCreature.Play();
        var result = Game.GenerateFigure(destination, illusionSample);
        result.GetComponent<Health>().max = GetComponent<Health>().max;
        result.GetComponent<Health>().current = GetComponent<Health>().current;
        result.GetComponent<Health>().UpdateHearts();
        result.creator = this;
        return result;
    }

    private async Task ChargeAttack(IEnumerable<Cell> targetArea) {
        SoundManager.instance.chargeMagicAttack.Play();
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
        figure.Location.Shift(4 * PlayerDelta / 2).Free &&
        figure.Location.Shift(4 * PlayerDelta / 2).GetFigure<TeleportTrap>() == null;


    private void TryGenerateCursedSign(Cell location) {
        if (location.GetFigure<Terrain>(t => t.OccupiesTerrainPlace) != null) {
            Debug.LogFormat($"{this} cannot create cursed sign: there is terrain here");
            return;
        }

        Debug.LogFormat($"{this} creates cursed sign");
        Game.GenerateFigure(location, cursedSignSample);
        Game.instance.GetComponent<CursedSignCounter>().cursedSignCount++;
    }

    protected override async Task MakeMove() { // Slow debug calls
        // Execute Attack
        if (chargedArea != null && chargedArea.Count > 0) {
            Debug.LogFormat($"{this} attack charged area");
            await Task.WhenAll(
                chargedArea.SelectMany(c => c.GetFigures<Unit>(u => u.SoulVulnerable)).ToList().Select(u => Attack(u)).
                Concat(chargedArea.Select(FakeAttack))
            );
            chargedArea = null;
            UpdateIcons();
            return;
        }

        // Aggressive Teleport
        var aggressiveTeleportDestinations = Helpers.Moves.
            Select(m => Player.instance.figure.Location.Shift(2 * m)).
            Where(cell =>
                cell.Biome == Library.instance.darkrootForest &&
                cell.Free
            );
        if (
            (!witch.alive || !sister.alive) &&
            (Player.instance.figure.Location.position - figure.Location.position).MaxDelta() > 2 &&
            Player.instance.figure.Location.Biome == Library.instance.darkrootForest &&
            aggressiveTeleportDestinations.Count() > 0
        ) {
            Debug.LogFormat($"{this} aggressive teleport roll");
            if (Rand.rndEvent(aggressiveTeleportProbability)) {
                Debug.LogFormat($"{this} aggressive teleport");
                await GetComponent<Invulnerability>().Gain(1);
                await Another.GetComponent<Invulnerability>().Gain(1);
                await Helpers.Teleport(figure, aggressiveTeleportDestinations.Rnd());
                var illusionDestinations =
                    (aggressiveTeleportDestinations.Count() >= 2 ?
                    aggressiveTeleportDestinations.RndSelection(2) :
                    aggressiveTeleportDestinations).ToList();
                foreach (var cell in illusionDestinations) {
                    var illusion = await CreateIllusion(cell);
                    await illusion.GetComponent<Invulnerability>().Gain(1);
                }
                await ChargeAttack(AttackArea(PlayerDelta));
                return;
            }
        }

        // Syncronized Attack
        if (Another.syncronizedNonAttackTime != Game.instance.time && SyncronizedAttackCondition) {
            Debug.LogFormat($"{this} syncronized teleport");
            var playerDelta = PlayerDelta;
            syncronizedAttackTime = Game.instance.time;
            await GetComponent<Invulnerability>().Gain(1);
            await Another.GetComponent<Invulnerability>().Gain(1);
            await Helpers.Teleport(Another.figure, figure.Location.Shift(4 * PlayerDelta / 2));
            await Task.WhenAll(
                ChargeAttack(AttackArea(playerDelta)),
                Another.ChargeAttack(AttackArea(playerDelta))
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
        syncronizedNonAttackTime = Game.instance.time;

        // Create Illusion
        var acceptableMoves = Helpers.Moves.Where(move => figure.Location.Shift(move).FreeAndNoWolfTrap).ToList();
        if (
            PlayerDelta.MaxDelta() <= 2 &&
            acceptableMoves.Count() >= 2 &&
            !figure.Location.Vicinity(4).Any(cell => cell.GetFigure<SampleTracker>(
                st => st.createdFromSample == illusionSample.GetComponent<SampleTracker>()
            ))
        ) {
            var oldLocation = figure.Location;
            var selectedMoves = acceptableMoves.RndSelection(2).ToList();
            Debug.LogFormat($"{this} creates illusion at {selectedMoves.ExtToString()}");
            var illusion = await CreateIllusion(figure.Location);
            await Task.WhenAll(
                SmartWalk(selectedMoves[0]),
                illusion.SmartWalk(selectedMoves[1])
            );
            var fog = oldLocation.GetFigure<Fog>();
            if (fog != null) fog.On = true;
            return;
        }

        // Curse player
        if (await CursePlayer()) {
            return;
        }

        // Flee (and probably create sign)
        if (PlayerDelta.MaxDelta() <= 2) {
            Debug.LogFormat($"{this} flee");
            var oldLocation = figure.Location;
            if (await SmartWalkOrFakeMove(-Helpers.StepAtDirection(PlayerDelta))) {
                Debug.LogFormat($"{this} flee success");
                if (
                    !oldLocation.Vicinity(CursedSignMinDistance - 1).Any(cell => cell.GetFigure<CursedSign>())
                ) {
                    TryGenerateCursedSign(oldLocation);
                }
            } else {
                if (Rand.rndEvent(defensiveTeleportProbability)) {
                    await Helpers.TeleportAway(figure, defensiveTeleportMaxDistance);
                }
            }
            return;
        }

        // Wander randomly
        Debug.LogFormat($"{this} wanders randomly");
        await SmartWalkOrFakeMove(Helpers.Moves.Rnd());
    }

    protected abstract Task<bool> CursePlayer();

    public override async Task Die() {
        await base.Die();
        if (!witch.alive && !sister.alive) {
            var win = Game.instance.Win();
        }
    }
}
