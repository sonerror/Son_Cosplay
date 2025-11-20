using UnityEngine;

namespace VinhLB.Grid
{
    public abstract class CoordinateConverter
    {
        public abstract Vector3 Forward { get; }

        public abstract Vector3 GridToWorld(int x, int y, Vector2 cellSize, Vector3 origin);
            
        public abstract Vector3 GridToWorldCenter(int x, int y, Vector2 cellSize, Vector3 origin);
            
        public abstract Vector2Int WorldToGrid(Vector3 worldPosition, Vector2 cellSize, Vector3 origin);
    }

    // Plane XY
    public class VerticalConverter : CoordinateConverter
    {
        public override Vector3 Forward => Vector3.forward;
        
        public override Vector3 GridToWorld(int x, int y, Vector2 cellSize, Vector3 origin)
        {
            return new Vector3(x * cellSize.x, y * cellSize.y, 0) + origin;
        }

        public override Vector3 GridToWorldCenter(int x, int y, Vector2 cellSize, Vector3 origin)
        {
            return new Vector3(x * cellSize.x + cellSize.x * 0.5f, y * cellSize.y + cellSize.y * 0.5f, 0) + origin;
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition, Vector2 cellSize, Vector3 origin)
        {
            Vector3 gridPosition = (worldPosition - origin) / cellSize;
            int x = Mathf.FloorToInt(gridPosition.x);
            int y = Mathf.FloorToInt(gridPosition.y);

            return new Vector2Int(x, y);
        }
    }
}