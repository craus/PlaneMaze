using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI gemsCounter;
    public TextMeshProUGUI areaCounter;
    public TextMeshProUGUI timeCounter;
    public TextMeshProUGUI commandQueueCounter;

    public List<GameObject> cheatUI;

    public void Update() {
        if (Game.instance.player) {
            gemsCounter.text = Game.instance.player.gems.ToString();
        }
        areaCounter.text = Game.instance.ghostSpawnProbabilityPerTurn.ToString("0.000");
        timeCounter.text = Game.instance.time.ToString();
        if (Player.instance != null) {
            commandQueueCounter.text = Player.instance.commands.Count.ToString();
        }
        foreach (var go in cheatUI) {
            go.SetActive(Cheats.on);
        }
    }
}
