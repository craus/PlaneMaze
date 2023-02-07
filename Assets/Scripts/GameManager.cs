using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    public Game game;
    public Game gameSample;
    public Texture2D mazeSample;

    public void Start() {
        NewGame();
        //mazeSample.Reinitialize(mazeSample.width / 20, mazeSample.height / 20);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Restart();
        }
        if (Input.GetKeyDown(KeyCode.PageUp)) {
            Game.instance.speed *= 1.25f;
        }
        if (Input.GetKeyDown(KeyCode.PageDown)) {
            Game.instance.speed /= 1.25f;
        }
    }

    public void Restart() {
        DestroyGame();
        NewGame();
    }

    public void NewGame() {
        game = Instantiate(gameSample, transform);
        game.speed = 1;
    }

    public void DestroyGame() {
        Destroy(game.gameObject);
    }
}
