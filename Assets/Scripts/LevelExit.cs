using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [Header("Win UI")]
    public GameObject youWinText;

    [Header("Scene Settings")]
    public string nextLevelName = "Level2";
    public float delayBeforeNextLevel = 2f;

    private bool activated = false;

    void Start()
    {
        if (youWinText != null)
        {
            youWinText.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated)
        {
            return;
        }

        bool isPlayer =
            other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player");

        if (!isPlayer)
        {
            return;
        }

        activated = true;
        StartCoroutine(WinRoutine());
    }

    IEnumerator WinRoutine()
    {
        if (youWinText != null)
        {
            youWinText.SetActive(true);
        }
        else
        {
            Debug.LogWarning("YouWinText is not assigned in LevelExit.");
        }

        yield return new WaitForSeconds(delayBeforeNextLevel);

        SceneManager.LoadScene(nextLevelName);
    }
}