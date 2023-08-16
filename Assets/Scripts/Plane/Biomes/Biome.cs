using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Biome : MonoBehaviour
{
    [SerializeField] private int size;
    public int Size => (int)(Metagame.instance.WorldSizeMultiplier * size);

    public float ghostPower = 1;

    public GameObject floorModel;
    public GameObject wallModel;

    public List<Weighted<Monster>> monsterSamples;
    public List<Weighted<Terrain>> terrainSamples;
}
