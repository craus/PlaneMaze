using System;
using System.Collections.Generic;

namespace SparseCollections
{
    /// <summary>
    /// This class implements a sparse 2 dimensional matrix with chunks.
    /// </summary>
    /// <typeparam name="TValue">The type of the array values</typeparam>
    [Serializable]
    public class Sparse2DChunkBasedMatrix<TValue>
    {
        public Sparse2DMatrix<int, int, TValue[,]> chunks = new SparseCollections.Sparse2DMatrix<int, int, TValue[,]>();

        TValue[,] lastChunk;
        int lastChunkX;
        int lastChunkY;

        private TValue[,] getChunk(int x, int y) {
            int cx = x >> 7;
            int cy = y >> 7;

            if (cx == lastChunkX && cy == lastChunkY && lastChunk != null) {
                return lastChunk;
            }

            lastChunkX = cx;
            lastChunkY = cy;
            lastChunk = chunks[cx, cy];

            if (lastChunk == null) {
                lastChunk = chunks[cx, cy] = new TValue[128, 128];
            }

            return lastChunk;
        }

        public TValue this[int x, int y]
        {
            get
            {
                return getChunk(x, y)[x & 127, y & 127];
            }
            set
            {
                getChunk(x, y)[x & 127, y & 127] = value;
            }
        }
    }
}
