using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Game))]
public class CursedSignCounter : MonoBehaviour
{
    public static int Max = 2;

    public int cursedSignCount = 0;

    public Game game;

    public virtual void Awake() {
        game = GetComponent<Game>();

        new ValueTracker<int>(() => cursedSignCount, v => cursedSignCount = v);

        game.afterPlayerMove.Add(async (index) => {
            if (cursedSignCount >= Max) {
                CursedSignIndicator.instance.Attack();
                await Player.instance.Hit(new Attack(
                    Vector2Int.zero,
                    Game.instance.generatedFigures[Library.instance.cursedSign.GetComponent<Figure>()].First(c => c.gameObject.activeSelf),
                    Player.instance.figure,
                    null,
                    Player.instance.figure.Location,
                    1
                ));
            }
        });
    }
}
