using UnityEngine;
using UnityEngine.Serialization;

namespace MapGrid
{
    public class GridMapEngine
    {
        public Vector2 GridSize;
        public Vector2 CellSize;
        
        public int Row_Cells = 100;
        public int Col_Cells = 100;

        public Transform MapGameObject { get; private set; }

        public void Initialize(Transform mapGameObject)
        {
            MapGameObject = mapGameObject;
            // Find unity size of a cell
            CalcCellValues();
        }

        public void CalcCellValues()
        {
            Row_Cells = Mathf.CeilToInt(GridSize.y * 1f / CellSize.y);
            Col_Cells = Mathf.CeilToInt(GridSize.x * 1f / CellSize.x);
        }
        
        public void SetNewCellSize(float cellWidth, float cellHeight)
        {
            float unitWidth = Mathf.RoundToInt(Col_Cells / cellWidth);
            float unitHeight = Mathf.RoundToInt(Row_Cells / cellHeight);

            CellSize.x = Col_Cells / unitWidth;
            CellSize.y = Row_Cells / unitHeight;
            
            CalcCellValues();
        }
        
        public CellIndex GetIndexOfWorldPosition(Vector3 worldPosition)
        {
            // Transform to localposition
            Vector3 localPosition = TransformWorldToLocal(worldPosition);
            
            // Change pivot
            localPosition.x += GridSize.x * 0.5f;
            localPosition.z += GridSize.y * 0.5f;
            
            CellIndex index = new CellIndex();
            index.Col = Mathf.FloorToInt(localPosition.x / CellSize.x);
            index.Row = Mathf.FloorToInt(localPosition.z / CellSize.y);

            return index;
        }

        public Vector3 GetWorldPositionOfIndex(CellIndex index)
        {
            Vector3 localPosition = new Vector3();
            
            localPosition.x = index.Col * CellSize.x;
            localPosition.z = index.Row * CellSize.y;
            
            // Change pivot
            localPosition.x -= GridSize.x * 0.5f;
            localPosition.z -= GridSize.y * 0.5f;
            
            // transform to world position
            Vector3 worldPosition = TransformLocalToWorld(localPosition);
            
            return worldPosition;
        }
        
        public Vector3 GetCenteredPivotWorldPositionOfIndex(CellIndex index)
        {
            Vector3 localPosition = new Vector3();
            
            localPosition.x = index.Col * CellSize.x;
            localPosition.z = index.Row * CellSize.y;
            
            // Change pivot of whole Grid
            localPosition.x -= GridSize.x * 0.5f;
            localPosition.z -= GridSize.y * 0.5f;
            
            // Center align pivot
            localPosition.x += CellSize.x * 0.5f;
            localPosition.z += CellSize.y * 0.5f; 
            
            // transform to world position
            Vector3 worldPosition = TransformLocalToWorld(localPosition);
            
            return worldPosition;
        }

        public Vector3 TransformLocalToWorld(Vector3 localPosition)
        {
            return MapGameObject.TransformPoint(localPosition);
        }

        public Vector3 TransformWorldToLocal(Vector3 worldPosition)
        {
            return MapGameObject.InverseTransformPoint(worldPosition);
        }
        
        public Vector3[] GetWorldCornersOfCell(CellIndex index)
        {
            Vector3 localPosition = GetWorldPositionOfIndex(index);
            localPosition = TransformWorldToLocal(localPosition);
            
            Vector3[] corners = new Vector3[4];
            
            // Get corners local pos
            Vector3 min = localPosition;
            min.x = localPosition.x;
            min.z = localPosition.z;
            
            Vector3 max = localPosition;
            max.x += CellSize.x;
            max.z += CellSize.y;
            
            corners[0] = min;
            corners[1] = new Vector3(min.x, 0f, max.z);
            corners[2] = max;
            corners[3] = new Vector3(max.x, 0f, min.z);
            
            // Transform from local to world
            for (int i = 0; i < 4; i++)
            {
                Vector3 corner = corners[i];

                corner = TransformLocalToWorld(corner);
                corners[i] = corner;
            }
            
            return corners;
        }
        
        public bool CheckCellIndexCollision(CellIndex index, LayerMask checkLayer)
        {
            Vector3 center = GetCenteredPivotWorldPositionOfIndex(index);
            
            // change pivot to center of cell
            center.y = 0.5f;
            Collider[] colliders = Physics.OverlapBox(center, new Vector3(CellSize.x * 0.5f, 0.4f, CellSize.y * 0.5f)
                , MapGameObject.rotation, checkLayer);
            
//            for (int i = 0; i < colliders.Length; i++)
//            {
//                Debug.LogFormat("<>===###: {0}", colliders[i].gameObject.name);
//            }

            bool isCollided = colliders.Length > 0;

            return isCollided;
        }
//#if UNITY_EDITOR
//        public LayerMask TestTargetLayerToCheck;
//        private void OnDrawGizmos()
//        {
////            for (int row = 0; row < Row_Cells; row++)
////            {
////                for (int col = 0; col < Col_Cells; col++)
////                {
////                    DebugDrawCell(new CellIndex(row, col), Color.white);
////                }
////            }
//
//            
//            if (Input.GetMouseButton(0))
//            {
//                Vector3 testPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//                testPos.y = 1f;
//
//                bool isCollision = CheckCellIndexCollision(GetIndexOfWorldPosition(testPos), TestTargetLayerToCheck);
//
//                if (isCollision)
//                {
//                    DebugDrawCell(GetIndexOfWorldPosition(testPos), Color.red);
//                }
//                else
//                {
//                    DebugDrawCell(GetIndexOfWorldPosition(testPos), Color.blue);
//                }
//            }
//        }
//
//        private void DebugDrawCell(CellIndex index, Color color)
//        {
//            Vector3 worldCenter = GetWorldPositionOfIndex(index);
//
//            Gizmos.color = color;
//
//            Vector3[] corners = GetWorldCornersOfCell(index);
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