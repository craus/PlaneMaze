using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    SparseCollections.Sparse2DChunkBasedMatrix<Cell> map = new SparseCollections.Sparse2DChunkBasedMatrix<Cell>();

    public Field field;
    public Cell cellSample;
    public Transform cellParent;

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

    private Cell GenerateCell(int x, int y) {
        var cell = Instantiate(cellSample);
        cell.SetFieldCell(field[x, y]);
        cell.transform.SetParent(cellParent);
        cell.transform.position = new Vector3(x, y, 0);
        return cell;
    }

    private void ShowCell(int x, int y) {
        if (map[x, y] == null) {
            map[x, y] = GenerateCell(x, y);
        }
    }
}
