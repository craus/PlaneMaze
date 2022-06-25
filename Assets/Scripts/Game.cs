using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance => GameManager.instance.game;

    public int area = 0;
    public int border = 0;
    public float time = 0;
    public float EnemyStrength => (time-50) * Mathf.Log(time + 1, 100) / 6;
    public float strengthPeak = 0;

    public float Strength => 1f * area / border;

    public bool lost = false;

    public void Lose() {
        lost = true;
    }
}
