using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Games
{
    [System.Serializable]
    public struct Cell
    {
        public GameObject obj;
        public bool isFilled;
        public bool isStart;
        public bool isCurrent;
        public bool isBarrier; // 新增障碍标记
    }

    public class FillColorGame : MonoBehaviour
    {
        [Header("游戏设置")] [SerializeField] private int gridSize = 5; // NxN矩阵大小

        [SerializeField] private List<Vector2> barriers = new(
            new[] { new Vector2(1, 1), new Vector2(2, 2), new Vector2(3, 3) }); // 障碍物位置列表

        [SerializeField] private Material cellMaterial; // 单元格材质
        [SerializeField] private float cellSize = 1f; // 单元格大小
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite fillSprite;
        [SerializeField] private Sprite startSprite;
        [SerializeField] private Sprite currentSprite;
        [SerializeField] private Sprite availableSprite;
        [SerializeField] private Sprite barrierSprite;
        [Header("颜色设置")] [SerializeField] private Color fillSpriteColor = Color.white;
        [SerializeField] private Color availableSpriteColor = Color.white;
        [SerializeField] private Color barrierSpriteColor = Color.white;
        [SerializeField] private Color defaultCellColor = Color.white; // 新增默认颜色
        [SerializeField] private float animationDuration = 0.1f; // 动画持续时间

        private Cell[,] cells;
        private readonly List<Vector2Int> filledCells = new();
        private bool isUpdatingVisuals;
        private bool isGameStarted;
        private bool isGameOver;
        private int filledCount;
        private Vector2Int currentPosition;
        private List<Vector2Int> availableDirections = new();
        private readonly List<SpriteRenderer> lastDirectionRenderer = new();
        private GameObject cursor;

        // Start is called before the first frame update
        void Start()
        {
            EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
            // EventCenter.GetInstance().AddEventListener<Vector3>("鼠标移动", OnMouseMove);
            ResetGame();
        }

        private void ResetGame()
        {
            if (cells != null)
            {
                foreach (var cell in cells)
                {
                    Destroy(cell.obj);
                }
            }

            // 重置游戏状态
            // 初始化游戏网格
            cells = new Cell[gridSize, gridSize];

            // 计算中心偏移量（以父物体transform.position为中心）
            Vector3 center = transform.position;
            float offset = (gridSize - 1) * cellSize / 2f;
            Vector3 parentScale = transform.lossyScale;

            // 创建网格单元格对象
            for (var x = 0; x < gridSize; x++)
            {
                for (var y = 0; y < gridSize; y++)
                {
                    // 计算缩放后的位置
                    Vector3 localPos = new Vector3(x * cellSize - offset, y * cellSize - offset, 0.1f);
                    Vector3 scaledLocalPos = Vector3.Scale(localPos, parentScale);
                    var obj = new GameObject
                    {
                        name = $"Cell_{x}_{y}",
                        transform =
                        {
                            parent = transform,
                            position = center + scaledLocalPos,
                            localScale = Vector3.Scale(new Vector3(cellSize, cellSize, 1), parentScale)
                        }
                    };
                    var spr = obj.AddComponent<SpriteRenderer>();
                    spr.sprite = emptySprite;
                    // 设置SpriteRenderer的锚点到中心（Unity默认锚点就是中心，无需额外设置）
                    // 如果有RectTransform（用于UI），则设置anchor和pivot为中心
                    var rectTransform = obj.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                        rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    }

                    // 添加碰撞体用于鼠标检测
                    if (obj.GetComponent<BoxCollider>() == null)
                    {
                        var collider = obj.AddComponent<BoxCollider>();
                        collider.size = new Vector3(1, 1, 0.1f);
                        collider.center = Vector3.zero; // 保证碰撞体居中
                    }

                    // 设置材质
                    // var r = obj.GetComponent<Renderer>();
                    // if (r != null && cellMaterial != null)
                    // {
                    //     r.material = cellMaterial;
                    // }

                    // 缩放cell以适应父物体缩放
                    obj.transform.localScale = Vector3.Scale(new Vector3(cellSize, cellSize, 1), parentScale);

                    cells[x, y].obj = obj;
                }
            }

            isUpdatingVisuals = false;
            isGameStarted = false;
            isGameOver = false;

            currentPosition = Vector2Int.zero;
            availableDirections.Clear();
            filledCells.Clear();
            lastDirectionRenderer.Clear();

            // 添加障碍物
            foreach (var barrier in barriers)
            {
                SetBarrier((int)barrier.x, (int)barrier.y);
            }

            filledCount = barriers.Count;

            Debug.Log("游戏已重置");
        }

        private void SetBarrier(int x, int y)
        {
            if (!IsWithinBounds(x, y)) return;

            var cell = cells[x, y];
            var rr = cell.obj.GetComponent<SpriteRenderer>();
            cell.isFilled = true; // 设置为障碍
            cell.isBarrier = true; // 新增障碍标记
            rr.sprite = barrierSprite; // 设置障碍图片
            rr.color = new Color(barrierSpriteColor.r, barrierSpriteColor.g, barrierSpriteColor.b, 0f);
            rr.DOFade(1.0f, 0.6f);
            rr.DOColor(barrierSpriteColor, 0.6f);
            cells[x, y] = cell;
        }

        private bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
        }

        private List<Vector2Int> GetAvailableDirections(int x, int y)
        {
            var directions = new List<Vector2Int>();

            // 检查上、下、左、右四个方向
            if (IsWithinBounds(x, y - 1) && !cells[x, y - 1].isFilled) directions.Add(Vector2Int.down); // 上
            if (IsWithinBounds(x, y + 1) && !cells[x, y + 1].isFilled) directions.Add(Vector2Int.up); // 下
            if (IsWithinBounds(x - 1, y) && !cells[x - 1, y].isFilled) directions.Add(Vector2Int.left); // 左
            if (IsWithinBounds(x + 1, y) && !cells[x + 1, y].isFilled) directions.Add(Vector2Int.right); // 右

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
                        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out var hit))
                        {
                            float offset = (gridSize - 1) * cellSize / 2f;
                            Vector3 parentScale = transform.lossyScale;
                            Vector3 localPos = hit.point - transform.position;
                            // 反缩放
                            Vector3 unscaledLocalPos = new Vector3(
                                parentScale.x != 0 ? localPos.x / parentScale.x : 0,
                                parentScale.y != 0 ? localPos.y / parentScale.y : 0,
                                0
                            );
                            int x = Mathf.RoundToInt((unscaledLocalPos.x + offset) / cellSize);
                            int y = Mathf.RoundToInt((unscaledLocalPos.y + offset) / cellSize);
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
            if (!IsWithinBounds(x, y) || cells[x, y].isFilled) return;

            cells[x, y].isFilled = true;
            cells[x, y].isStart = true;
            cells[x, y].isCurrent = true;
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
            if (cells == null) return;

            for (var x = 0; x < gridSize; x++)
            {
                for (var y = 0; y < gridSize; y++)
                {
                    Destroy(cells[x, y].obj);
                }
            }
        }

        // 绘制网格和格子状态
        private void OnDrawGizmos()
        {
            if (cells == null) return;

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
                    var cell = cells[x, y];
                    var cellPosition = new Vector3(x, y, 0);
                    var gzmCellSize = new Vector3(0.9f, 0.9f, 0.1f);

                    if (cell.isStart)
                        Gizmos.color = Color.magenta;
                    else if (cell.isCurrent)
                        Gizmos.color = Color.blue;
                    else if (cell.isFilled)
                        Gizmos.color = Color.cyan;
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
                Gizmos.color = Color.green;
                Gizmos.DrawCube(pos, new Vector3(0.8f, 0.8f, 0.1f));
            }
        }

        private void HandleDirectionSelection()
        {
            if (Camera.main == null) return;

            if (!Input.GetMouseButtonDown(0)) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;

            float offset = (gridSize - 1) * cellSize / 2f;
            Vector3 parentScale = transform.lossyScale;
            Vector3 localPos = hit.point - transform.position;
            // 反缩放
            Vector3 unscaledLocalPos = new Vector3(
                parentScale.x != 0 ? localPos.x / parentScale.x : 0,
                parentScale.y != 0 ? localPos.y / parentScale.y : 0,
                0
            );
            int x = Mathf.RoundToInt((unscaledLocalPos.x + offset) / cellSize);
            int y = Mathf.RoundToInt((unscaledLocalPos.y + offset) / cellSize);
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
            cells[currentPosition.x, currentPosition.y].isCurrent = false;

            // 使用DOTween动画移动
            var newPosition = currentPosition + direction;

            // 更新游戏状态
            currentPosition = newPosition;
            cells[currentPosition.x, currentPosition.y].isFilled = true;
            cells[currentPosition.x, currentPosition.y].isCurrent = true;
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

            foreach (var rr in lastDirectionRenderer)
            {
                rr.sprite = emptySprite; // 清除上次的方向提示
                rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration);
                rr.DOColor(defaultCellColor, 0.6f).SetDelay(animationDuration); // 渐变恢复为默认颜色
            }

            lastDirectionRenderer.Clear();

            var delayIndex = 0;
            foreach (var rr in filledCells
                         .Select(filledCellPos =>
                             cells[filledCellPos.x, filledCellPos.y].obj.GetComponent<SpriteRenderer>())
                         .Where(rr => rr != null))
            {
                rr.sprite = fillSprite;
                rr.color = new Color(fillSpriteColor.r, fillSpriteColor.g, fillSpriteColor.b, 0.5f);
                rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration * delayIndex);
                rr.DOColor(fillSpriteColor, 0.6f).SetDelay(animationDuration * delayIndex);
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
                    var cell = cells[x, y];
                    var rr = cell.obj.GetComponent<SpriteRenderer>();
                    if (rr == null) continue;

                    // 跳过未变化的cell（只对isCurrent、isStart、isFilled、isBarrier有动画）
                    bool needAnim = false;
                    if (cell.isBarrier && rr.sprite != barrierSprite) needAnim = true;
                    else if (cell.isCurrent && rr.sprite != currentSprite) needAnim = true;
                    else if (cell.isStart && rr.sprite != startSprite) needAnim = true;
                    else if (cell.isFilled && rr.sprite != fillSprite) needAnim = true;
                    else if (!cell.isBarrier && !cell.isCurrent && !cell.isStart && !cell.isFilled &&
                             rr.sprite != emptySprite) needAnim = true;
                    if (!needAnim) continue;

                    if (cell.isBarrier)
                    {
                        rr.sprite = barrierSprite;
                        rr.color = new Color(barrierSpriteColor.r, barrierSpriteColor.g, barrierSpriteColor.b, 0f);
                        rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration * delayIndex);
                        rr.DOColor(barrierSpriteColor, 0.6f).SetDelay(animationDuration * delayIndex);
                    }
                    else if (cell.isCurrent)
                    {
                        rr.sprite = currentSprite;
                        if (fillSpriteColor != Color.white)
                        {
                            rr.color = new Color(fillSpriteColor.r, fillSpriteColor.g, fillSpriteColor.b, 0f);
                            rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration * delayIndex);
                            rr.DOColor(fillSpriteColor, 0.6f).SetDelay(animationDuration * delayIndex);
                        }
                        else
                        {
                            rr.color = new Color(rr.color.r, rr.color.g, rr.color.b, 0f);
                            rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration * delayIndex);
                            rr.DOColor(Color.white, 0.6f).SetDelay(animationDuration * delayIndex);
                        }
                    }
                    else if (cell.isStart)
                    {
                        rr.sprite = startSprite;
                        if (fillSpriteColor != Color.white)
                        {
                            rr.color = new Color(fillSpriteColor.r, fillSpriteColor.g, fillSpriteColor.b, 0f);
                            rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration * delayIndex);
                            rr.DOColor(fillSpriteColor, 0.6f).SetDelay(animationDuration * delayIndex);
                        }
                        else
                        {
                            rr.color = new Color(rr.color.r, rr.color.g, rr.color.b, 0f);
                            rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration * delayIndex);
                            rr.DOColor(Color.white, 0.6f).SetDelay(animationDuration * delayIndex);
                        }
                    }
                    else if (cell.isFilled)
                    {
                        rr.sprite = fillSprite;
                        rr.color = new Color(fillSpriteColor.r, fillSpriteColor.g, fillSpriteColor.b, 0f);
                        rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration * delayIndex);
                        rr.DOColor(fillSpriteColor, 0.6f).SetDelay(animationDuration * delayIndex);
                    }
                    else
                    {
                        rr.sprite = emptySprite;
                        rr.color = new Color(rr.color.r, rr.color.g, rr.color.b, 0f);
                        rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration * delayIndex);
                        rr.DOColor(Color.white, 0.6f).SetDelay(animationDuration * delayIndex);
                    }
                }
            }

            // 更新可用方向提示
            if (isGameStarted && !isGameOver)
            {
                foreach (var rr in from dir in availableDirections
                         let x = currentPosition.x + dir.x
                         let y = currentPosition.y + dir.y
                         where IsWithinBounds(x, y)
                         select cells[x, y].obj.GetComponent<SpriteRenderer>()
                         into rr
                         where rr != null
                         select rr)
                {
                    lastDirectionRenderer.Add(rr);
                    rr.sprite = availableSprite;
                    // rr.color = new Color(availableSpriteColor.r, availableSpriteColor.g, availableSpriteColor.b, 0f);
                    rr.DOFade(1.0f, 0.6f).SetDelay(animationDuration * delayIndex);
                    rr.DOColor(availableSpriteColor, 0.6f).SetDelay(animationDuration * delayIndex);
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