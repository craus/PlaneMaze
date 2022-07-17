using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Building : MonoBehaviour
{
    public int cost = 5;
    public int sellCost = 3;
    public int damagedSellCost = 3;
    public int health = 5;
    public int maxHealth = 5;
    public bool undestructible = false;

    public virtual int SellCost => health == maxHealth ? sellCost : damagedSellCost;
}
