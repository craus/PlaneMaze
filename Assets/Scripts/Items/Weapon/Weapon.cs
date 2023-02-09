using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Item))]
public abstract class Weapon : MonoBehaviour
{
    public int damage = 1;

    public Unit Owner => Player.instance;

    public GameObject attackProjectile;


    public abstract Task<bool> TryAttack(Vector2Int delta);
}
