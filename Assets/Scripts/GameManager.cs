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

    [Header("Scene Names")]
    public string nextLevelName = "Level2";

    [Header("Respawn")]
    public Transform player;
    public Transform playerSpawnPoint;
    public Transform ghost;
    public Transform ghostSpawnPoint;

    private CharacterController playerController;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        hasKey = false;

        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
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

    public void TryOpenDoor()
    {
        if (hasKey)
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.Log("Door is locked. Find the key first.");
        }
    }

    public void PlayerCaught()
    {
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
        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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