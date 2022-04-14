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

    public void Start() {
        NewGame();
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
        player = Instantiate(playerSample);
        board = Instantiate(boardSample);
        player.figure.Move(board.GetCell(Vector2Int.zero));
    }

    public void DestroyGame() {
        Destroy(player.gameObject);
        Destroy(board.gameObject);
    }
}
