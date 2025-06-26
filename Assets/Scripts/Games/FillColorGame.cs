using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

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
    [Header("游戏设置")] [SerializeField] private int gridSize = 5; // NxN矩阵大小
    [SerializeField] private Material cellMaterial; // 单元格材质
    [SerializeField] private float cellSize = 1f; // 单元格大小
    private GameObject[,] cellObjects; // 存储单元格游戏对象
    [SerializeField] private Color fillColor = Color.blue;
    [SerializeField] private Color startColor = Color.green;
    [SerializeField] private Color currentColor = Color.yellow;
    [SerializeField] private Color availableColor = Color.cyan;

    private Cell[,] gameGrid;
    private bool isGameStarted = false;
    private bool isGameOver = false;
    private int filledCount = 0;
    private Vector2Int currentPosition;
    private List<Vector2Int> availableDirections = new List<Vector2Int>();

    private bool m_IsMouseEntered = false;

    public void OnMouseEnter()
    {
        m_IsMouseEntered = true;
    }

    public void OnMouseExit()
    {
        m_IsMouseEntered = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
        InitializeGame();
    }

    private void InitializeGame()
    {
        // 初始化游戏网格
        gameGrid = new Cell[gridSize, gridSize];
        cellObjects = new GameObject[gridSize, gridSize];

        // 创建网格单元格对象
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // 创建单元格游戏对象
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cell.name = $"Cell_{x}_{y}";
                cell.transform.parent = transform;
                cell.transform.position = new Vector3(x * cellSize, y * cellSize, 0);
                cell.transform.localScale = new Vector3(cellSize * 0.9f, cellSize * 0.9f, 0.1f);

                // 添加碰撞体用于鼠标检测
                if (cell.GetComponent<BoxCollider>() == null)
                {
                    BoxCollider collider = cell.AddComponent<BoxCollider>();
                    collider.size = new Vector3(1, 1, 0.1f);
                }

                // 设置材质
                Renderer renderer = cell.GetComponent<Renderer>();
                if (renderer != null && cellMaterial != null)
                {
                    renderer.material = cellMaterial;
                }

                cellObjects[x, y] = cell;
            }
        }

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
        List<Vector2Int> directions = new List<Vector2Int>();

        // 检查上、下、左、右四个方向
        if (IsWithinBounds(x, y - 1) && !gameGrid[x, y - 1].isFilled) directions.Add(Vector2Int.up); // 上
        if (IsWithinBounds(x, y + 1) && !gameGrid[x, y + 1].isFilled) directions.Add(Vector2Int.down); // 下
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

    private void OnKeyDown(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Mouse0: // 鼠标左键
                Debug.Log("Left mouse button pressed.");
                if (m_IsMouseEntered)
                {
                    if (!isGameStarted)
                    {
                        // 开始游戏 - 选择起始位置
                        // 这里需要实现获取鼠标点击的格子位置逻辑
                        // 简化实现：假设点击位置转换为网格坐标
                        // 获取鼠标点击位置并转换为世界坐标
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out RaycastHit hit))
                        {
                            // 假设网格中心在原点，每个格子大小为1单位
                            int x = Mathf.RoundToInt(hit.point.x + gridSize / 2f);
                            int y = Mathf.RoundToInt(hit.point.y + gridSize / 2f);
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
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("某键按下", OnKeyDown);
        // 清理单元格对象
        if (cellObjects != null)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    Destroy(cellObjects[x, y]);
                }
            }
        }
    }

    // 绘制网格和格子状态
    private void OnDrawGizmos()
    {
        if (gameGrid == null) return;

        // 绘制网格线
        Gizmos.color = Color.black;
        for (int x = 0; x <= gridSize; x++)
        {
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, gridSize, 0));
            Gizmos.DrawLine(new Vector3(0, x, 0), new Vector3(gridSize, x, 0));
        }

        // 绘制格子状态
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Cell cell = gameGrid[x, y];
                Vector3 cellPosition = new Vector3(x + 0.5f, y + 0.5f, 0);
                Vector3 cellSize = new Vector3(0.9f, 0.9f, 0.1f);

                if (cell.isStart)
                    Gizmos.color = startColor;
                else if (cell.isCurrent)
                    Gizmos.color = currentColor;
                else if (cell.isFilled)
                    Gizmos.color = fillColor;
                else
                    Gizmos.color = Color.white;

                Gizmos.DrawCube(cellPosition, cellSize);
            }
        }

        // 绘制可用方向提示
        if (isGameStarted && !isGameOver)
        {
            foreach (var dir in availableDirections)
            {
                Vector3 pos = new Vector3(currentPosition.x + dir.x + 0.5f, currentPosition.y + dir.y + 0.5f, 0);
                Gizmos.color = availableColor;
                Gizmos.DrawCube(pos, new Vector3(0.8f, 0.8f, 0.1f));
            }
        }
    }

    private void HandleDirectionSelection()
    {
        if (Input.GetMouseButtonDown(0) && m_IsMouseEntered)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 计算点击位置相对当前位置的方向
                int x = Mathf.RoundToInt(hit.point.x - 0.5f);
                int y = Mathf.RoundToInt(hit.point.y - 0.5f);
                Vector2Int direction = new Vector2Int(x - currentPosition.x, y - currentPosition.y);

                if (IsValidDirection(direction))
                {
                    MoveToDirection(direction);
                }
            }
        }
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

        // 更新当前位置
        currentPosition += direction;
        gameGrid[currentPosition.x, currentPosition.y].isFilled = true;
        gameGrid[currentPosition.x, currentPosition.y].isCurrent = true;
        filledCount++;

        // 更新可用方向
        availableDirections = GetAvailableDirections(currentPosition.x, currentPosition.y);
        CheckGameState();
    }

    // 更新单元格视觉状态
    private void UpdateCellVisuals()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Cell cell = gameGrid[x, y];
                Renderer renderer = cellObjects[x, y].GetComponent<Renderer>();
                if (renderer == null) continue;

                if (cell.isCurrent)
                    renderer.material.color = currentColor;
                else if (cell.isStart)
                    renderer.material.color = startColor;
                else if (cell.isFilled)
                    renderer.material.color = fillColor;
                else
                    renderer.material.color = Color.white;
            }
        }

        // 更新可用方向提示
        if (isGameStarted && !isGameOver)
        {
            foreach (var dir in availableDirections)
            {
                int x = currentPosition.x + dir.x;
                int y = currentPosition.y + dir.y;
                if (IsWithinBounds(x, y))
                {
                    Renderer renderer = cellObjects[x, y].GetComponent<Renderer>();
                    if (renderer != null)
                        renderer.material.color = availableColor;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted && !isGameOver)
        {
            UpdateCellVisuals();
            HandleDirectionSelection();
        }
    }
}