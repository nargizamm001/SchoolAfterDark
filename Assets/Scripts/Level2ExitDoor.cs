using TMPro;
using UnityEngine;

public class Level2ExitDoor : MonoBehaviour
{
    [Header("Door")]
    public Collider doorCollider;
    public GameObject doorObject;
    public GameObject lockObject;

    [Header("Message UI")]
    public TextMeshProUGUI messageText;
    public string lockedMessage = "Find all 3 keys first.";

    private bool isUnlocked = false;

    void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        bool isPlayer =
            other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player");

        if (!isPlayer)
        {
            return;
        }

        if (isUnlocked)
        {
            return;
        }

        if (Level2Manager.instance != null && Level2Manager.instance.HasAllKeys())
        {
            UnlockDoor();
        }
        else
        {
            ShowLockedMessage();
        }
    }

    void UnlockDoor()
    {
        isUnlocked = true;

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        if (lockObject != null)
        {
            lockObject.SetActive(false);
        }

        Debug.Log("Level 2 exit unlocked.");
    }

    void ShowLockedMessage()
    {
        if (messageText == null)
        {
            Debug.Log(lockedMessage);
            return;
        }

        messageText.text = lockedMessage;
        messageText.gameObject.SetActive(true);

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 2f);
    }

    void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }
}