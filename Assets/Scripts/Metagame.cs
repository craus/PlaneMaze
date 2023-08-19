using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Metagame : MonoBehaviour
{
    public static Metagame instance => GameManager.instance.metagame;

    private List<Ascention> ascentions = new List<Ascention>();
    public IEnumerable<Ascention> Ascentions => ascentions;

    public bool pickingPhase;
    public int losesWithNoPenalty = 0;
    public bool runInProgress = false;
    public bool hardcore = false;

    public IEnumerable<Ascention> ModeAscensions => hardcore ? Library.instance.AllAscentions : Library.instance.ascentions;

    public Game game;

    internal bool HasAscention<T>() where T : Ascention => ascentions.Any(a => a is T);
    internal int AscentionsCount<T>() where T : Ascention => ascentions.Count(a => a is T);


    public int LosesRequiredForPenalty => hardcore ? 1 : 4;
    public bool DropLosesOnWin => true;

    public bool SpawnGhosts => true; // Ascention<GhostSpawns>();
    public float GhostSpawnSpeedMultiplier => 1; // Mathf.Pow(2, Ascentions<DoubleGhostSpawns>());
    public int MaxGhostSpawnsPerTurn => 4;
    public float GhostSpawnTimeReductionHalfLife => 16000;
    public float StartGhostSpawnProbability => 0.01f;
    public float GhostSpawnAcceleration(int time) =>
        HasAscention<AcceleratingGhostSpawns>() ? 1 - Mathf.Pow(0.5f, time * 1f / GhostSpawnTimeReductionHalfLife) : 0;
    public float GhostSpawnProbabilityPerTurn(int time) => 
        (StartGhostSpawnProbability + (1 - StartGhostSpawnProbability) * GhostSpawnAcceleration(time)) * GhostSpawnSpeedMultiplier;

    public float HealingPotionSpawnProbability => HasAscention<NoFreeHealingPotions>() ? 0 : 0.004f;

    public float PricesMultiplier => 
        Mathf.Pow(4, AscentionsCount<QuadrupleMapAndPrices>()) *
        Mathf.Pow(2, AscentionsCount<MoreMonsters>()) *
        0.25f;

    public float WorldSizeMultiplier => HasAscention<QuadrupleMapAndPrices>() ? 1 : 0.25f;

    public float MonsterProbability => HasAscention<MoreMonsters>() ? 0.2f : 0.1f;
    
    public MetagameModel ConvertToModel() {
        var result = new MetagameModel {
            pickingPhase = pickingPhase,
            losesWithNoPenalty = losesWithNoPenalty,
            runInProgress = runInProgress,
            hardcore = hardcore,
            ascentions = ascentions.Select(a => a.Save()).ToList()
        };
        return result;
    }

    public static Metagame ConvertFromModel(MetagameModel model) {
        var result = Instantiate(Library.instance.metagameSample);

        result.pickingPhase = model.pickingPhase;
        result.losesWithNoPenalty = model.losesWithNoPenalty;
        result.runInProgress = model.runInProgress;
        result.hardcore = model.hardcore;

        foreach (var a in model.ascentions) {
            var ascention = global::Ascention.Load(a);
            result.ascentions.Add(ascention);
        }

        return result;
    }

    public void SwitchToHardcore() {
        hardcore = true;
        ascentions.Clear();
        GameManager.instance.SaveMetagame();
    }

    public void SwitchToSoftcore() {
        hardcore = false;
        GameManager.instance.SaveMetagame();
    }

    public async Task Win() {
        runInProgress = false;
        losesWithNoPenalty = 0;
        if (ModeAscensions.Any(a => a.CanAdd(this))) {
            await AddRandomAscention();
        } else {
            await ConfirmationManager.instance.AskConfirmation(
                $"You beat the game at max ascension! Congratulations!",
                canCancel: false
            );
        }
    }

    public async Task Lose() {
        runInProgress = false;

        if (ascentions.Count() == 0) return;

        losesWithNoPenalty++;
        if (losesWithNoPenalty >= LosesRequiredForPenalty) {
            losesWithNoPenalty = 0;
            await RemoveRandomAscention();
        } else {
            await ConfirmationManager.instance.AskConfirmation(
                $"{LosesRequiredForPenalty - losesWithNoPenalty} more losses until descension.",
                canCancel: false
            );
        }
    }

    public async Task Abandon() {
        await ConfirmationManager.instance.AskConfirmation(
            $"Previous run was abandoned and resulted into loss.",
            canCancel: false
        );
        await Lose();
    }

    public int AscentionLevel(Ascention ascention) {
        return ascentions.Count(a => a == ascention);
    }

    public string AscentionLevelString(Ascention ascention) {
        var l = AscentionLevel(ascention);
        if (l == 1) return "";
        return $"x{l} ";
    }

    public async Task AddRandomAscention() {
        var newAscention = ModeAscensions.Where(a => a.CanAdd(this)).Rnd();
        await ConfirmationManager.instance.AskConfirmation($"New ascention added: {newAscention.name}", canCancel: false);
        ascentions.Add(newAscention);
        MainUI.instance.UpdateAscentionsList();
        GameManager.instance.SaveMetagame();
    }

    public async Task RemoveRandomAscention() {
        var removingAscention = ascentions.Where(a => a.CanRemove(this)).Rnd();
        await ConfirmationManager.instance.AskConfirmation($"Ascention removed: {removingAscention.name}", canCancel: false);
        ascentions.Remove(removingAscention);
        MainUI.instance.UpdateAscentionsList();
        GameManager.instance.SaveMetagame();
    }

    internal string AscentionsList() => string.Join('\n', ascentions.Unique().Select(a => $"{AscentionLevelString(a)}{a.name}"));
    internal string ShortAscensionsList() => string.Join("", ascentions.Unique().Select(a => $"{AscentionLevelString(a)}{a.abbreviation}"));
}
