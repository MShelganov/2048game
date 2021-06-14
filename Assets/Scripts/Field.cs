using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field fieldInstance;
    public static Field Instance { get => fieldInstance; }

    [Header("Настройки поля")]
    public float cellSize = 200;
    public float space = 30;
    public int fieldSize = 4;
    public int initCellCount = 2;

    [Space(10)]
    public Cell cellPrefab;
    private RectTransform rectTransform;

    private Cell[,] field;

    private bool anyCellsMoved = false;

    void Awake()
    {
        if (fieldInstance == null)
            fieldInstance = this;
        rectTransform = this.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (MouseManager.Instance.SwipeUp)
            OnInput(Vector2.up);
        if (MouseManager.Instance.SwipeDown)
            OnInput(Vector2.down);
        if (MouseManager.Instance.SwipeLeft)
            OnInput(Vector2.left);
        if (MouseManager.Instance.SwipeRight)
            OnInput(Vector2.right);
    }

    private void OnInput(Vector2 direction)
    {
        if (!GameController.GameStarted)
            return;

        anyCellsMoved = false;
        ResetCellsFlags();
        Move(direction);

        if (anyCellsMoved)
        {
            CreateRandomCell(initCellCount);
            CheckGameResult();
        }
    }

    private void Move(Vector2 direction)
    {
        int startXY = direction.x > 0 || direction.y < 0 ? fieldSize - 1 : 0;
        int dir = direction.x != 0 ? (int)direction.x : -(int)direction.y;

        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = startXY; j >= 0 && j < fieldSize; j -= dir)
            {
                Cell cell = direction.x != 0 ? field[j, i] : field[i, j];

                if (cell.IsEmpty)
                    continue;

                Cell cellToMerge = FindCellToMerge(cell, direction);

                if (cellToMerge != null)
                {
                    cell.MergeWithCell(cellToMerge);
                    anyCellsMoved = true;
                    continue;
                }

                Cell emptyCell = FindEmptyCell(cell, direction);

                if (emptyCell != null)
                {
                    cell.MoveToCell(emptyCell);
                    anyCellsMoved = true;
                }
            }
        }
    }

    private Cell FindCellToMerge(Cell cell, Vector2 direction)
    {
        int startX = cell.X + (int)direction.x;
        int startY = cell.Y - (int)direction.y;

        for (int x = startX, y = startY;
             x >= 0 && x < fieldSize && y >= 0 && y < fieldSize;
             x += (int)direction.x, y -= (int)direction.y)
        {
            if (field[x, y].IsEmpty)
                continue;
            if (field[x, y].Count == cell.Count && !field[x, y].HasMerged)
                return field[x, y];

            break;
        }
        return null;
    }

    private Cell FindEmptyCell(Cell cell, Vector2 direction)
    {
        Cell emptyCell = null;
        int startX = cell.X + (int)direction.x;
        int startY = cell.Y - (int)direction.y;

        for (int x = startX, y = startY;
             x >= 0 && x < fieldSize && y >= 0 && y < fieldSize;
             x += (int)direction.x, y -= (int)direction.y)
        {
            if (field[x, y].IsEmpty)
                emptyCell = field[x, y];
            else
                break;
        }
        return emptyCell;
    }

    private void CheckGameResult()
    {
        bool lose = true;

        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                if(field[i, j].Value == Cell.MaxCount)
                {
                    GameController.Instance.Win();
                    return;
                }

                if (lose && field[i, j].IsEmpty ||
                    FindCellToMerge(field[i, j], Vector2.left) != null ||
                    FindCellToMerge(field[i, j], Vector2.right) != null ||
                    FindCellToMerge(field[i, j], Vector2.up) != null ||
                    FindCellToMerge(field[i, j], Vector2.down) != null)
                {
                    lose = false;
                }
            }
        }

        if(lose)
        {
            GameController.Instance.Lose();
        }
    }

    private void CreateField()
    {
        field = new Cell[fieldSize, fieldSize];

        float fieldWidth = fieldSize * (cellSize + space) + space;
        rectTransform.sizeDelta = new Vector2(fieldWidth, fieldWidth);

        float startX = -(fieldWidth / 2) + (cellSize / 2) + space;
        float startY = (fieldWidth / 2) - (cellSize / 2) - space;

        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                Cell tempCell = Instantiate<Cell>(cellPrefab, this.transform, false);
                Vector2 tempPosition = new Vector2(startX + i * (cellSize + space), startY - j * (cellSize + space));
                tempCell.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                tempCell.transform.localPosition = tempPosition;

                field[i, j] = tempCell;

                tempCell.SetValue(0, i, j, cellSize);
            }
        }
    }

    public void GenerateField()
    {
        if (field == null)
            CreateField();

        for (int i = 0; i < fieldSize; i++)
            for (int j = 0; j < fieldSize; j++)
                field[i, j].SetValue(0);

        CreateRandomCell(initCellCount);
    }

    private void CreateRandomCell(int number)
    {
        List<Cell> emptyCells = new List<Cell>();
        for (int i = 0; i < fieldSize; i++)
            for (int j = 0; j < fieldSize; j++)
                if (field[i, j].IsEmpty)
                    emptyCells.Add(field[i, j]);

        if (emptyCells.Count == 0)
            Debug.LogWarning("<color=red>Lost</color>");

        for (int c = 0; c < number; c++)
        {
            int randomValue = Random.Range(0, 10) == 0 ? 2 : 1; // 2^2 || 2^1
            if (emptyCells.Count > 0)
            {
                int at = Random.Range(0, emptyCells.Count);
                Cell emptyCell = emptyCells[at];
                emptyCell.SetValue(randomValue);
                emptyCells.RemoveAt(at);
            }
        }
    }

    private void ResetCellsFlags()
    {
        for (int i = 0; i < fieldSize; i++)
            for (int j = 0; j < fieldSize; j++)
                field[i, j].ResetFlags();
    }
}
