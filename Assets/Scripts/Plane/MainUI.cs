﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainUI : Singletone<MainUI>
{
    public GameObject pauseScreen;

    public Slider sounds;
    public Slider music;
    public Slider master;

    public TMP_Text ascentionsListText;
    public GameObject ascentionsPanel;

    public AudioMixer mixer; 

    public Settings settings;

    public string settingsFilename = "settings.dat";

    public bool MainMenuShown => pauseScreen.activeSelf;

    public Button hardcore;
    public Button softcore;

    public GameObject hardcoreLabel;
    public TMP_Text ascensionsText;

    public void Start() {
        pauseScreen.SetActive(false);
        UpdateAscentionsList();

        settings = FileManager.LoadFromFile<Settings>(settingsFilename);
        if (settings == null) {
            settings = new Settings();
            FileManager.SaveToFile(settings, settingsFilename);
        }
        UpdateVolume();

        sounds.onValueChanged.AddListener(_ => UpdateSettings());
        music.onValueChanged.AddListener(_ => UpdateSettings());
        master.onValueChanged.AddListener(_ => UpdateSettings());
    }

    public void UpdateVolume() {
        mixer.SetFloat("sounds", settings.sounds);
        mixer.SetFloat("music", settings.music);
        mixer.SetFloat("master", settings.master);
    }

    bool showingValues = false;
    public void UpdateSettings() {
        if (showingValues) {
            return;
        }
        settings.sounds = sounds.value;
        settings.music = music.value;
        settings.master = master.value;
        UpdateVolume();
    }

    public void UpdateAscentionsList() {
        ascentionsListText.text = GameManager.instance.metagame.AscentionsList();
        ascentionsPanel.SetActive(GameManager.instance.metagame.Ascentions.Count() > 0);
        ascensionsText.text = GameManager.instance.metagame.ShortAscensionsList();
        hardcoreLabel.SetActive(Metagame.instance.hardcore);
        softcore.interactable = Metagame.instance.hardcore;
        hardcore.interactable = !Metagame.instance.hardcore;
    }

    public void MainMenuButton() {
        pauseScreen.SetActive(!MainMenuShown);

        UpdateAscentionsList();

        showingValues = true;
        sounds.value = settings.sounds;
        music.value = settings.music;
        master.value = settings.master;
        showingValues = false;

        FileManager.SaveToFile(settings, settingsFilename);
    }

    public async void Hardcore() {
        if (await ConfirmationManager.instance.AskConfirmation("Are you sure? You will ALL ascensions now and ONE ascension for each loss!")) {
            Metagame.instance.SwitchToHardcore();
            UpdateAscentionsList();
        }
    }

    public async void Softcore() {
        if (await ConfirmationManager.instance.AskConfirmation("Are you sure? If you switch back after that, you will ALL ascensions now and ONE ascension for each loss!")) {
            Metagame.instance.SwitchToSoftcore();
            softcore.enabled = false;
            hardcore.enabled = true;
            UpdateAscentionsList();
        }
    }

    public void QuitApplication() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public async void QuitButton() {
        if (await ConfirmationManager.instance.AskConfirmation("Are you sure you want to quit now?")) {
            QuitApplication();
        }
    }

    public async void RestartButton() {
        if (await ConfirmationManager.instance.AskConfirmation("Are you sure you want to restart now?")) {
            GameManager.instance.RestartGame();
        }
    }

    public async void RestartMetagameButton() {
        if (await ConfirmationManager.instance.AskConfirmation("Are you sure you want to reset ALL PROGRESS now?")) {
            GameManager.instance.RestartMetagame();
        }
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            if (ConfirmationManager.instance.AwaitingConfirmation) {
                ConfirmationManager.instance.OK();
            } else {
                InfoPanel.instance.panel.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (ConfirmationManager.instance.AwaitingConfirmation) {
                ConfirmationManager.instance.Cancel();
            } else if (InfoPanel.instance.panel.activeSelf) {
                InfoPanel.instance.panel.SetActive(false);
            } else {
                MainMenuButton();
            }
        }

        if (Cheats.on) {
            if (Input.GetKeyDown(KeyCode.Z)) {
                UndoManager.instance.Undo();
            }
            if (Input.GetKeyDown(KeyCode.X)) {
                Rand.Range(0, 1f);
            }
        } 

        if (Input.anyKeyDown) {
            if (ConfirmationManager.instance.AwaitingConfirmation && ConfirmationManager.instance.canConfirmByAnyButton) {
                ConfirmationManager.instance.AnyButton();
            } else {
                if (Player.instance != null && !ConfirmationManager.instance.AwaitingConfirmation) {
                    Player.instance.ReadKeys();
                }
            }
        }
    }
}
