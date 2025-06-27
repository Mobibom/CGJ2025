using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HuaRongPuzzle : MonoBehaviour
{
    private int gridSize = 3;
    private float cellSize = 1f;
    private Color tileColor = Color.white;
    private Color emptyColor = Color.clear;
    private bool isAnimating = false;

    private GameObject huarongPanel;
    private GameObject[,] tiles; // 3x3拼图块物体
    private Vector2Int emptyPos; // 空格位置

    public void Start()
    {
        EventCenter.GetInstance().AddEventListener<Vector2>("初始化华容道", InitializeGame);
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", HandleMouseClick);
        EventCenter.GetInstance().AddEventListener<Vector2>("重置华容道",ResetHuaRongPuzzle);
        
        EventCenter.GetInstance().EventTrigger<Vector2>("初始化华容道", Vector2.zero);
    }
    private void InitializeGame(Vector2 originPoint)
    {
        huarongPanel = new GameObject("HuaRongPanel");
        huarongPanel.transform.position = originPoint;
        
        /*
         * 1 2 3
         * 4 5 6
         * 7 8 []
         */
        tiles = new GameObject[gridSize, gridSize];
        int number = 1;
        for (int y = gridSize - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridSize; x++)
            {
                bool isLast = (number == 9);
                
                GameObject tile = new GameObject(isLast ? "Empty" : "Tile " + number);
                tile.transform.SetParent(huarongPanel.transform);
                tile.transform.localPosition = new Vector2(x * cellSize, y * cellSize);
                tile.transform.localScale = Vector2.one * (cellSize * 0.9f);
                tile.AddComponent<BoxCollider2D>();

                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("HuaRongPuzzle/tile" + number);
                sr.color = isLast ? emptyColor : tileColor;

                tiles[x, y] = tile;
                number++;
            }
        }

        // 设置右下角为“空格”
        emptyPos = new Vector2Int(gridSize - 1, 0);
        
        // 打乱顺序
        // ShuffleTiles();
        
        // 设置为演示时的华容道顺序
        SetCustomLayout();
    }
    
    private void ShuffleTiles(int steps = 50)
    {
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        for (int i = 0; i < steps; i++)
        {
            List<Vector2Int> validMoves = new List<Vector2Int>();
            foreach (var dir in directions)
            {
                Vector2Int target = emptyPos + dir;
                if (target.x >= 0 && target.x < gridSize && target.y >= 0 && target.y < gridSize)
                {
                    validMoves.Add(target);
                }
            }

            if (validMoves.Count > 0)
            {
                Vector2Int move = validMoves[Random.Range(0, validMoves.Count)];
                SwapTiles(move, emptyPos, false);
                emptyPos = move;
            }
        }

        // 防止刚好被打乱成完成状态
        if (CheckWin())
        {
            ShuffleTiles(steps);
        }
    }

    private void SetCustomLayout()
    {
        int[,] customOrder = new int[3, 3]
        {
            { 2, 6, 9 },
            { 4, 1, 5 },
            { 8, 3, 7 }
        };

        GameObject[,] newTiles = new GameObject[gridSize, gridSize];

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                int tileNumber = customOrder[row, col];
                Vector2Int originalPos = FindTileByNumber(tileNumber);
                GameObject tile = tiles[originalPos.x, originalPos.y];

                tile.transform.localPosition = new Vector2(col * cellSize, (gridSize - 1 - row) * cellSize);
                newTiles[col, gridSize - 1 - row] = tile;

                if (tileNumber == 9)
                    emptyPos = new Vector2Int(col, gridSize - 1 - row);
            }
        }

        tiles = newTiles;
    }
    
    private Vector2Int FindTileByNumber(int tileNumber)
    {
        string targetName = tileNumber == 9 ? "Empty" : "Tile " + tileNumber;
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (tiles[x, y].name == targetName)
                    return new Vector2Int(x, y);
            }
        }
        return new Vector2Int(-1, -1); // 没找到
    }

    
    private Vector2Int FindInCustomOrder(int[,] arr, int target)
    {
        int rows = arr.GetLength(0);    // first dimension
        int cols = arr.GetLength(1);    // second dimension

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (arr[row, col] == target)
                    return new Vector2Int(col, row);
                // or new Vector2Int(row, col) depending on whether
                // you treat X as column and Y as row
            }
        }

        // not found
        return new Vector2Int(-1, -1);
    }

    private GameObject GetEmptyTile()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (tiles[x, y].name == "Empty")
                    return tiles[x, y];
            }
        }
        return null;
    }
    
    private void HandleMouseClick(KeyCode keyCode)
    {
        if (isAnimating) return;
        if (keyCode == KeyCode.Mouse0)
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null)
            {
                GameObject clicked = hit.collider.gameObject;
                TryMove(clicked);
            }
        }
    }
    
    private void TryMove(GameObject clickedTile)
    {
        // 查找该 tile 的坐标
        Vector2Int clickedPos = FindTilePosition(clickedTile);
        if (clickedPos.x == -1) return;

        // 判断是否与空格相邻
        if (IsAdjacent(clickedPos, emptyPos))
        {
            SwapTiles(clickedPos, emptyPos);
            emptyPos = clickedPos;
        }
    }
    
    private Vector2Int FindTilePosition(GameObject tile)
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (tiles[x, y] == tile)
                    return new Vector2Int(x, y);
            }
        }
        return new Vector2Int(-1, -1); // 没找到
    }
    
    private bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy == 1; // 只允许上下左右移动
    }
    
    private void SwapTiles(Vector2Int a, Vector2Int b,  bool withAnimation = true)
    {
        GameObject tileA = tiles[a.x, a.y];
        GameObject tileB = tiles[b.x, b.y];

        Vector3 posA = tileA.transform.localPosition;
        Vector3 posB = tileB.transform.localPosition;

        if (withAnimation)
        {
            if (isAnimating) return;
            isAnimating = true;
            
            float duration = 0.2f;
            
            // 同时开始两个移动动画
            Sequence seq = DOTween.Sequence();
            seq.Join(tileA.transform.DOLocalMove(posB, duration));
            seq.Join(tileB.transform.DOLocalMove(posA, duration));
            
            seq.OnComplete(() =>
            {
                isAnimating = false;
                // 交换引用必须放在动画结束后或之前都可以，这里放后面保证位置同步
                tiles[a.x, a.y] = tileB;
                tiles[b.x, b.y] = tileA;
                
                if (CheckWin())
                {
                    Debug.Log("🎉 华容道通关(拼图完成)");
                    EventCenter.GetInstance().EventTrigger("华容道通关");
                }
            });
        }
        else
        {
            tileA.transform.localPosition = posB;
            tileB.transform.localPosition = posA;

            // 交换数组
            tiles[a.x, a.y] = tileB;
            tiles[b.x, b.y] = tileA;
        }
    }
    private bool CheckWin()
    {
        int num = 1;
        for (int y = gridSize - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject tile = tiles[x, y];
                bool isLast = (num == 9);
                if (isLast)
                {
                    if (tile.name != "Empty")
                        return false;
                }
                else
                {
                    if (tile.name != "Tile " + num)
                        return false;
                }
                num++;
            }
        }
        return true;
    }

    private void ResetHuaRongPuzzle(Vector2 originPoint)
    {
        foreach (Transform child in huarongPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        EventCenter.GetInstance().EventTrigger<Vector2>("初始化华容道", originPoint);
    }
    
    private void HackWin()
    {
        Debug.Log("HackWin()");
        EventCenter.GetInstance().EventTrigger("华容道通关");
    }

}
