﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class Unit : MonoBehaviour
{
    public Figure figure;

    public void Awake() {
        if (figure == null) figure = GetComponent<Figure>();
        figure.afterMove.AddListener(AfterMove);
    }

    public virtual void AfterMove(bool isTeleport) {
        if (!isTeleport) {
            if (figure.location.fieldCell.teleport) {
                figure.Move(figure.location.board.GetCell(figure.location.fieldCell.teleportTarget), isTeleport: true);
            }
        }
    }
}