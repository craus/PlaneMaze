﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class RingOfHealth : MonoBehaviour
{
    public int healthBonus = 2;
    public Unit lastOwner;

    public void OnEnable() {
        GetComponent<Item>().afterPick.Add(OnPick);
        GetComponent<Item>().afterDrop.Add(OnDrop);
    }

    public void OnDisable() {
        GetComponent<Item>().afterPick.Remove(OnPick);
        GetComponent<Item>().afterDrop.Remove(OnDrop);
    }

    private async Task OnPick() {
        var item = GetComponent<Item>();
        var owner = item.Owner;
        lastOwner = owner;
        var health = owner.GetComponent<Health>();
        health.current += healthBonus;
        health.max += healthBonus;
        health.UpdateHearts();
    }

    private async Task OnDrop() {
        var health = lastOwner.GetComponent<Health>();
        await health.Hit(healthBonus);
        if (!lastOwner.alive) {
            return;
        }
        health.max -= healthBonus;
        health.UpdateHearts();
    }
}
