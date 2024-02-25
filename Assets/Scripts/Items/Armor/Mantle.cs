using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class Mantle : MonoBehaviour, IReceiveAttackModifier
{
    public int range = 1;
    public int cooldown = 4;
    public int currentCooldown = 0;

    public Image highlightedIcon;
    public Image activeIcon;

    public int Priority => 0;

    public void Awake() {
        Game.instance.afterMonsterMove.Add(AfterMonsterMove);
        currentCooldown = 0;
        highlightedIcon.fillAmount = 1;
        activeIcon.gameObject.SetActive(true);
        new ValueTracker<int>(() => currentCooldown, v => {
            currentCooldown = v;
            highlightedIcon.fillAmount = 1f * (cooldown - currentCooldown) / cooldown;
        });
    }

    public async Task ModifyAttack(Attack attack) {
        if (attack.damage < 1) {
            Debug.LogFormat($"Mantle ignores attack");
            return;
        }
        if (currentCooldown > 0) {
            Debug.LogFormat($"Mantle on cooldown: {currentCooldown}");
            return;
        }
        currentCooldown = cooldown;
        Debug.LogFormat($"Mantle goes in cooldown {currentCooldown}");
        highlightedIcon.fillAmount = 0;
        activeIcon.gameObject.SetActive(false);

        var destinations = GetComponent<Item>().Owner.figure.Location.Vicinity(maxDx: range, maxDy: range).Where(c => c.Free);
        if (destinations.Count() > 0) {
            var destination = destinations.Rnd();
            Debug.LogFormat($"Mantle teleports player");
            if (await GetComponent<Item>().Owner.figure.Move(destination, isTeleport: true, teleportAnimation: true)) {
                Debug.LogFormat($"Mantle teleported player");
                attack.damage = 0;
            } else {
                Debug.LogFormat($"Teleportation failed");
            }
        } else {
            Debug.LogFormat($"Mantle failed to find teleport destination");
            // do nothing
        }
    }

    private async Task AfterMonsterMove(int turnNumber) {
        if (!GetComponent<Item>().Equipped) {
            return;
        }

        Debug.LogFormat($"Mantle tries to reduce cooldown ({currentCooldown})");

        if (currentCooldown > 0) {
            currentCooldown--;
            Debug.LogFormat($"Mantle reduces cooldown to ({currentCooldown})");
            await highlightedIcon.RadialFill(1f * (cooldown - currentCooldown) / cooldown, 0.1f);
            if (currentCooldown == 0) {
                activeIcon.gameObject.SetActive(true);
            }
        }
    }
}
