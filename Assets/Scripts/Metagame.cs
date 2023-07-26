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
    public List<Ascention> ascentions;
    public bool pickingPhase;

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

    public static Metagame Load(MetagameModel model) {
        var result = Instantiate(Library.instance.metagameSample);

        result.pickingPhase = model.pickingPhase;

        foreach (var a in model.ascentions) {
            var ascention = global::Ascention.Load(a);
            result.ascentions.Add(ascention);
        }

        return result;
    }

    public MetagameModel Save() {
        var result = new MetagameModel {
            pickingPhase = pickingPhase,
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
        GameManager.instance.SaveMetagame();
    }

    internal string AscentionsList() => string.Join('\n', ascentions.Unique().Select(a => $"{AscentionLevelString(a)}{a.name}"));
}
