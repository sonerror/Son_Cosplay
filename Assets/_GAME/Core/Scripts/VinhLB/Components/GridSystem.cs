using UnityEditor;
using UnityEngine;

namespace VinhLB
{
    public class GridSystem : MonoBehaviour
    {
        [SerializeField]
        private int _width;
        [SerializeField]
        private int _height;
        [SerializeField]
        private Vector2 _nodeOffset;
        [SerializeField]
        private Transform _originPoint;

        private Node[,] _nodeGrid;

        public int Width => _width;
        public int Height => _height;
        public int CellAmount => _width * _height;
        public Vector2 NodeOffset => _nodeOffset;
        public Vector2 CenterRelativePosition => _originPoint.position;
        public Vector2 CenterPosition { get; private set; }
        public Vector3 OriginOffset { get; private set; }
        
        public Node this[int x, int y]
        {
            get => _nodeGrid[x, y];
            set => _nodeGrid[x, y] = value;
        }

        public void Initialize(int width, int height)
        {
            _width = width;
            _height = height;
            
            Initialize();
        }
        
        public void Initialize()
        {
            if (_nodeGrid != null)
            {
                return;
            }

            _nodeGrid = new Node[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Node node = new Node
                    {
                        X = x,
                        Y = y,
                        PositionX = x * (1f - _nodeOffset.x),
                        PositionY = y * (1f - _nodeOffset.y),
                        GridSystem = this
                    };

                    _nodeGrid[x, y] = node;
                }
            }

            Node bottomLeftNode = _nodeGrid[0, 0];
            Node topRightNode = _nodeGrid[_width - 1, _height - 1];
            
            CenterPosition = new Vector2(
                (topRightNode.PositionX - bottomLeftNode.PositionX) / 2,
                (topRightNode.PositionY - bottomLeftNode.PositionY) / 2);
            
            OriginOffset = _originPoint.position - new Vector3(
                CenterPosition.x,
                CenterPosition.y,
                _originPoint.position.z);
        }

        public bool IsNodeExist(int x, int y, out Node node)
        {
            node = null;
            if (_nodeGrid != null && x >= 0 && x < _width && y >= 0 && y < _height)
            {
                node = _nodeGrid[x, y];

                return true;
            }

            // Debug.LogError($"Node [{x}, {y}] is not exist");
            return false;
        }

        public bool TryGetNearestNode(Vector3 position, out Node node, float startMinDistance = float.PositiveInfinity)
        {
            node = null;
            if (_nodeGrid != null)
            {
                float minDistance = startMinDistance;
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        Vector3 nodePosition = _nodeGrid[x, y].GetNodeRelativePosition();
                        float distance = Vector3.Distance(nodePosition, position);
                        if (distance < minDistance)
                        {
                            node = _nodeGrid[x, y];
                            minDistance = distance;
                        }
                        
                    }
                }

                if (node != null)
                {
                    return true;
                }
            }
            
            return false;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_nodeGrid != null)
            {
                GUIStyle textStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 10,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState
                    {
                        textColor = Color.cyan
                    }
                };
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        Node node = _nodeGrid[x, y];
                        Handles.Label(node.GetNodeRelativePosition(), $"({x}, {y})", textStyle);
                    }
                }
            }
        }
#endif
    }
}