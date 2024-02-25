using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Item))]
public class GlovesOfSwapping : MonoBehaviour, IAttackModifier
{
    public int Priority => 0;

    public async Task ModifyAttack(Attack attack) {
        var fromLocation = attack.from.Location;
        var toLocation = attack.to.Location;
        attack.afterAttack.Add(
            async () => {
                if (attack.from != null) fromLocation = attack.from.Location;
                if (attack.to != null) toLocation = attack.to.Location;

                var anotherUnit = toLocation.GetFigures<Unit>().FirstOrDefault(u => u.OccupiesPlace && u.figure != attack.to);
                if (anotherUnit != null) {
                    Debug.LogFormat($"Swap cancelled: another unit on toLocation: {anotherUnit}");
                    return;
                }

                anotherUnit = fromLocation.GetFigures<Unit>().FirstOrDefault(u => u.OccupiesPlace && u.figure != attack.from);
                if (anotherUnit != null) {
                    Debug.LogFormat($"Swap cancelled: another unit on fromLocation: {anotherUnit}");
                    return;
                }

                if (fromLocation.board != Game.instance.mainWorld) {
                    Debug.LogFormat($"Swap cancelled: player location is not in mainWorld");
                    return;
                }

                if (attack.from.GetComponent<Root>().Current > 0 || attack.to.GetComponent<Root>().Current > 0) {
                    return;
                }

                List<Task> moves = new List<Task>();
                if (attack.to != null && attack.to.gameObject.activeSelf) moves.Add(attack.to.Move(fromLocation));
                if (attack.from != null && attack.from.gameObject.activeSelf) {
                    moves.Add(attack.from.GetComponent<Invulnerability>().Gain(1));
                    moves.Add(attack.from.Move(toLocation));
                }

                await Task.WhenAll(moves);
            }
        );
    }
}
