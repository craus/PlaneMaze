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
    public TextMeshProUGUI animationSpeed;
    public TextMeshProUGUI frameTime;

    public List<GameObject> cheatUI;

    public void Start() {
        Update();
    }

    public void Update() {
        if (Game.instance.player) {
            gemsCounter.text = Game.instance.player.gems.ToString();
        }
        areaCounter.text = Game.instance.ghostSpawnProbabilityPerTurn.ToString("0.000");
        timeCounter.text = Game.instance.time.ToString();
        animationSpeed.text = Helpers.animationSpeed.ToString("0.000");
        frameTime.text = Time.deltaTime.ToString("0.000");
        if (Player.instance != null) {
            commandQueueCounter.text = Player.instance.commands.Count.ToString();
        }
        foreach (var go in cheatUI) {
            go.SetActive(Cheats.on);
        }

        if (Input.GetKeyDown(KeyCode.Equals)) {
            Helpers.animationSpeed *= 1.2f;
        }
        if (Input.GetKeyDown(KeyCode.Minus)) {
            Helpers.animationSpeed /= 1.2f;
        }
    }
}
