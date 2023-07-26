using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Library : Singletone<Library>
{
    public Metagame metagameSample;

    public DoubleGhostSpawns doubleGhostSpawns;
    public GhostSpawns ghostSpawns;
    public MonstersHaveMoreHealth monstersHaveMoreHealth;
    public PlayerHasLessHealth playerHasLessHealth;
    public SkeletonsResurrect skeletonsResurrect;
    public AcceleratingGhostSpawns acceleratingGhostSpawns;
    public CommonEnemiesHaveMultipleHP commonEnemiesHaveMultipleHP;
    public NoFreeHealingPotions noFreeHealingPotions;
    public HealOnlyOneHP healOnlyOneHP;

    public List<Ascention> ascentions;

    public GameObject teleportExit;
}
