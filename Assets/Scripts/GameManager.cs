using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    public Game game;
    public Game gameSample;

    public void Start() {
        NewGame();
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Restart();
        }
    }

    public void Restart() {
        DestroyGame();
        NewGame();
    }

    public void NewGame() {
        game = Instantiate(gameSample, transform);
    }

    public void DestroyGame() {
        Destroy(game.gameObject);
    }
}
