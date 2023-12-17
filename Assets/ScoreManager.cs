using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int score = 0;

    public int Score => score;

    public void IncreaseScore()
    {
        score += 1;

        scoreText.text = "Score: " + score.ToString();
    }
}