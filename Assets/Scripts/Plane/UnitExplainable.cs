using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class UnitExplainable : MonoBehaviour, IExplainable
{
    public SpriteRenderer iconImage;

    [Multiline(8)]
    public string description;

    public string Text => description;
    public Sprite Icon => iconImage.sprite;
    public Color IconColor => iconImage.color;
    public Material IconMaterial => iconImage.material;
    public IExplainable Sample { get; set; }
}
