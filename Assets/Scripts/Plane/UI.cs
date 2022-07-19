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

    public void Update() {
        gemsCounter.text = Game.instance.player.gems.ToString();
        areaCounter.text = Game.instance.clearedCells.Count.ToString();
        timeCounter.text = Game.instance.time.ToString();
    }
}
