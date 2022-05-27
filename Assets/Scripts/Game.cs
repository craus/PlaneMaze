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
}
