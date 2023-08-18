﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Library : Singletone<Library>
{
    public Metagame metagameSample;

    public Gem gem;

    public List<Biome> biomes;

    public List<Figure> figures;

    public Biome dungeon;
    public Biome crypt;
    public Biome darkrootForest;

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
    public MonstersHeal monstersHeal;
    public MoreMonsters moreMonsters;
    public MaskedTerrain maskedTerrain;
    public FasterMonsters fasterMonsters;
    public NoStartingWeapon noStartingWeapon;

    public List<Ascention> ascentions;
    public List<Ascention> additionalAscentions;

    public IEnumerable<Ascention> AllAscentions => ascentions.Concat(additionalAscentions);

    public GameObject teleportExit;

    public CursedSign cursedSign;
    public Tree tree;

    public static T Get<T>() => instance.figures.Select(f => f.GetComponent<T>()).FirstOrDefault(c => c != null);
}
