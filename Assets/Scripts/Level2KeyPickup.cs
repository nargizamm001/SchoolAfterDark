using UnityEngine;

public class Level2KeyPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        bool isPlayer =
            other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player");

        if (!isPlayer)
        {
            return;
        }

        if (Level2Manager.instance != null)
        {
            Level2Manager.instance.CollectKey();
        }

        Destroy(gameObject);
    }
}