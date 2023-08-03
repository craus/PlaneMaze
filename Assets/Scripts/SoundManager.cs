using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : Singletone<SoundManager>
{
    public AudioSource push;
    public AudioSource pushAttack;
    public AudioSource meleeAttack;
    public AudioSource rangedAttack;
    public AudioSource monsterMeleeAttack;
    public AudioSource monsterRangedAttack;
    public AudioSource heroDamaged;
    public AudioSource itemPick;
    public AudioSource gemPick;
    public AudioSource monsterDeath;
    public AudioSource playerDeath;
    public AudioSource teleport;
    public AudioSource gargoyleWakeup;
    public AudioSource swampDebuff;
    public AudioSource peaceDebuff;
    public AudioSource additionalMove;
    public AudioSource wolftrapAttack;
    public AudioSource buy;
    public AudioSource defence;
    public AudioSource bowCharge;
    public AudioSource heal;
    public AudioSource consumeSoul;
    public AudioSource lichDeath;
    public AudioSource terraform;
    public AudioSource spikeBootsAttack;
    public AudioSource failedAction;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.F4)) {
            gemPick.Play();
        }
    }
}
