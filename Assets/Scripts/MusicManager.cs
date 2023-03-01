using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicManager : Singletone<MusicManager>
{
    public List<AudioSource> playlist;
    public AudioSource last;

    public void Update() {
        if (playlist.Any(a => a.isPlaying)) {
            return;
        }
        var next = playlist.rndExcept(last);
        next.Play();
        last = next;
    }
}
