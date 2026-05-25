using UnityEngine;

public class KeyPickup : MonoBehaviour
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

        if (GameManager.instance != null)
        {
            GameManager.instance.PickUpKey();
        }

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddKeyBonus();
        }

        Destroy(gameObject);
    }
}