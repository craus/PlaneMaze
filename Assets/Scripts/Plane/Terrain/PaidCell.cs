using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class PaidCell : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;

    public int price;

    public void UpdateText() {
        text.text = price.ToString();
    }

    public void Awake() {
        GetComponent<Figure>().collide = Collide;
    }

    private async Task Collide(Cell from, Figure victim) {
        var player = victim.GetComponent<Player>();
        if (player == null) {
            return;
        }
        player.gems -= price;
        SoundManager.instance.buy.Play();
        //Destroy(gameObject);
        gameObject.SetActive(false);
        GetComponent<Figure>().OnDestroy();
    }

    internal void SetPrice(int newPrice) {
        price = newPrice;
        UpdateText();
    }
}
