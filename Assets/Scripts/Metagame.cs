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
    [SerializeField] private int losesRequiredForPenalty = 4;

    public List<Ascention> ascentions;
    public bool pickingPhase;
    public int losesWithNoPenalty = 0;
    public bool runInProgress = false;

    public Game game;

    internal bool Ascention<T>() where T : Ascention => ascentions.Any(a => a is T);
    internal int Ascentions<T>() where T : Ascention => ascentions.Count(a => a is T);

    public bool SpawnGhosts => true; // Ascention<GhostSpawns>();
    public float GhostSpawnSpeedMultiplier => 1; // Mathf.Pow(2, Ascentions<DoubleGhostSpawns>());
    public int MaxGhostSpawnsPerTurn => 4;
    public float GhostSpawnTimeReductionHalfLife => 16000;
    public float StartGhostSpawnProbability => 0.01f;
    public float GhostSpawnAcceleration(int time) =>
        Ascention<AcceleratingGhostSpawns>() ? 1 - Mathf.Pow(0.5f, time * 1f / GhostSpawnTimeReductionHalfLife) : 0;
    public float GhostSpawnProbabilityPerTurn(int time) => 
        (StartGhostSpawnProbability + (1 - StartGhostSpawnProbability) * GhostSpawnAcceleration(time)) * GhostSpawnSpeedMultiplier;

    public float HealingPotionSpawnProbability => Ascention<NoFreeHealingPotions>() ? 0 : 0.004f;

    public float PricesMultiplier => Ascention<QuadrupleMapAndPrices>() ? 1 : 0.25f;
    public int WorldSize => Ascention<QuadrupleMapAndPrices>() ? 1000 : 250;

    public static Metagame Load(MetagameModel model) {
        var result = Instantiate(Library.instance.metagameSample);

        result.pickingPhase = model.pickingPhase;
        result.losesWithNoPenalty = model.losesWithNoPenalty;
        result.runInProgress = model.runInProgress;

        foreach (var a in model.ascentions) {
            var ascention = global::Ascention.Load(a);
            result.ascentions.Add(ascention);
        }

        return result;
    }

    public async Task Win() {
        runInProgress = false;
        await AddRandomAscention();
    }

    public async Task Abandon() {
        await ConfirmationPanel.instance.AskConfirmation(
            $"Previous run was abandoned and resulted into loss.",
            canCancel: false
        );
        await Lose();
    }

    public async Task Lose() {
        runInProgress = false;

        if (ascentions.Count() == 0) return;

        losesWithNoPenalty++;
        if (losesWithNoPenalty >= losesRequiredForPenalty) {
            losesWithNoPenalty = 0;
            await RemoveRandomAscention();
        } else {
            await ConfirmationPanel.instance.AskConfirmation(
                $"{losesRequiredForPenalty - losesWithNoPenalty} more losses until descension.",
                canCancel: false
            );
        }
    }

    public MetagameModel Save() {
        var result = new MetagameModel {
            pickingPhase = pickingPhase,
            losesWithNoPenalty = losesWithNoPenalty,
            runInProgress = runInProgress,
            ascentions = ascentions.Select(a => a.Save()).ToList()
        };
        return result;
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
        var newAscention = Library.instance.ascentions.Where(a => a.CanAdd(this)).Rnd();
        await ConfirmationPanel.instance.AskConfirmation($"New ascention added: {newAscention.name}", canCancel: false);
        ascentions.Add(newAscention);
        MainUI.instance.UpdateAscentionsList();
        GameManager.instance.SaveMetagame();
    }

    public async Task RemoveRandomAscention() {
        var removingAscention = ascentions.Rnd();
        await ConfirmationPanel.instance.AskConfirmation($"Ascention removed: {removingAscention.name}", canCancel: false);
        ascentions.Remove(removingAscention);
        MainUI.instance.UpdateAscentionsList();
        GameManager.instance.SaveMetagame();
    }

    internal string AscentionsList() => string.Join('\n', ascentions.Unique().Select(a => $"{AscentionLevelString(a)}{a.name}"));
}
