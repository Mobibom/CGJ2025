using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public struct HuaRongTile
{
    public int row;
    public int col;
    public bool isEmpty;
}

public class HuaRongPuzzleMgr : BaseManager<HuaRongPuzzleMgr>
{
    private int gridSize = 3;
    private float cellSize = 1f;
    private Color tileColor = Color.white;
    private Color emptyColor = Color.clear;
    private bool isAnimating = false;

    private GameObject huarongPanel;
    private GameObject[,] tiles; // 3x3拼图块物体
    private Vector2Int emptyPos; // 空格位置

    public void InitializeGame()
    {
        huarongPanel = new GameObject("HuaRongPanel");
        huarongPanel.transform.position = Vector2.zero;
        
        tiles = new GameObject[gridSize, gridSize];
        int number = 1;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                bool isLast = (x == gridSize - 1) && (y == gridSize - 1);
                
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
        emptyPos = new Vector2Int(gridSize - 1, gridSize - 1);
        
        // 打乱顺序
        ShuffleTiles();
        
        // TODO: 设置为演示时的华容道顺序
    }
    
    public void ShuffleTiles(int steps = 50)
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
    
    public void HandleMouseClick()
    {
        if (isAnimating) return;
        if (Input.GetMouseButtonDown(0))
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
    
    void TryMove(GameObject clickedTile)
    {
        // 查找该 tile 的坐标
        Vector2Int clickedPos = FindTilePosition(clickedTile);
        if (clickedPos.x == -1) return;

        // 判断是否与空格相邻
        if (IsAdjacent(clickedPos, emptyPos))
        {
            SwapTiles(clickedPos, emptyPos);
            emptyPos = clickedPos;

            if (CheckWin())
            {
                Debug.Log("🎉 恭喜完成拼图！");
            }
        }
    }
    
    Vector2Int FindTilePosition(GameObject tile)
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
    
    bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy == 1; // 只允许上下左右移动
    }
    
    void SwapTiles(Vector2Int a, Vector2Int b,  bool withAnimation = true)
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
    bool CheckWin()
    {
        int num = 1;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject tile = tiles[x, y];
                bool isLast = (x == gridSize - 1) && (y == gridSize - 1);
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

}
