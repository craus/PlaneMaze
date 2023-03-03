using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Figure))]
public class ItemGenerationRules : MonoBehaviour
{
    public float startingWeight = 1;
    public float fieldWeight = 1;
    public float storeWeight = 1;
    public int minPrice = 10;
    public int maxPrice = 20;
}
