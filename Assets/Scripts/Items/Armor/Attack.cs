using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Attack 
{
    public Vector2Int delta;
    public Figure from;
    public Figure to;
    public int damage;

    public List<Func<Task>> afterAttack = new List<Func<Task>>();

    public Attack(Figure from, Figure to, int damage) {
        this.from = from;
        this.to = to;
        this.damage = damage;
    }

    public override string ToString() => $"Attack from {from} to {to} at {delta} with damage {damage}";
}
