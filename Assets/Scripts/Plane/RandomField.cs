using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomField
{
    SparseCollections.Sparse2DMatrix<int, int, float> f = new SparseCollections.Sparse2DMatrix<int, int, float>(-1);

    public float this[int x, int y]
    {
        get
        {
            if (f[x, y] == -1) {
                f[x, y] = Random.Range(0, 1f);
            }
            return f[x, y];
        }
    }
}
