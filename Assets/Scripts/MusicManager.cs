using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : Singletone<MusicManager>
{
    public List<AudioSource> playlist;
    public List<AudioSource> losePlaylist;
    public List<AudioSource> winPlaylist;
    public List<AudioSource> storePlaylist;
    public AudioSource last;

    public AudioMixer mixer;

    public List<AudioSource> currentPlaylist;
    public List<AudioSource> all;

    public void Awake() {
        currentPlaylist = playlist;
        all = playlist.Concat(winPlaylist).Concat(losePlaylist).Concat(storePlaylist).ToList();
    }

    public void Update() {
        if (all.Any(a => a.isPlaying)) {
            return;
        }

        //if (Player.instance != null) {
        //    if (Player.instance.figure.location.board == Game.instance.mainWorld) {
        //        mixer.SetFloat("base", 0);
        //        mixer.SetFloat("store", -80);
        //    } else {
        //        mixer.SetFloat("base", -80);
        //        mixer.SetFloat("store", 0);
        //    }
        //}
    }

    public void PlayNext() {
        var next = currentPlaylist.rndExcept(last);
        next.Play();
        last = next;
    }

    public void ToRandomPlace() {
        if (last != null) {
            last.timeSamples = UnityEngine.Random.Range(0, last.clip.samples);
        }
    }

    public void Switch(List<AudioSource> playlist) {
        if (last != null) {
            last.Stop();
        }
        currentPlaylist = playlist;
        PlayNext();
        ToRandomPlace();
    }
}
