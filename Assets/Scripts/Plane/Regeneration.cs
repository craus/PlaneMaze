using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Regeneration : MonoBehaviour
{
    public int movesSinceHitToHeal = 3;
    public int healCooldown = 3;
}
