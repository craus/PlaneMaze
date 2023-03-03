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
        var backCell = GetComponent<Item>().Owner.figure.location.Shift(-GetComponent<Item>().Owner.lastMove);
        var attackDirection = attack.to.position - attack.from.position;
        if (
            GetComponent<Item>().Owner.lastMove.Codirected(-attackDirection) &&
            attackDirection != Vector2Int.zero &&
            backCell.Free
        ) {
            attack.damage -= 1;
            SoundManager.instance.defence.Play();
            await GetComponent<Item>().Owner.figure.Move(backCell);
        }
    }
}
