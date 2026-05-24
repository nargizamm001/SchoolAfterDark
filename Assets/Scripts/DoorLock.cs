using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [Header("Door To Unlock")]
    public Collider doorCollider;
    public GameObject doorObject;

    [Header("Lock Object")]
    public GameObject lockObject;

    private bool isUnlocked = false;

    void Start()
    {
        if (lockObject == null)
        {
            lockObject = gameObject;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isUnlocked) return;

        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null && GameManager.instance.hasKey)
            {
                UnlockDoor();
            }
            else
            {
                Debug.Log("Locked. You need a key.");
            }
        }
    }

    void UnlockDoor()
    {
        isUnlocked = true;

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        if (doorObject != null)
        {
            Renderer[] doorRenderers = doorObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in doorRenderers)
            {
                renderer.enabled = false;
            }
        }

        if (lockObject != null)
        {
            lockObject.SetActive(false);
        }

        Debug.Log("Door unlocked.");
    }
}