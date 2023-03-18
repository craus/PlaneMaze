using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainUI : Singletone<MainUI>
{
    public GameObject mainMenu;

    public Slider sounds;
    public Slider music;
    public Slider master;

    public AudioMixer mixer; 

    public Settings settings;

    public string settingsFilename = "settings.dat";

    public bool MainMenuShown => mainMenu.activeSelf;

    public void Start() {
        mainMenu.SetActive(false);

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

    public void MainMenuButton() {
        mainMenu.SetActive(!MainMenuShown);

        showingValues = true;
        sounds.value = settings.sounds;
        music.value = settings.music;
        master.value = settings.master;
        showingValues = false;

        FileManager.SaveToFile(settings, settingsFilename);
    }

    public void QuitButton() {
        ConfirmationPanel.instance.DoWithConfirmation(() => Application.Quit(), "Are you sure you want to quit now?");
    }

    public void RestartButton() {
        ConfirmationPanel.instance.DoWithConfirmation(() => GameManager.instance.Restart(), "Are you sure you want to restart now?");
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            MainMenuButton();
        }
    }
}
