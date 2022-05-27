using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    public Player playerSample;
    public Player player;

    public Board boardSample;
    public Board board;

    public Game gameSample;
    public Game game;

    public void Start() {
        NewGame();

        int x = 3;
        x += 4 - 2;
        Debug.Log(x);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Restart();
        }
    }

    public void Restart() {
        DestroyGame();
        NewGame();
    }

    public void NewGame() {
        board = Instantiate(boardSample);
        game = Instantiate(gameSample);

        var start = board.GetCell(new Vector2Int(0, 0));
        start.fieldCell.wall = false;
        start.Capture(forced: true);
    }

    public void DestroyGame() {
        if (player) {
            Destroy(player.gameObject);
        }
        Destroy(board.gameObject);
    }
}
