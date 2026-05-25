using UnityEngine;
using UnityEngine.AI;

public class SlowGhostAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float stoppingDistance = 1.1f;

    [Header("Catch")]
    public float catchCooldown = 1.5f;

    private NavMeshAgent agent;
    private bool canCatch = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.acceleration = 8f;
            agent.angularSpeed = 180f;
        }
    }

    void Update()
    {
        if (agent == null || player == null)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(player.position);
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

        if (!canCatch)
        {
            return;
        }

        canCatch = false;

        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerCaught();
        }

        Invoke(nameof(ResetCatch), catchCooldown);
    }

    void ResetCatch()
    {
        canCatch = true;
    }
}