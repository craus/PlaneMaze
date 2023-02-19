using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Attack 
{
    public Cell from;
    public Cell to;
    public int damage;

    public Attack(Cell from, Cell to, int damage) {
        this.from = from;
        this.to = to;
        this.damage = damage;
    }
}
