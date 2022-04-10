using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    SparseCollections.Sparse2DMatrix<int, int, Cell> map = new SparseCollections.Sparse2DMatrix<int, int, Cell>();

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
