using UnityEngine;

namespace VinhLB
{
    [System.Serializable]
    public class Node
    {
        public int X = 0;
        public int Y = 0;
        public float PositionX = 0f;
        public float PositionY = 0f;
        public int Weight = 1;
        
        public GridSystem GridSystem;
        
        public Node UpNode;
        public Node DownNode;
        public Node LeftNode;
        public Node RightNode;

        public object LinkedObject;
        
        public Vector3 GetNodeRelativePosition()
        {
            Vector3 position = Vector3.zero;
            position.x = PositionX;
            position.y = PositionY;
            position += GridSystem.OriginOffset;

            return position;
        }
    }
}