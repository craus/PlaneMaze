﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Invisibility : MonoBehaviour
{
    [SerializeField] private GameObject model;

    public void Switch(bool on) {
        model.SetActive(!on);
    }

    public void Check() {
        Switch(CalculateInvisibility());
    }

    private bool HiddenInsideFog() {
        var fog = GetComponent<Figure>().Location.GetFigure<Fog>();
        return 
            GetComponent<Unit>() != null &&
            fog != null &&
            fog.On &&
            (Player.instance.figure.Location.position - GetComponent<Figure>().Location.position).SumDelta() > 1;
    }

    private bool FarFromPlayer() => 
        (Player.instance.figure.Location.position - GetComponent<Figure>().Location.position).MaxDelta() >= 2;

    private bool HiddenOutsideFog() => PlayerInsideFog() && FarFromPlayer();

    private bool PlayerInsideFog() {
        var playerFog = Player.instance.figure.Location.GetFigure<Fog>();
        return playerFog && playerFog.On;
    }

    private bool CalculateInvisibility() {
        if (HiddenInsideFog()) return true;
        if (HiddenOutsideFog()) return true;
        return false;
    }
}