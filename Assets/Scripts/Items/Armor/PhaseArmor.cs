using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class PhaseArmor : MonoBehaviour
{
    public int invulnerabilityDuration = 1;
    public int invulnerabilityPeriod = 4;
    public int currentPhase = 0;

    public Image highlightedIcon;

    public void Awake() {
        Game.instance.afterPlayerMove.Add(AfterPlayerMove);
    }

    private async Task AfterPlayerMove() {
        if (!GetComponent<Item>().Equipped) {
            return;
        }

        currentPhase++;
        await highlightedIcon.RadialFill(1f * currentPhase / invulnerabilityPeriod, 0.1f);

        if (currentPhase == invulnerabilityPeriod) {
            currentPhase = 0;
            highlightedIcon.fillAmount = 0;
            GetComponent<Item>().Owner.GetComponent<Invulnerability>().Gain(invulnerabilityDuration+1);
        }
    }
}
