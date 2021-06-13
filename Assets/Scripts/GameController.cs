using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController gameController;

    public static GameController Instance { get => gameController; }
    public static int Points{ get; private set; }
    public static bool GameStarted { get; private set; }

    public TextMeshProUGUI gameResult;

    public TextMeshProUGUI pointsText;

    void Awake()
    {
        if (gameController == null)
            gameController = this;
    }

    // Update is called once per frame
    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        gameResult.text = string.Empty;
        SetPoints(0);
        GameStarted = true;
        Field.Instance.GenerateField();
    }

    public void Lose()
    {
        GameStarted = false;
        gameResult.text = "Ты проиграл!";
    }

    public void Win()
    {
        GameStarted = false;
        gameResult.text = "Ты выиграл!";
    }

    public void AddPoints(int points)
    {
        Points += points;
        pointsText.text = Points.ToString();
    }

    private void SetPoints(int points)
    {
        Points = points;
        pointsText.text = Points.ToString();
    }
}
