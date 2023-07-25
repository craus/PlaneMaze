using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Metagame : MonoBehaviour
{
    public List<Ascention> ascentions;
    public bool pickingPhase;

    public Game game;
    
    public static Metagame Load(MetagameModel model) {
        var result = Instantiate(Library.instance.metagameSample);

        result.pickingPhase = model.pickingPhase;

        foreach (var a in model.ascentions) {
            var ascention = Ascention.Load(a);
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
        return 0;
    }

    public async Task AddRandomAscention() {
        var newAscention = Library.instance.ascentions.Where(a => a.CanAdd(this)).Rnd();
        await ConfirmationPanel.instance.AskConfirmation($"New ascention added: {newAscention}", canCancel: false);
        ascentions.Add(newAscention);
        GameManager.instance.SaveMetagame();
    }
}
