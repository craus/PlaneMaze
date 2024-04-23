using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    public static Game instance => GameManager.instance ? GameManager.instance.game : null;

    public static bool fasterMonsters;
    public static bool monstersRegenerate;

    public Player playerSample;
    public Player player;
    public GameEvents gameEvents;

    public List<Weapon> weaponSamples;
    public List<Item> itemSamples;
    public List<Figure> startingItemsSamples;

    public List<Store> storeSamples;
    public Portal portalSample;
    public HealingPotion healingPotionSample;
    public PaidCell paidCellSample;
    public Info infoSample;
    public RingOfTerraforming ringOfTerraformingSample;

    public Ghost ghostSample;
    public Lich lichSample;
    public Gem gemSample;

    public string bossName;

    public int unlockedCells = (int)1e9;

    public int time = 0;
    public int moveNumber = 0;
    public int ghostSpawnTimeReductionHalfLife = 1000;
    public float ghostSpawnProbabilityPerTurn;

    public Board boardSample;
    public Board mainWorld;
    public List<Board> stores;

    public List<Func<int, Task>> afterPlayerMove = new List<Func<int, Task>>();
    public List<Func<int, Task>> afterMonsterMove = new List<Func<int, Task>>();
    public List<Func<Task>> afterGameStart = new List<Func<Task>>();

    public HashSet<(Cell, Cell)> contaminations = new HashSet<(Cell, Cell)>();
    public HashSet<Cell> clearedCells = new HashSet<Cell>();

    public Map<int, TaskCompletionSource<bool>> completedTurns = new Map<int, TaskCompletionSource<bool>>(() => new TaskCompletionSource<bool>());

    public Metagame Metagame => GameManager.instance.metagame;

    public DateTime startTime;

    public bool gameOver = false;

    public void Awake() {
        new ValueTracker<int>(() => time, v => {
            time = v;
            completedTurns.Clear();
        });
        new ValueTracker<int>(() => moveNumber, v => moveNumber = v);
        new ValueTracker<bool>(() => gameOver, v => gameOver = v);
        MusicManager.instance.CreateValueTrackers();

        fasterMonsters = Metagame.instance.HasAscention<FasterMonsters>();
        monstersRegenerate = Metagame.instance.HasAscention<MonstersHeal>();
    }

    public async void Start() {
        startTime = DateTime.Now;

        InfoPanel.instance.viewedInfo = new HashSet<IExplainable>();

        mainWorld = Instantiate(boardSample, transform);
        mainWorld.currentBiome = Library.instance.dungeon;
        UnityEngine.Debug.LogFormat("New game started");

        //await GenerateTestWorld();
        await WorldGenerator.instance.GenerateWorld();

        mainWorld.movables.ToList().ForEach(m => {
            m.OnGameStart();
        });

        await Task.WhenAll(afterGameStart.Select(f => f()));

        MusicManager.instance.Switch(MusicManager.instance.playlist);
        UndoManager.instance.Save();

        await ConfirmationManager.instance.AskConfirmation(
            panel: ConfirmationManager.instance.startPanel, 
            canCancel: false,
            canConfirmByAnyButton: true
        );
    }

    public async Task AskForNextRun() {
        if (await ConfirmationManager.instance.AskConfirmation("Do you want to start another run? If you start, abandon run will be counted as a loss!")) {
            GameManager.instance.RestartGame();
        } else {
            MainUI.instance.QuitApplication();
        }
    }

    public async Task Win() {
        if (gameOver) return;
        gameOver = true;

        Debug("Win");
        await Player.instance.ongoingAnimations;
        Debug("Player move completed, continue to win sequence");

        MusicManager.instance.Switch(MusicManager.instance.winPlaylist);
        await ConfirmationManager.instance.AskConfirmation(
            message: $"Victory! You killed {bossName} and saved the world!",
            panel: ConfirmationManager.instance.winPanel, 
            canCancel: false
        );
        await GameManager.instance.metagame.Win();
        await AskForNextRun();
    }

    public async Task Lose() {
        if (gameOver) return;
        gameOver = true;
        Debug("Lose");
        await Player.instance.ongoingAnimations;
        Debug("Player move completed, continue to lose sequence");

        MusicManager.instance.Switch(MusicManager.instance.losePlaylist);
        if (Player.instance.lastAttacker != null) {
            await ConfirmationManager.instance.AskConfirmation(
                canCancel: false,
                panel: ConfirmationManager.instance.infoPanel,
                customShow: () => InfoPanel.instance.Show(
                    Player.instance.lastAttacker.GetComponent<IExplainable>(),
                    repeatable: true
                ),
                canConfirmByAnyButton: true
            );
        }
        await ConfirmationManager.instance.AskConfirmation(panel: ConfirmationManager.instance.losePanel, canCancel: false);
        await GameManager.instance.metagame.Lose();
        await AskForNextRun();
    }

    public async void Update() {
        if (Cheats.on) {
            if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.LeftShift)) {
                await Win();
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                await Lose();
            }
        }
    }

    public static void Debug(string message) {
        UnityEngine.Debug.LogFormat($"[{instance.time}] [{instance.moveNumber}] {message}");
    }

    public void AddGem(Cell cell, int amount) {
        if (amount <= 0) {
            return;
        }
        var oldGem = cell.GetFigure<Gem>();
        if (oldGem != null) {
            oldGem.amount += amount;
            oldGem.UpdateSprite();
        } else {
            var newGem = GenerateFigure(cell, gemSample);
            newGem.amount = amount;
            newGem.UpdateSprite();
        }
    }

    private async Task MonstersAndItemsTick(int turnNumber) {
        Debug($"Monsters before move");
        await Task.WhenAll(Player.instance.figure.Location.board.movables.ToList().Select(m => m.BeforeMove()));

        Debug($"Monsters move");
        await Task.WhenAll(Player.instance.figure.Location.board.movables.ToList().Select(m => m.Move()).Concat(afterPlayerMove.Select(listener => listener(turnNumber))));
        if (this == null) {
            return;
        }
        await Task.WhenAll(afterMonsterMove.Select(listener => listener(turnNumber)));
    }

    private void TryFire(Cell location) {
        if (location.figures.Count != 0) return;
        GenerateFigure(location, Library.instance.fire);
    }

    public async Task AfterPlayerMove() {
        await MonstersAndItemsTick(time);
        completedTurns[time].SetResult(true);
        time++;

        if (Metagame.SpawnGhosts) {
            await SpawnGhosts();
        }

        var inferno = WorldGenerator.instance.GetBiome<Biome>(b => b.GetComponent<Inferno>() != null);
        if (inferno != null) {
            TryFire(inferno.cells.rnd());
        }
    }

    private async Task SpawnGhosts() {
        if (Player.instance == null) return;
        if (player.figure.Location.board != mainWorld) return;

        ghostSpawnProbabilityPerTurn = 1 - Mathf.Pow(
            1 - Metagame.GhostSpawnProbabilityPerTurn(time),
            Player.instance.figure.Location.Biome.ghostPower
        );

        for (int i = 0; i < Metagame.MaxGhostSpawnsPerTurn && Rand.rndEvent(ghostSpawnProbabilityPerTurn); i++) {
            await SpawnGhost();
        }
    }

    public Map<Figure, List<Figure>> generatedFigures = new Map<Figure, List<Figure>>(() => new List<Figure>());
    public static T GenerateFigure<T>(Cell cell, T sample) where T : MonoBehaviour {
        var f = Instantiate(sample);
        f.gameObject.name = $"{sample.gameObject.name} #{instance.generatedFigures[sample.GetComponent<Figure>()].Count}";
        instance.generatedFigures[sample.GetComponent<Figure>()].Add(f.GetComponent<Figure>());

        if (f.GetComponent<SampleTracker>() != null) {
            f.GetComponent<SampleTracker>().createdFromSample = sample.GetComponent<SampleTracker>();
        }

        _ = f.GetComponent<Figure>().Move(cell, isTeleport: true);

        var explainable = f.GetComponent<IExplainable>();
        if (explainable != null) {
            explainable.Sample = sample.GetComponent<IExplainable>();
        }

        var iMovable = f.GetComponent<IMovable>();
        if (iMovable != null) {
            cell.board.movables.Add(iMovable);
        }

        return f;
    }

    private async Task SpawnGhost() {
        var center = Player.instance.figure.Location;
        GenerateFigure(Rand.rndExcept(center.Vicinity(11).Where(c => c.Free).ToList(), center.Vicinity(7)), ghostSample);
    }

    private static bool CanAttack(Unit attacker) =>
        attacker == null ||
        attacker.figure.Location.GetFigure<PeaceTrap>() == null &&
        !attacker.GetComponent<Disarm>().Active;

    private static bool CanBeAttacked(Unit defender, Weapon weapon) => 
        defender != null && 
        defender.Vulnerable &&
        !defender.figure.Location.Wall;

    private static bool IsRanged(Cell attackLocation, Cell defenceLocation) =>
        attackLocation != null &&
        defenceLocation != null &&
        (defenceLocation.position - attackLocation.position).MaxDelta() >= 2;

    private static bool Highground(
        Cell attackLocation,
        Cell defenceLocation,
        Unit attacker,
        Unit defender
    ) =>
        attackLocation != null &&
        defenceLocation != null &&
        defenceLocation.GetFigure<Hill>() != null && defender.BenefitsFromTerrain &&
        !(attackLocation.GetFigure<Hill>() != null && attacker.BenefitsFromTerrain);

    public static bool CanAttack(
        Unit attacker, 
        Unit defender, 
        Weapon weapon = null, 
        Cell attackLocation = null,
        Cell defenceLocation = null
    ) {
        if (attacker) attackLocation ??= attacker.figure.Location;
        if (defender) defenceLocation ??= defender.figure.Location;

        return
            CanAttack(attacker) &&
            CanBeAttacked(defender, weapon) &&
            (IsRanged(attackLocation, defenceLocation) || !Highground(attackLocation, defenceLocation, attacker, defender));
    }
}
