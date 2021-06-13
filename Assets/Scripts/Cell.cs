using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    /// <summary>
    /// Положение по X координате.
    /// </summary>
    public int X { get; private set; }

    /// <summary>
    /// Положение по Y координате.
    /// </summary>
    public int Y { get; private set; }

    /// <summary>
    /// Размер плитки на экране.
    /// </summary>
    public float Size { get; private set; }

    /// <summary>
    /// Номинал или 2 в степени Count.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Фактический результат.
    /// </summary>
    public int Value { get => IsEmpty ? 0 : (int)Mathf.Pow(2, Count); }

    /// <summary>
    /// Пустая ли ячейка.
    /// </summary>
    public bool IsEmpty { get => Count == 0; }

    /// <summary>
    /// Объединяться можно только единожды.
    /// </summary>
    public bool HasMerged { get; private set; }

    /// <summary>
    /// Выигрошное значение 2^11 или 2048 - Победа!
    /// </summary>
    public const int MaxCount = 11;

    /// <summary>
    /// Микроб который обозначает собой номинал.
    /// </summary>
    public Image image;

    /// <summary>
    /// Текст на плитке, номинал.
    /// </summary>
    public TextMeshProUGUI number;

    /// <summary>
    /// Увеличиваем номинал.
    /// </summary>
    public void IncreaseValue()
    {
        Count++;
        HasMerged = true;
        GameController.Instance.AddPoints(Value);
        // Обновляем ячейку.
        UpdateCell();
    }

    /// <summary>
    /// Положение плитки остается таким же. Поэтому меняем только номинал.
    /// </summary>
    /// <param name="count"> Число которое присвоется ячейке. </param>
    public void SetValue(int count)
    {
        Count = count;
        UpdateCell();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetValue(int count, int x, int y)
    {
        X = x;
        Y = y;
        Count = count;
        UpdateCell();
    }

    /// <summary>
    /// Началная инициализация яцейки. С указанием размера.
    /// </summary>
    /// <param name="count"> Число которое присвоется ячейке. </param>
    /// <param name="x"> Положение по X. </param>
    /// <param name="y"> Положение по Y. </param>
    /// <param name="size"> Размер ячейки.</param>
    public void SetValue(int count, int x, int y, float size)
    {
        X = x;
        Y = y;
        Size = size;
        Count = count;
        UpdateCell();
    }

    /// <summary>
    /// Для проверки столкновений ячеек.
    /// </summary>
    public void ResetFlags()
    {
        HasMerged = false;
    }

    /// <summary>
    /// Объединение двух плиток с одинаковым номиналом.
    /// </summary>
    /// <param name="someCell"></param>
    public void MergeWithCell(Cell someCell)
    {
        someCell.IncreaseValue();
        // Делаем плитку пустой.
        SetValue(0);
        UpdateCell();
    }

    /// <summary>
    /// Перемещение плитки в свободную ячейку.
    /// </summary>
    /// <param name="target"></param>
    public void MoveToCell(Cell target)
    {
        target.SetValue(Count, target.X, target.Y);
        SetValue(0);
        UpdateCell();
    }

    /// <summary>
    /// Обновляем вид плитки.
    /// </summary>
    public void UpdateCell()
    {
        if (IsEmpty)
        {
            image.sprite = SpriteManager.Instance.Empty;
            LeanTween.size(this.GetComponent<RectTransform>(), new Vector2(Size, Size), .6f).setEaseInOutBounce();
        }
        else
        {
            if (SpriteManager.Instance.Sprites.Count < Count)
                image.sprite = SpriteManager.Instance.Default;
            else
                image.sprite = SpriteManager.Instance.Sprites[(Count - 1)];
            LeanTween.size(image.GetComponent<RectTransform>(), new Vector2(Size, Size), .3f).setEaseInBounce();
        }
        if (number != null)
        {
            number.text = Value.ToString();
            number.rectTransform.sizeDelta = new Vector2(Size, Size);
            number.autoSizeTextContainer = true;
        }
    }
}
