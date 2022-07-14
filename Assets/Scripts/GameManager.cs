using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    public Player playerSample;
    public Player player;

    public IEnumerable<Cell> cellOrder;
    public int unlockedCells;

    public Board boardSample;
    public Board board;

    public Gem gemSample;
    public Gem gem;

    public Cell lastLocation;

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
        player.figure.savePoint = board.GetCell(Vector2Int.zero);
        player.figure.Move(board.GetCell(Vector2Int.zero));
        Debug.LogFormat("New game started");
        cellOrder = Algorithm.Prim(
            start: player.figure.location,
            edges: c => c.Neighbours().Where(c => !c.Wall).Select(c => new Algorithm.Weighted<Cell>(c, Random.Range(0, 1f))),
            maxSteps: 10000
        ).ToList();
        unlockedCells = 50;
        cellOrder.ForEach((i, c) => {
            c.order = i;
            c.UpdateCell();
        });

        gem = Instantiate(gemSample);
        gem.GetComponent<Figure>().Move(cellOrder.Take(unlockedCells).Rnd());
        lastLocation = player.figure.location;

        Debug.LogFormat($"Cells: {cellOrder.Count()}");
    }

    public void OnGemTaken() {
        unlockedCells += 50;
        cellOrder.ForEach(c => c.UpdateCell());
        gem = Instantiate(gemSample);
        gem.GetComponent<Figure>().Move(cellOrder.Take(unlockedCells).Rnd());
        //gem.GetComponentInChildren<SpriteRenderer>().enabled = false;
        //player.figure.Move();
        //CameraControl.instance.TeleportToPlayer();
        lastLocation = player.figure.location;
    }

    public void DestroyGame() {
        Destroy(player.gameObject);
        Destroy(board.gameObject);
        Destroy(gem.gameObject);
    }
}
