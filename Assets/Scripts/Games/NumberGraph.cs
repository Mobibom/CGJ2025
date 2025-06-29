using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberGraph : MonoBehaviour
{
    private const int matrixSize = 7; // N*N 矩阵大小
    public GameObject cellPrefab; // 矩阵单元格预制体
    private GameObject[,] cells;
    private int[,] matrix;
    private int[][] columnConstraints;
    private int[][] rowConstraints;
    private bool isSolved;
    private Action onFinish;

    void Start()
    {
        matrix = new int[matrixSize, matrixSize];
        cells = new GameObject[matrixSize, matrixSize];

        columnConstraints = new int[matrixSize][];
        columnConstraints[0] = new int[] { 1, 1 };
        columnConstraints[1] = new int[] { 3, 2 };
        columnConstraints[2] = new int[] { 3, 1 };
        columnConstraints[3] = new int[] { 1 };
        columnConstraints[4] = new int[] { 7 };
        columnConstraints[5] = new int[] { 1, 1 };
        columnConstraints[6] = new int[] { 1, 1 };

        rowConstraints = new int[matrixSize][];
        rowConstraints[0] = new int[] { 2, 2 };
        rowConstraints[1] = new int[] { 2, 1 };
        rowConstraints[2] = new int[] { 3, 3 };
        rowConstraints[3] = new int[] { 1 };
        rowConstraints[4] = new int[] { 3 };
        rowConstraints[5] = new int[] { 1, 1 };
        rowConstraints[6] = new int[] { 2, 1, 1 };

        for (int i = 0; i < matrixSize; i++)
        {
            for (int j = 0; j < matrixSize; j++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(j + 2, -i + 2, 0), Quaternion.identity);
                BoxCollider boxCollider = cell.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(1, 1, 1);
                cell.transform.SetParent(transform);
                cell.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                cell.GetComponent<SpriteRenderer>().color = Color.white;
                cells[i, j] = cell;
            }
        }
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
        cellPrefab.transform.localPosition = new Vector3(10000f, 0.5f, 0);
    }

    void Update()
    {
        if (isSolved)
        {
            // 解题成功，显示结果
            // 实现
            Debug.Log("Solved!");
            isSolved = false;
            onFinish?.Invoke();
        }
    }

    private void Reset()
    {
        for (int i = 0; i < matrixSize; i++)
        {
            for (int j = 0; j < matrixSize; j++)
            {
                matrix[i, j] = 0;
                cells[i, j].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public void SetFinishedCallback(Action callback)
    {
        // 设置完成回调
        if (callback != null)
        {
            onFinish = callback;
        }
    }

    private void OnKeyDown(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Mouse0:
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject clickedObject = hit.collider.gameObject;
                    Tuple<int, int> cellPosition = GetCellPosition(clickedObject);
                    int row = cellPosition.Item1;
                    int col = cellPosition.Item2;
                    if (row >= 0 && col >= 0)
                    {
                        if (matrix[row, col] == 0)
                        {
                            matrix[row, col] = 1; // 填充
                            cells[row, col].GetComponent<SpriteRenderer>().color = Color.red; // 改变颜色
                        }
                        else
                        {
                            matrix[row, col] = 0; // 清空
                            cells[row, col].GetComponent<SpriteRenderer>().color = Color.white; // 恢复颜色
                        }
                        ValidateConstraints();
                    }
                }
                break;
        }
    }

    Tuple<int, int> GetCellPosition(GameObject clickedObject)
    {
        for (int i = 0; i < matrixSize; i++)
        {
            for (int j = 0; j < matrixSize; j++)
            {
                if (cells[i, j] == clickedObject)
                {
                    return new Tuple<int, int>(i, j);
                }
            }
        }
        // not going to happen
        Debug.LogError("Cell not found!");
        return new Tuple<int, int>(-1, -1);
    }

    private void ValidateConstraints()
    {
        for (int i = 0; i < matrixSize; i++)
        {
            int[] rowConstraint = rowConstraints[i];
            int[] columnConstraint = columnConstraints[i];

            // 行约束, i 代表行号
            int sum = 0;
            int rowConstraintIndex = 0;
            for (int j = 0; j < matrixSize; j++)
            {
                sum += matrix[i, j];
                if ((sum != 0 && matrix[i, j] == 0 )|| (j == matrixSize - 1 && sum != 0))
                {
                    if (rowConstraintIndex == rowConstraint.Length)
                    {
                        return;
                    }
                    if (sum != rowConstraint[rowConstraintIndex])
                    {
                        return;
                    }
                    else
                    {
                        sum = 0;
                        rowConstraintIndex++;
                    }
                }
            }
            if (rowConstraintIndex != rowConstraint.Length)
            {
                return;
            }
            Debug.Log(i +"行约束校验完成");

            // 列约束, i 代表列号
            int columnConstraintIndex = 0;
            for (int j = 0; j < matrixSize; j++)
            {
                sum += matrix[j, i];
                if ((sum != 0 && matrix[j, i] == 0) || (j == matrixSize - 1 && sum != 0))
                {
                    if (columnConstraintIndex == columnConstraint.Length)
                    {
                        return;
                    }
                    if (sum != columnConstraint[columnConstraintIndex])
                    {
                        return;
                    }
                    else
                    {
                        sum = 0;
                        columnConstraintIndex++;
                    }
                }
            }
            if (columnConstraintIndex != columnConstraint.Length)
            {
                return;
            }
            Debug.Log(i + "列约束校验完成");
            // 所有约束都满足，解题成功
        }
        isSolved = true;
    }
}