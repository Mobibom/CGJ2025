using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MatchThreeGamePanel : BasePanel
{
    private int eliminationCount = 0; // 记录消除的格子数量

    public enum CellColor
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
        Transparent // 透明色，用于消除后的格子
    }

    /// <summary>
    /// 二维数组，存储每个格子的颜色
    /// </summary>
    private CellColor[,] gridColors;

    // Start is called before the first frame update
    void Start()
    {
        // 初始化游戏面板
        Initialize();
    }

    private void Initialize()
    {
        gridColors = new CellColor[7, 7]
        {
            { CellColor.Purple, CellColor.Purple, CellColor.Purple, CellColor.Blue, CellColor.Green, CellColor.Green, CellColor.Red },
            { CellColor.Green, CellColor.Blue, CellColor.Blue, CellColor.Red, CellColor.Yellow, CellColor.Green, CellColor.Blue },
            { CellColor.Red, CellColor.Blue, CellColor.Purple, CellColor.Red, CellColor.Blue, CellColor.Blue, CellColor.Yellow },
            { CellColor.Red, CellColor.Yellow, CellColor.Green, CellColor.Red, CellColor.Purple, CellColor.Purple, CellColor.Purple },
            { CellColor.Red, CellColor.Green, CellColor.Blue, CellColor.Blue, CellColor.Blue, CellColor.Yellow, CellColor.Yellow },
            { CellColor.Green, CellColor.Yellow, CellColor.Purple, CellColor.Yellow, CellColor.Yellow, CellColor.Green, CellColor.Green },
            { CellColor.Yellow, CellColor.Yellow, CellColor.Purple, CellColor.Red, CellColor.Red, CellColor.Red, CellColor.Green },
        };

        var cellContainer = transform.Find("CellContainer");
        if (cellContainer == null)
        {
            Debug.LogError("CellContainer 未找到");
            return;
        }

        // 初始化每个格子的颜色，并为每个格子添加鼠标点击事件
        for (int i = 0; i < gridColors.GetLength(0); i++)
        {
            for (int j = 0; j < gridColors.GetLength(1); j++)
            {
                GameObject cell = cellContainer.GetChild(i * gridColors.GetLength(1) + j).gameObject;
                cell.GetComponent<Image>().color = GetColorFromCellColor(gridColors[i, j]);

                // 添加鼠标点击事件
                Button cellButton = cell.GetComponent<Button>();
                if (cellButton == null)
                {
                    cellButton = cell.AddComponent<Button>();
                }
                int row = i;
                int col = j;
                cellButton.onClick.RemoveAllListeners();
                cellButton.onClick.AddListener(() => OnCellClicked(row, col));
            }
        }
    }

    private void OnCellClicked(int row, int col)
    {
        // 如果周围有相同颜色的格子，则执行消除逻辑
        CellColor clickedColor = gridColors[row, col];

        if (clickedColor == CellColor.Transparent)
        {
            return; // 如果格子已经是透明色，则不执行任何操作
        }

        // 记录联通格子列表
        var connectedCells = new System.Collections.Generic.List<(int, int)>();

        // BFS 检查周围有没有联通恰好三个一样颜色的格子
        System.Collections.Generic.Queue<(int, int)> queue = new System.Collections.Generic.Queue<(int, int)>();
        queue.Enqueue((row, col));
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int r = current.Item1;
            int c = current.Item2;
            // 检查是否已经处理过这个格子
            if (connectedCells.Contains((r, c)))
                continue;
            // 检查当前格子的颜色是否与点击的颜色相同
            if (gridColors[r, c] == clickedColor)
            {
                connectedCells.Add((r, c));
                // 检查四个方向
                if (r > 0) queue.Enqueue((r - 1, c)); // 上
                if (r < gridColors.GetLength(0) - 1) queue.Enqueue((r + 1, c)); // 下
                if (c > 0) queue.Enqueue((r, c - 1)); // 左
                if (c < gridColors.GetLength(1) - 1) queue.Enqueue((r, c + 1)); // 右
            }
        }

        // 如果找到三个或更多相同颜色的格子，则执行消除逻辑
        if (connectedCells.Count >= 3)
        {
            var sequence = DOTween.Sequence();

            foreach (var cell in connectedCells)
            {
                int r = cell.Item1;
                int c = cell.Item2;
                gridColors[r, c] = CellColor.Transparent; // 将格子颜色设置为透明
                GameObject cellObj = transform.Find("CellContainer").GetChild(r * gridColors.GetLength(1) + c).gameObject;

                cellObj.GetComponent<Image>().color = GetColorFromCellColor(CellColor.Transparent); // 更新格子颜色显示
            }
            Debug.Log($"消除了 {connectedCells.Count} 个相同颜色的格子。");
            eliminationCount += connectedCells.Count;

            // 消除后，上方还存在的格子下落
            for (int c = 0; c < gridColors.GetLength(1); c++)
            {
                int emptyRow = gridColors.GetLength(0) - 1; // 从底部开始查找空格子
                for (int r = gridColors.GetLength(0) - 1; r >= 0; r--)
                {
                    if (gridColors[r, c] != CellColor.Transparent)
                    {
                        // 如果当前格子不是透明色，则将其移动到空格子的位置
                        if (r != emptyRow)
                        {
                            gridColors[emptyRow, c] = gridColors[r, c];
                            GameObject cellObj = transform.Find("CellContainer").GetChild(emptyRow * gridColors.GetLength(1) + c).gameObject;
                            cellObj.GetComponent<Image>().color = GetColorFromCellColor(gridColors[r, c]);
                        }
                        emptyRow--; // 移动到下一个空格子
                    }
                }
                // 将剩余的空格子设置为透明色
                for (int r = emptyRow; r >= 0; r--)
                {
                    gridColors[r, c] = CellColor.Transparent;
                    GameObject cellObj = transform.Find("CellContainer").GetChild(r * gridColors.GetLength(1) + c).gameObject;
                    cellObj.GetComponent<Image>().color = GetColorFromCellColor(CellColor.Transparent);
                }
            }
        }
        else
        {
            Debug.Log("没有足够的相同颜色格子进行消除。");
        }

        if (eliminationCount == gridColors.GetLength(0) * gridColors.GetLength(1))
        {
            Debug.Log("游戏结束，所有格子已被消除！");
            Initialize(); // 重新初始化游戏面板
        }
    }

    private Color GetColorFromCellColor(CellColor cellColor)
    {
        switch (cellColor)
        {
            case CellColor.Red:
                return Color.red;
            case CellColor.Blue:
                return Color.blue;
            case CellColor.Green:
                return Color.green;
            case CellColor.Yellow:
                return Color.yellow;
            case CellColor.Purple:
                return new Color(0.5f, 0, 0.5f); // 紫色
            case CellColor.Transparent:
                return new Color(0, 0, 0, 0); // 透明色
            default:
                return Color.white; // 默认颜色
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
