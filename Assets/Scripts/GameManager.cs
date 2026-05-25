using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private static int savedLives = 3;
    private static bool shouldUseSavedLives = false;

    [Header("Player State")]
    public int lives = 3;
    public bool hasKey = false;

    [Header("UI")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI keyText;
    public GameObject gameOverPanel;
    public GameObject youWinText;

    [Header("Scene Names")]
    public string nextLevelName = "Level2";
    public string mainMenuSceneName = "MainMenu";

    [Header("Win Settings")]
    public float winDelay = 2f;

    [Header("Player References")]
    public Transform player;
    public Transform playerSpawnPoint;
    public PlayerMovement playerMovement;

    [Header("Ghost References")]
    public Transform ghost;
    public Transform ghostSpawnPoint;

    private bool levelFinished = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (shouldUseSavedLives)
        {
            lives = savedLives;
        }
        else
        {
            lives = 3;
            savedLives = 3;
        }

        hasKey = false;
        levelFinished = false;

        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (youWinText != null)
        {
            youWinText.SetActive(false);
        }

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateUI();
    }

    public void PickUpKey()
    {
        hasKey = true;
        UpdateUI();

        Debug.Log("Key picked up!");
    }

    public void PlayerCaught()
    {
        if (levelFinished)
        {
            return;
        }

        lives--;
        savedLives = lives;

        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            RestartRoundWithRemainingLives();
        }
    }

    void RestartRoundWithRemainingLives()
    {
        shouldUseSavedLives = true;

        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    void GameOver()
    {
        levelFinished = true;
        shouldUseSavedLives = false;
        savedLives = 3;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.SaveHighScore();
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void WinLevel()
    {
        if (levelFinished)
        {
            return;
        }

        levelFinished = true;

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.SaveHighScore();
        }

        StartCoroutine(WinRoutine());
    }

    IEnumerator WinRoutine()
    {
        if (youWinText != null)
        {
            youWinText.SetActive(true);
        }

        yield return new WaitForSeconds(winDelay);

        shouldUseSavedLives = false;
        savedLives = 3;

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelName);
    }

    public void RestartGame()
    {
        shouldUseSavedLives = false;
        savedLives = 3;

        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void QuitToMainMenu()
    {
        shouldUseSavedLives = false;
        savedLives = 3;

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    void UpdateUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }

        if (keyText != null)
        {
            keyText.text = hasKey ? "Key: Yes" : "Key: No";
        }
    }
}