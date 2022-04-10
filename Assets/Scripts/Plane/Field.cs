using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Field : MonoBehaviour
{
    SparseCollections.Sparse2DMatrix<int, int, FieldCell> map = new SparseCollections.Sparse2DMatrix<int, int, FieldCell>();

    public FieldCell this[int x, int y]
    {
        get
        {
            if (map[x, y] == null) {
                map[x,y] = Generate(x, y);
            }
            return map[x, y];
        }
    }

    private FieldCell Generate(int x, int y) {
        var result = new FieldCell();
        result.wall = Rand.rndEvent(1 / (1 + Mathf.Sqrt(2))) ? true : false;
        return result;
    }
}
