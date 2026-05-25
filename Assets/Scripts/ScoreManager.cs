using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("Score Settings")]
    public int score = 0;
    public int highScore = 0;
    public int scorePerSecond = 1;
    public int keyBonusScore = 100;
    public int scoreBarMaxValue = 300;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public Slider scoreBar;

    private float scoreTimer = 0f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (scoreBar != null)
        {
            scoreBar.minValue = 0;
            scoreBar.maxValue = scoreBarMaxValue;
            scoreBar.value = 0;
        }

        UpdateUI();
    }

    void Update()
    {
        scoreTimer += Time.deltaTime;

        if (scoreTimer >= 1f)
        {
            scoreTimer = 0f;
            AddScore(scorePerSecond);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    public void AddKeyBonus()
    {
        AddScore(keyBonusScore);
    }

    public void SaveHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    public void ResetScore()
    {
        score = 0;
        scoreTimer = 0f;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }

        if (scoreBar != null)
        {
            scoreBar.value = Mathf.Clamp(score, 0, scoreBarMaxValue);
        }
    }
}