using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Games
{
    [System.Serializable]
    public struct Cell
    {
        public int x;
        public int y;
        public bool isFilled;
        public bool isStart;
        public bool isCurrent;
    }

    public class FillColorGame : MonoBehaviour
    {
        [Header("游戏设置")] [SerializeField] private int gridSize = 7; // NxN矩阵大小
        [SerializeField] private Material cellMaterial; // 单元格材质
        [SerializeField] private float cellSize = 1f; // 单元格大小
        private GameObject[,] cellObjects; // 存储单元格游戏对象
        [SerializeField] private Color fillColor = Color.blue;
        [SerializeField] private Color startColor = Color.green;
        [SerializeField] private Color currentColor = Color.yellow;
        [SerializeField] private Color availableColor = Color.cyan;
        [SerializeField] private float animationDuration = 0.1f; // 动画持续时间

        private Cell[,] gameGrid;
        private List<Vector2Int> filledCells = new();
        private bool isUpdatingVisuals;
        private bool isGameStarted;
        private bool isGameOver;
        private int filledCount;
        private Vector2Int currentPosition;
        private List<Vector2Int> availableDirections = new();
        private GameObject cursor;

        // Start is called before the first frame update
        void Start()
        {
            EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
            EventCenter.GetInstance().AddEventListener<Vector3>("鼠标移动", OnMouseMove);
            InitializeGame();
        }

        private void InitializeGame()
        {
            // 初始化游戏网格
            gameGrid = new Cell[gridSize, gridSize];
            filledCells.Clear();
            cellObjects = new GameObject[gridSize, gridSize];

            // 创建网格单元格对象
            for (var x = 0; x < gridSize; x++)
            {
                for (var y = 0; y < gridSize; y++)
                {
                    // 创建单元格游戏对象
                    var cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cell.name = $"Cell_{x}_{y}";
                    cell.transform.parent = transform;
                    cell.transform.position = new Vector3(x * cellSize, y * cellSize, 0);
                    cell.transform.localScale = new Vector3(cellSize * 0.9f, cellSize * 0.9f, 0.1f);

                    // 添加碰撞体用于鼠标检测
                    if (cell.GetComponent<BoxCollider>() == null)
                    {
                        cell.AddComponent<BoxCollider>().size = new Vector3(1, 1, 0.1f);
                    }

                    // 设置材质
                    var r = cell.GetComponent<Renderer>();
                    if (r != null && cellMaterial != null)
                    {
                        r.material = cellMaterial;
                    }

                    cellObjects[x, y] = cell;
                }
            }

            isUpdatingVisuals = false;
            isGameStarted = false;
            isGameOver = false;
            filledCount = 0;
            Debug.Log($"初始化 {gridSize}x{gridSize} 网格游戏");
        }

        private bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
        }

        private List<Vector2Int> GetAvailableDirections(int x, int y)
        {
            var directions = new List<Vector2Int>();

            // 检查上、下、左、右四个方向
            if (IsWithinBounds(x, y - 1) && !gameGrid[x, y - 1].isFilled) directions.Add(Vector2Int.down); // 上
            if (IsWithinBounds(x, y + 1) && !gameGrid[x, y + 1].isFilled) directions.Add(Vector2Int.up); // 下
            if (IsWithinBounds(x - 1, y) && !gameGrid[x - 1, y].isFilled) directions.Add(Vector2Int.left); // 左
            if (IsWithinBounds(x + 1, y) && !gameGrid[x + 1, y].isFilled) directions.Add(Vector2Int.right); // 右

            return directions;
        }

        private void CheckGameState()
        {
            if (filledCount == gridSize * gridSize)
            {
                Debug.Log("游戏胜利！所有格子已填满！");
                isGameOver = true;
                // 可以添加胜利UI显示逻辑
            }
            else if (availableDirections.Count == 0)
            {
                Debug.Log("游戏失败！无法继续移动。");
                isGameOver = true;
                // 可以添加失败UI显示逻辑
            }
        }


        private void OnMouseMove(Vector3 mousePosition)
        {
            // 创建光标对象（如果不存在）
            if (cursor == null)
            {
                cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                cursor.name = "Cursor";
                cursor.transform.parent = transform;
                cursor.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);

                var rr = cursor.GetComponent<Renderer>();
                if (rr != null)
                {
                    rr.material = new Material(Shader.Find("Standard"));
                    rr.material.color = new Color(1, 1, 1, 0.5f); // 半透明白色
                }
            }

            if (Camera.main == null)
            {
                Debug.LogError("Main Camera not found!");
                return;
            }

            // 更新光标位置
            var worldPos =
                Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y,
                    Camera.main.nearClipPlane + 0.1f));
            cursor.transform.position = worldPos;
        }

        private void OnKeyDown(KeyCode keyCode)
        {
            if (Camera.main == null) return;

            switch (keyCode)
            {
                case KeyCode.Mouse0: // 鼠标左键
                    Debug.Log("Left mouse button pressed.");

                    if (!isGameStarted)
                    {
                        // 开始游戏 - 选择起始位置
                        // 这里需要实现获取鼠标点击的格子位置逻辑
                        // 简化实现：假设点击位置转换为网格坐标
                        // 获取鼠标点击位置并转换为世界坐标
                        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out var hit))
                        {
                            // 假设网格中心在原点，每个格子大小为1单位
                            var x = Mathf.RoundToInt(hit.point.x /*+ gridSize / 2f*/);
                            var y = Mathf.RoundToInt(hit.point.y /*+ gridSize / 2f*/);
                            if (IsWithinBounds(x, y))
                            {
                                StartGameAt(x, y);
                            }
                        }
                    }
                    else if (!isGameOver)
                    {
                        // 处理方向选择
                        HandleDirectionSelection();
                    }

                    break;
            }
        }

        private void StartGameAt(int x, int y)
        {
            if (!IsWithinBounds(x, y)) return;

            gameGrid[x, y].isFilled = true;
            gameGrid[x, y].isStart = true;
            gameGrid[x, y].isCurrent = true;
            filledCount++;
            currentPosition = new Vector2Int(x, y);
            isGameStarted = true;
            availableDirections = GetAvailableDirections(x, y);
            HandleDirectionSelection();
        }

        private void OnDestroy()
        {
            EventCenter.GetInstance().RemoveEventListener<KeyCode>("某键按下", OnKeyDown);
            // 清理单元格对象
            if (cellObjects == null) return;

            for (var x = 0; x < gridSize; x++)
            {
                for (var y = 0; y < gridSize; y++)
                {
                    Destroy(cellObjects[x, y]);
                }
            }
        }

        // 绘制网格和格子状态
        private void OnDrawGizmos()
        {
            if (gameGrid == null) return;

            // 绘制网格线
            Gizmos.color = Color.black;
            for (var x = 0; x <= gridSize; x++)
            {
                Gizmos.DrawLine(new Vector3(x - 0.5f, 0 - 0.5f, 0), new Vector3(x - 0.5f, gridSize - 0.5f, 0));
                Gizmos.DrawLine(new Vector3(0 - 0.5f, x - 0.5f, 0), new Vector3(gridSize - 0.5f, x - 0.5f, 0));
            }

            // 绘制格子状态
            for (var x = 0; x < gridSize; x++)
            {
                for (var y = 0; y < gridSize; y++)
                {
                    var cell = gameGrid[x, y];
                    var cellPosition = new Vector3(x, y, 0);
                    var gzmCellSize = new Vector3(0.9f, 0.9f, 0.1f);

                    if (cell.isStart)
                        Gizmos.color = startColor;
                    else if (cell.isCurrent)
                        Gizmos.color = currentColor;
                    else if (cell.isFilled)
                        Gizmos.color = fillColor;
                    else
                        Gizmos.color = Color.white;

                    Gizmos.DrawCube(cellPosition, gzmCellSize);
                }
            }

            // 绘制可用方向提示
            if (!isGameStarted || isGameOver) return;

            foreach (var pos in availableDirections.Select(dir =>
                         new Vector3(currentPosition.x + dir.x, currentPosition.y + dir.y, 0)))
            {
                Gizmos.color = availableColor;
                Gizmos.DrawCube(pos, new Vector3(0.8f, 0.8f, 0.1f));
            }
        }

        private void HandleDirectionSelection()
        {
            if (Camera.main == null) return;

            if (!Input.GetMouseButtonDown(0)) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;

            // 计算点击位置相对当前位置的方向
            var x = Mathf.RoundToInt(hit.point.x);
            var y = Mathf.RoundToInt(hit.point.y);
            var direction = new Vector2Int(x - currentPosition.x, y - currentPosition.y);

            Debug.Log($"选择方向: {direction} {x} {y} {hit.point}");
            while (IsValidDirection(direction))
            {
                filledCells.Add(currentPosition);
                MoveToDirection(direction);
            }

            filledCells.Add(currentPosition);

            UpdateCellVisuals();
            CheckGameState();
        }

        private bool IsValidDirection(Vector2Int direction)
        {
            // 检查是否是有效的方向向量 (只能是上下左右四个方向)
            if (Mathf.Abs(direction.x) + Mathf.Abs(direction.y) != 1)
                return false;

            return availableDirections.Contains(new Vector2Int(direction.x, direction.y));
        }

        private void MoveToDirection(Vector2Int direction)
        {
            // 清除当前位置标记
            gameGrid[currentPosition.x, currentPosition.y].isCurrent = false;

            // 使用DOTween动画移动
            var newPosition = currentPosition + direction;

            // 更新游戏状态
            currentPosition = newPosition;
            gameGrid[currentPosition.x, currentPosition.y].isFilled = true;
            gameGrid[currentPosition.x, currentPosition.y].isCurrent = true;
            filledCount++;

            // 更新可用方向
            availableDirections = GetAvailableDirections(currentPosition.x, currentPosition.y);
        }

        // 更新单元格视觉状态
        private void UpdateCellVisuals()
        {
            if (filledCells.Count == 0 || isUpdatingVisuals)
            {
                return; // 没有需要更新的单元格
            }

            isUpdatingVisuals = true;
            var delayIndex = 0;
            foreach (var rr in filledCells
                         .Select(filledCellPos =>
                             cellObjects[filledCellPos.x, filledCellPos.y].GetComponent<Renderer>())
                         .Where(rr => rr != null))
            {
                rr.material.DOColor(fillColor, 0.6f).SetDelay(animationDuration * delayIndex);
                var cellTransform = rr.transform;
                cellTransform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.3f, 1, 0.5f)
                    .SetDelay(animationDuration * delayIndex);
                delayIndex++;
            }

            filledCells.Clear();

            for (var x = 0; x < gridSize; x++)
            {
                for (var y = 0; y < gridSize; y++)
                {
                    var cell = gameGrid[x, y];
                    var rr = cellObjects[x, y].GetComponent<Renderer>();
                    if (rr == null) continue;

                    if (cell.isCurrent)
                        rr.material.DOColor(currentColor, 0.6f).SetDelay(animationDuration * delayIndex);
                    else if (cell.isStart)
                        rr.material.DOColor(startColor, 0.6f).SetDelay(animationDuration * delayIndex);
                    else if (cell.isFilled)
                    {
                    }
                    else
                        rr.material.DOColor(Color.white, 0.6f).SetDelay(animationDuration * delayIndex);
                }
            }

            // 更新可用方向提示
            if (isGameStarted && !isGameOver)
            {
                foreach (var rr in from dir in availableDirections
                         let x = currentPosition.x + dir.x
                         let y = currentPosition.y + dir.y
                         where IsWithinBounds(x, y)
                         select cellObjects[x, y].GetComponent<Renderer>()
                         into rr
                         where rr != null
                         select rr)
                {
                    rr.material.DOColor(availableColor, 0.6f).SetDelay(animationDuration * delayIndex);
                }
            }

            isUpdatingVisuals = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (isGameStarted && !isGameOver)
            {
            }
        }
    }
}