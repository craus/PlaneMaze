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

    private async void GainInvulnerabilityAfterAnimations(int turnNumber) {
        await Game.instance.completedTurns[turnNumber].Task;

        if (currentPhase == invulnerabilityPeriod) {
            currentPhase = 0;
            highlightedIcon.fillAmount = 0;
            await GetComponent<Item>().Owner.GetComponent<Invulnerability>().Gain(invulnerabilityDuration + 1);
        }
    }

    private async Task AfterPlayerMove(int turnNumber) {
        if (!GetComponent<Item>().Equipped) {
            return;
        }

        // do not await this
        GainInvulnerabilityAfterAnimations(turnNumber);

        currentPhase++;
        await highlightedIcon.RadialFill(1f * currentPhase / invulnerabilityPeriod, 0.1f);
    }
}
