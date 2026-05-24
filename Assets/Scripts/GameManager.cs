using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    [Header("Win Settings")]
    public float winDelay = 2f;

    [Header("Respawn")]
    public Transform player;
    public Transform playerSpawnPoint;
    public Transform ghost;
    public Transform ghostSpawnPoint;

    private CharacterController playerController;
    private bool levelFinished = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        hasKey = false;
        levelFinished = false;

        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
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
        if (levelFinished) return;

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            RespawnRound();
        }
    }

    void RespawnRound()
    {
        if (player != null && playerSpawnPoint != null)
        {
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            player.position = playerSpawnPoint.position;
            player.rotation = playerSpawnPoint.rotation;

            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }

        if (ghost != null && ghostSpawnPoint != null)
        {
            ghost.position = ghostSpawnPoint.position;
            ghost.rotation = ghostSpawnPoint.rotation;
        }
    }

    void GameOver()
    {
        levelFinished = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void WinLevel()
    {
        if (levelFinished) return;

        levelFinished = true;
        StartCoroutine(WinRoutine());
    }

    IEnumerator WinRoutine()
    {
        if (youWinText != null)
        {
            youWinText.SetActive(true);
        }

        yield return new WaitForSeconds(winDelay);

        SceneManager.LoadScene(nextLevelName);
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