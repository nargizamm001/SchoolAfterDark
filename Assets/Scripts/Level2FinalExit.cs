using UnityEngine;

public class Level2FinalExit : MonoBehaviour
{
    private bool activated = false;

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

        if (Level2Manager.instance != null)
        {
            Level2Manager.instance.FinishLevel();
        }
    }
}