using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Games.HhuaRongPuzzle
{
    public class HuaRongPuzzle : MonoBehaviour
    {
        [Header("配置 Tiles")]
        [SerializeField] private Sprite[] tileSprites = new Sprite[9]; // 0-8 对应 1-9号图块（最后一个为空）
    
        [Header("自定义布局（从左上到右下，填写数字1-9）")]
        [SerializeField] private int[] customOrder = new int[]
        {
            2, 6, 9,
            4, 1, 5,
            8, 3, 7
        };
    
        [Header("是否使用自定义布局")]
        [SerializeField] private bool useCustomLayout = true;
    
        private readonly int gridSize = 3;
        private readonly float cellSize = 2f;
        private readonly Color tileColor = Color.white;
        private readonly Color emptyColor = Color.clear;
        private bool isAnimating;

        private GameObject huarongPanel;
        private GameObject[,] tiles; // 3x3拼图块物体
        private Vector2Int emptyPos; // 空格位置
        
        private bool isCreatedHuarongPuzzle = false;

        private Material unlitmat;

        public void Start()
        {
            EventCenter.GetInstance().AddEventListener<Vector2>("初始化华容道", InitializeGame);
            EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", HandleMouseClick);
            EventCenter.GetInstance().AddEventListener<Vector2>("重置华容道",ResetHuaRongPuzzle);
            EventCenter.GetInstance().AddEventListener("华容道通关", DestroyHuarongPanel);
        }
        private void InitializeGame(Vector2 originPoint)
        {
            isCreatedHuarongPuzzle = true;
            
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
                    tile.transform.localScale = Vector2.one * (cellSize * 0.99f);
                    tile.AddComponent<BoxCollider2D>();

                    SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                    unlitmat = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
                    if( unlitmat != null )
                        sr.material = unlitmat;
                    if (number <= tileSprites.Length && tileSprites[number - 1] != null)
                    {
                        sr.sprite = tileSprites[number - 1];
                    }
                    else
                    {
                        Debug.LogWarning($"Tile sprite {number} 未指定");
                    }
                    sr.color = isLast ? emptyColor : tileColor;

                    tiles[x, y] = tile;
                    number++;
                }
            }

            // 设置右下角为“空格”
            emptyPos = new Vector2Int(gridSize - 1, 0);

            if (useCustomLayout)
            {
                // 设置为演示时的华容道顺序
                SetCustomLayout();
            }
            else
            {
                // 随机打乱顺序
                ShuffleTiles();
            }
        }

        private void DestroyHuarongPanel()
        {
            Destroy(huarongPanel);
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
            if (customOrder.Length != gridSize * gridSize)
            {
                Debug.LogError("自定义顺序长度不合法！");
                return;
            }

            GameObject[,] newTiles = new GameObject[gridSize, gridSize];

            for (int index = 0; index < customOrder.Length; index++)
            {
                int tileNumber = customOrder[index];
                int row = index / gridSize;
                int col = index % gridSize;

                Vector2Int originalPos = FindTileByNumber(tileNumber);
                GameObject tile = tiles[originalPos.x, originalPos.y];

                int y = gridSize - 1 - row;
                tile.transform.localPosition = new Vector2(col * cellSize, y * cellSize);
                newTiles[col, y] = tile;

                if (tileNumber == 9)
                    emptyPos = new Vector2Int(col, y);
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
    
        private void HandleMouseClick(KeyCode keyCode)
        {
            if (isAnimating) return;
            if (!isCreatedHuarongPuzzle) return;
            if (keyCode == KeyCode.Mouse0)
            {
                if (Camera.main != null)
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
        
            EventCenter.GetInstance().EventTrigger("初始化华容道", originPoint);
        }
    
        private void HackWin()
        {
            Debug.Log("HackWin()");
            EventCenter.GetInstance().EventTrigger("华容道通关");
        }

    }
}
