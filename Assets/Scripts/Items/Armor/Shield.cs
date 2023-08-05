using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Shield : MonoBehaviour, IReceiveAttackModifier, ISideDefence
{
    public int Priority => 0;

    public bool GivesDefenceFrom(Vector2Int direction) => GetComponent<Item>().Owner.lastMove.Codirected(-direction);

    public void Awake() {
        //GetComponent<Item>().onPick.Add(OnPick);
        //GetComponent<Item>().onDrop.Add(OnDrop);
    }

    public async Task ModifyAttack(Attack attack) {
        var backCell = GetComponent<Item>().Owner.figure.Location.Shift(-GetComponent<Item>().Owner.lastMove);
        var attackDirection = attack.defenceLocation.position - attack.attackLocation.position;
        if (
            GetComponent<Item>().Owner.lastMove.Codirected(-attackDirection) &&
            attackDirection != Vector2Int.zero &&
            attackDirection.sqrMagnitude == 1
        ) {
            attack.damage -= 1;
            SoundManager.instance.defence.Play();
            await GetComponent<Item>().Owner.figure.TryWalk(attackDirection);
        } else {
            Debug.LogFormat("Shield failed to protect player");
        }
    }
}
