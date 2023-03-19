using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance => Game.instance != null ? Game.instance.gameEvents : null;

    public List<Func<Unit, Task>> onUnitDeath = new List<Func<Unit, Task>>();
}
