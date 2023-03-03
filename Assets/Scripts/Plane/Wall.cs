using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Wall : Building
{
    public Wall wallSample;

    public TMPro.TextMeshProUGUI healthText;

    public void Update() {
        healthText.text = $"{health}";
    }

    public void Hit() {
        health--;
        if (health <= 0) {
            Die();
        }
    }

    public void Die() {
        Destroy(gameObject);
    }
}
