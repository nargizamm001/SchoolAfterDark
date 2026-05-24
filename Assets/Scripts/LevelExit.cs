using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            if (GameManager.instance != null)
            {
                GameManager.instance.WinLevel();
            }
        }
    }
}