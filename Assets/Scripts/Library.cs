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
    public EndlessSkeletonsResurrection endlessSkeletonsResurrection;
    public AcceleratingGhostSpawns acceleratingGhostSpawns;
    public CommonEnemiesHaveMultipleHP commonEnemiesHaveMultipleHP;
    public NoFreeHealingPotions noFreeHealingPotions;
    public HealOnlyOneHP healOnlyOneHP;
    public QuadrupleMapAndPrices quadrupleMapAndPrices;
    public MonstersBenefitFromTerrain monstersBenefitFromTerrain;
    public FasterBoss fasterBoss;
    public PlayerDontBenefitFromTerrain playerDontBenefitFromTerrain;

    public List<Ascention> ascentions;
    public List<Ascention> additionalAscentions;

    public IEnumerable<Ascention> AllAscentions => ascentions.Concat(additionalAscentions);

    public GameObject teleportExit;
}
