using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IExplainable
{
    public string Text { get; }
    public Sprite Icon { get; }
    public Color IconColor { get; }
    public Material IconMaterial { get; }

    public IExplainable Sample { get; set; }
}
