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

    public List<GameObject> amountModels;

    public void Awake() {
        GetComponent<Item>().afterPick.Add(OnPick);
        UpdateSprite();
    }

    public void UpdateSprite() {
        for (int i = 0; i < amountModels.Count(); i++) {
            amountModels[i].SetActive((i + 1) == amount);
        }
        amountModels.Last().SetActive(amount >= amountModels.Count);
    }

    private async Task OnPick() {
        Player.instance.gems += amount;

        gameObject.SetActive(false);
        //Destroy(gameObject);
        GetComponent<Figure>().OnDestroy();
    }
}
