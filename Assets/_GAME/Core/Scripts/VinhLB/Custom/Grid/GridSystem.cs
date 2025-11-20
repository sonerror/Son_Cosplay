using System;
using TMPro;
using UnityEngine;

namespace VinhLB.Grid
{
    public class GridSystem<T>
    {
        private int _width;
        private int _height;
        private Vector2 _cellSize;
        private Vector3 _origin;
        private T[,] _grid;
        
        private CoordinateConverter _coordinateConverter;

        public event Action<int, int, T> ValueChanged;

        public GridSystem(int width, int height, Vector2 cellSize, Vector3 origin, CoordinateConverter coordinateConverter)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _origin = origin;
            _coordinateConverter = coordinateConverter;
            _grid = new T[_width, _height];
        }
        
        public GridSystem(int width, int height, Vector2 cellSize, CoordinateConverter coordinateConverter, Vector3 center)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _coordinateConverter = coordinateConverter;
            _origin = CalculateOriginByCenter(width, height, cellSize, center, coordinateConverter);
            _grid = new T[_width, _height];
        }

        public T GetValue(int x, int y)
        {
            if (IsValid(x, y))
            {
                return _grid[x, y];
            }
            
            return default(T);
        }

        public T GetValue(Vector3 worldPosition)
        {
            Vector2Int gridPosition = GetGridPosition(worldPosition);
            
            return GetValue(gridPosition.x, gridPosition.y);
        }

        public void SetValue(int x, int y, T value)
        {
            if (IsValid(x, y))
            {
                _grid[x, y] = value;
                
                ValueChanged?.Invoke(x, y, value);
            }
        }

        public void SetValue(Vector3 worldPosition, T value)
        {
            Vector2Int gridPosition = GetGridPosition(worldPosition);
            
            SetValue(gridPosition.x, gridPosition.y, value);
        }
        
        public Vector3 GetWorldPosition(int x, int y) => _coordinateConverter.GridToWorld(x, y, _cellSize, _origin);
        
        public Vector3 GetWorldPositionCenter(int x, int y) => _coordinateConverter.GridToWorldCenter(x, y, _cellSize, _origin);
        
        public Vector2Int GetGridPosition(Vector3 worldPosition) => _coordinateConverter.WorldToGrid(worldPosition, _cellSize, _origin);
        
        public bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < _width && y < _height;

        public void DrawDebugLines()
        {
            float duration = 100f;
            GameObject debuggingGO = new GameObject("Debugging");

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    CreateWorldText(debuggingGO.transform, $"{x}, {y}", GetWorldPositionCenter(x, y), _coordinateConverter.Forward);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, duration);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, duration);
                }
            }
            
            Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, duration);
        }

        private TextMeshPro CreateWorldText(Transform parent, string text, Vector3 position, Vector3 forward,
            int fontSize = 2, Color color = default, TextAlignmentOptions alignment = TextAlignmentOptions.Center, int sortingOrder = 0)
        {
            GameObject worldTextGO = new GameObject($"DebugText_{text}", typeof(TextMeshPro));
            worldTextGO.transform.SetParent(parent);
            worldTextGO.transform.position = position;
            worldTextGO.transform.forward = forward;
            
            TextMeshPro textMeshPro = worldTextGO.GetComponent<TextMeshPro>();
            textMeshPro.text = text;
            textMeshPro.fontSize = fontSize;
            textMeshPro.color = color == default ? Color.white : color;
            textMeshPro.alignment = alignment;
            textMeshPro.sortingOrder = sortingOrder;
            
            return textMeshPro;
        }

        public static Vector3 CalculateOriginByCenter(int width, int height, Vector2 cellSize, Vector3 center, 
            CoordinateConverter coordinateConverter)
        {
            if (width <= 0 || height <= 0 || cellSize.x <= 0 || cellSize.y <= 0)
            {
                return Vector3.zero;
            }
            
            Vector3 gridCenter = Vector3.zero;
            gridCenter.x = width / 2 * cellSize.x;
            if (width % 2 != 0)
            {
                gridCenter.x += cellSize.x / 2;
            }
            gridCenter.y = height / 2 * cellSize.y;
            if (height % 2 != 0)
            {
                gridCenter.y += cellSize.y / 2;
            }
            
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, coordinateConverter.Forward);
            Vector3 offset = rotation * gridCenter;
            
            return center - offset;
        }
    }
}