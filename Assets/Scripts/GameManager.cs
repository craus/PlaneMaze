﻿using System;
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

    public void Awake() {
        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (_, e) => Debug.LogException(e.Exception);
    }

    public async void Start() {
        var metagameModel = FileManager.LoadFromFile<MetagameModel>(savefileName);
        if (metagameModel == null) {
            NewMetagame();
            SaveMetagame();
        } else {
            metagame = Metagame.ConvertFromModel(metagameModel);
            if (metagame.runInProgress) {
                await metagame.Abandon();
            }
            metagame.transform.SetParent(transform);
            MainUI.instance.UpdateAscentionsList();
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
            WorldGenerator.instance.speed *= 1.25f;
        }
        if (Input.GetKeyDown(KeyCode.PageDown)) {
            WorldGenerator.instance.speed /= 1.25f;
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
        MainUI.instance.UpdateAscentionsList();
    }

    public void SaveMetagame() {
        FileManager.SaveToFile(metagame.ConvertToModel(), savefileName);
    }

    public void NewMetagame() {
        metagame = Instantiate(metagameSample, transform);
        metagame.transform.SetParent(transform);
    }

    public void NewGame() {
        UndoManager.instance.ResetTrackers();
        game = Instantiate(gameSample, transform);
        metagame.runInProgress = true;
        SaveMetagame();
        Inventory.instance = game.GetComponentInChildren<Inventory>();
        WorldGenerator.instance = game.GetComponentInChildren<WorldGenerator>();
        WorldGenerator.instance.speed = 100;
    }

    public void DestroyMetagame()
    {
        Destroy(metagame.gameObject);
    }

    public void DestroyGame() {
        Destroy(game.gameObject);
    }
}
