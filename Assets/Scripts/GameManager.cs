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
            SaveMetagame();
        } else {
            metagame = Metagame.Load(metagameModel);
            metagame.transform.SetParent(transform);
        }
        NewGame();
        //mazeSample.Reinitialize(mazeSample.width / 20, mazeSample.height / 20);
    }

    Vector3 oldMousePosition;
    public void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            MainUI.instance.RestartButton();
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            MainUI.instance.RestartMetagameButton();
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

    public void RestartGame() {
        Debug.LogFormat("Game restarted");
        DestroyGame();
        NewGame();
    }

    public void RestartMetagame()
    {
        Debug.LogFormat("Metagame restarted");
        DestroyMetagame();
        NewMetagame();
        SaveMetagame();

        RestartGame();
    }

    public void SaveMetagame() {
        FileManager.SaveToFile(metagame.Save(), savefileName);
    }

    public void NewMetagame() {
        metagame = Instantiate(metagameSample, transform);
        metagame.transform.SetParent(transform);
    }

    public void NewGame() {
        game = Instantiate(gameSample, transform);
        game.ascentions = metagame.ascentions;
        Inventory.instance = game.GetComponentInChildren<Inventory>();
        game.speed = 100;
    }

    public void DestroyMetagame()
    {
        Destroy(metagame.gameObject);
    }

    public void DestroyGame() {
        Destroy(game.gameObject);
    }
}
