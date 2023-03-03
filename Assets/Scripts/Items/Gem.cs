using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Gem : MonoBehaviour
{
    public int amount;

    public void Awake() {
        GetComponent<Item>().onPick.Add(OnPick);
    }

    private async Task OnPick() {
        Player.instance.gems += amount;
        Destroy(gameObject);
    }
}
