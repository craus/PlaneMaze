using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIImageExplainable : MonoBehaviour, IExplainable
{
    public Image image;

    [Multiline(8)]
    public string description;

    public string Text => description;
    public Sprite Icon => image.sprite;
    public Color IconColor => image.color;
    public Material IconMaterial => image.material;
    public IExplainable Sample { get; set; }
}
