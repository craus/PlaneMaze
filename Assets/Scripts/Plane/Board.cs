using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    SparseCollections.Sparse2DChunkBasedMatrix<Cell> map = new SparseCollections.Sparse2DChunkBasedMatrix<Cell>();

    public Field field;
    public Cell cellSample;
    public Gem gemSample;
    public Teleport teleportSample;
    public Transform cellParent;
    public Transform figureParent;

    public List<Cell> cellsList = new List<Cell>();

    public Cell GetCell(Vector2Int position) {
        ShowCell(position.x, position.y);
        return map[position.x, position.y];
    }

    public Cell GetCellByPosition(Vector3 position) {
        return map[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)];
    }

    private void Start() {
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 10; j++) {
                ShowCell(i, j);
            }
        }
    }

    private void Update() {
        int xMin = (int)Mathf.Round(Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect);
        int xMax = (int)Mathf.Round(Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect);
        int yMin = (int)Mathf.Round(Camera.main.transform.position.y - Camera.main.orthographicSize);
        int yMax = (int)Mathf.Round(Camera.main.transform.position.y + Camera.main.orthographicSize);

        for (int i = xMin; i <= xMax; i++) {
            for (int j = yMin; j <= yMax; j++) {
                ShowCell(i, j);
            }
        }
    }

    private void GenerateFigure(Cell cell, MonoBehaviour sample) {
        var teleport = Instantiate(sample);
        teleport.GetComponent<Figure>().Move(cell);
        teleport.transform.SetParent(figureParent);
    }

    private void GenerateGem(Cell cell) {
        if (!cell.fieldCell.wall && !cell.fieldCell.teleport) {
            if (Rand.rndEvent(0.01f)) {
                GenerateFigure(cell, gemSample);
            }
        }
    }

    private Cell GenerateCell(int x, int y) {
        var cell = Instantiate(cellSample);
        cell.gameObject.name = $"Cell ({x}, {y})";
        cell.SetFieldCell(field[x, y]);
        cell.transform.SetParent(cellParent);
        cell.transform.position = new Vector3(x, y, 0);
        cell.position = new Vector2Int(x, y);
        cell.board = this;

        if (cell.fieldCell.teleport) {
            GenerateFigure(cell, teleportSample);
        }

        //GenerateGem();

        return cell;
    }

    private void ShowCell(int x, int y) {
        if (map[x, y] == null) {
            map[x, y] = GenerateCell(x, y);
            cellsList.Add(map[x, y]);
        }
    }

    public void UpdateAllCells() {
        cellsList.ForEach(c => c.UpdateCell());
    }
}
