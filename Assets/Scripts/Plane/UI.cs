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
    public TextMeshProUGUI borderCounter;
    public TextMeshProUGUI timeCounter;

    public void Update() {
        if (GameManager.instance.player) {
            gemsCounter.text = GameManager.instance.player.gems.ToString();
        }
        if (GameManager.instance.game) {
            areaCounter.text = GameManager.instance.game.area.ToString();
            borderCounter.text = GameManager.instance.game.border.ToString();
            timeCounter.text = $"{GameManager.instance.game.time.Digits(2)}";
        }
    }
}
