using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    public Metagame metagameSample;
    public Metagame metagame;

    public Game game;
    public Game gameSample;

    public const string savefileName = "savefile.dat";

    public void Start() {
        var metagameModel = FileManager.LoadFromFile<MetagameModel>(savefileName);
        if (metagameModel == null) {
            NewMetagame();
            FileManager.SaveToFile(metagame.Save(), savefileName);
        } else {
            metagame = Metagame.Load(metagameModel);
        }
        NewGame();
        //mazeSample.Reinitialize(mazeSample.width / 20, mazeSample.height / 20);
    }

    Vector3 oldMousePosition;
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

        if (oldMousePosition != Input.mousePosition) {
            Cursor.visible = true;
        }
        oldMousePosition = Input.mousePosition;
    }

    public void Restart() {
        DestroyGame();
        NewGame();
    }

    public void NewMetagame() {
        metagame = Instantiate(metagameSample, transform);
    }

    public void NewGame() {
        game = Instantiate(gameSample, transform);
        game.ascentions = metagame.ascentions;
        Inventory.instance = game.GetComponentInChildren<Inventory>();
        game.speed = 100;
    }

    public void DestroyGame() {
        Destroy(game.gameObject);
    }
}
