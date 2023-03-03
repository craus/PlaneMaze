using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : Singletone<GameEvents>
{
    public List<Func<Unit, Task>> onUnitDeath = new List<Func<Unit, Task>>();
}
