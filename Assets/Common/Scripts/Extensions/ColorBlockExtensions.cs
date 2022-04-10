using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ColorBlockExtensions
{
    public static ColorBlock Multiplied(this ColorBlock cb, Color c)
    {
        var result = cb;
        result.disabledColor = cb.disabledColor * c;
        result.normalColor = cb.normalColor * c;
        result.highlightedColor = cb.highlightedColor * c;
        result.selectedColor = cb.selectedColor * c;
        result.pressedColor = cb.pressedColor * c;
        return result;
    }
}