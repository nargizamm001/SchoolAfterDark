using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2Manager : MonoBehaviour
{
    public static Level2Manager instance;

    [Header("Keys")]
    public int totalKeys = 3;
    public int collectedKeys = 0;

    [Header("UI")]
    public TextMeshProUGUI keysText;
    public GameObject finalWinText;

    [Header("Final")]
    public string sceneAfterWin = "MainMenu";
    public float finalDelay = 3f;

    private bool levelFinished = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        collectedKeys = 0;
        levelFinished = false;

        if (finalWinText != null)
        {
            finalWinText.SetActive(false);
        }

        UpdateKeysUI();
    }

    public void CollectKey()
    {
        if (levelFinished)
        {
            return;
        }

        collectedKeys++;

        if (collectedKeys > totalKeys)
        {
            collectedKeys = totalKeys;
        }

        UpdateKeysUI();

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(100);
        }

        Debug.Log("Level 2 key collected: " + collectedKeys + "/" + totalKeys);
    }

    public bool HasAllKeys()
    {
        return collectedKeys >= totalKeys;
    }

    public void FinishLevel()
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

        StartCoroutine(FinalRoutine());
    }

    IEnumerator FinalRoutine()
    {
        if (finalWinText != null)
        {
            finalWinText.SetActive(true);
        }

        yield return new WaitForSeconds(finalDelay);

        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(sceneAfterWin))
        {
            SceneManager.LoadScene(sceneAfterWin);
        }
    }

    void UpdateKeysUI()
    {
        if (keysText != null)
        {
            keysText.text = "Keys: " + collectedKeys + "/" + totalKeys;
        }
    }
}