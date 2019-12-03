using UnityEngine;

namespace MapGrid
{
    public class GridMapController : MonoBehaviour
    {
        [SerializeField]
        private LayerMask CollidedLayerMask;

        [SerializeField]
        private Vector2 GridSize;
        
        [SerializeField]
        private int MaxOffSetCheckLength = 5;
        
        public GridMapEngine GridMapEngine { get; private set; }

        public void Initialize()
        {
            GridMapEngine = new GridMapEngine();
            GridMapEngine.GridSize = GridSize;
            GridMapEngine.Initialize(transform);
        }

        public bool FindAvailablePositionOnMap(Vector3 worldPos, float radius, ref Vector3 availablePositionOnMap)
        {
            bool result;
            
            // Recalc to fit cell size with radius
            GridMapEngine.SetNewCellSize(radius, radius);

            CellIndex index = GridMapEngine.GetIndexOfWorldPosition(worldPos);
            
            bool isVertifyOkie = VertifyCorrectlyIndex(index);
            bool isHaveCollided = GridMapEngine.CheckCellIndexCollision(index, CollidedLayerMask);

            if (isHaveCollided
                || !isVertifyOkie)
            {
                result = GetAvailableAroundPosition(index, radius, ref availablePositionOnMap);
            }
            else
            {
                availablePositionOnMap = worldPos;
                result = true;
            }
            
//            Debug.LogFormat("<>===## RESULT: {0}", result);

            return result;
        }

        private bool GetAvailableAroundPosition(CellIndex centerIndex, float radius, ref Vector3 availablePositionOnMap)
        {
            bool result = false;
            
            int X = MaxOffSetCheckLength, Y = MaxOffSetCheckLength;

            int x = 0, y = 0, dx = 0, dy = -1;
            int t = Mathf.Max(X, Y);
            int maxI = t * t;

            for (int i = 0; i < maxI; i++)
            {
                if ((-X / 2 <= x) && (x <= X / 2) && (-Y / 2 <= y) && (y <= Y / 2))
                {
//                    System.out.println(x + "," + y);

                    CellIndex index = new CellIndex();
                    index.Col = centerIndex.Col + x;
                    index.Row = centerIndex.Row + y;
                    
//                    Debug.LogFormat("<>===### CenterRow: {0} // CenterCol: {1} // x: {2} // y: {3}", centerIndex.Row, centerIndex.Col, x, y);
                    
                    if (VertifyCorrectlyIndex(index))
                    {
                        bool isCollided = GridMapEngine.CheckCellIndexCollision(index, CollidedLayerMask);
                        if (!isCollided)
                        {
                            result = true;
                            availablePositionOnMap = GridMapEngine.GetWorldPositionOfIndex(index);
                            break;
                        }
                    }
                }

                if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
                {
                    t = dx;
                    dx = -dy;
                    dy = t;
                }

                x += dx;
                y += dy;
            }
            
            return result;
        }

        public bool VertifyCorrectlyIndex(CellIndex index)
        {
            bool isOkie = index.Col >= 0 && index.Row >= 0
                          && index.Col < GridMapEngine.Col_Cells
                          && index.Row < GridMapEngine.Row_Cells;
            return isOkie;
        }
        
//#if UNITY_EDITOR
//        public LayerMask TestTargetLayerToCheck;
//        private void OnDrawGizmos()
//        {
//            for (int row = 0; row < GridMapEngine.Row_Cells; row++)
//            {
//                for (int col = 0; col < GridMapEngine.Col_Cells; col++)
//                {
//                    DebugDrawCell(new CellIndex(row, col), Color.white);
//                }
//            }
//        }
//
//        private void DebugDrawCell(CellIndex index, Color color)
//        {
//            Vector3 worldCenter = GridMapEngine.GetWorldPositionOfIndex(index);
//
//            Gizmos.color = color;
//
//            Vector3[] corners = GridMapEngine.GetWorldCornersOfCell(index);
//            
//            Vector3 lb = corners[0];
//            Vector3 lt = corners[1];
//            Vector3 rt = corners[2];
//            Vector3 rb = corners[3];
//            
//            Gizmos.DrawLine(lb, rb);
//            Gizmos.DrawLine(rb, rt);
//            Gizmos.DrawLine(rt, lt);
//            Gizmos.DrawLine(lt, lb);
//
//            Gizmos.DrawSphere(worldCenter, 0.2f);
//        }
//#endif
    }
}