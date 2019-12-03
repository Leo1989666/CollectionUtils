using UnityEngine;

namespace MapGrid
{
    [System.Serializable]
    public class GridCell
    {
        public CellIndex Index;
        public Vector2 CellSize;
        public bool IsEmpty;
    }

    public struct CellIndex
    {
        public int Row;
        public int Col;

        public CellIndex(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}