using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Weapon : MonoBehaviour
{
    public int damage;

    public Unit Owner => Player.instance;

    public GameObject attackProjectile;
}
