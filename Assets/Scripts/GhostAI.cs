using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float patrolPointReachDistance = 0.7f;

    [Header("Vision")]
    public float viewDistance = 15f;
    public float viewAngle = 140f;
    public float closeDetectionDistance = 4f;
    public LayerMask visionLayers = ~0;

    [Header("Chase")]
    public float chaseSpeed = 7.5f;
    public float loseSightDelay = 2f;
    public float chaseStopDistance = 1f;

    [Header("Catch")]
    public float catchCooldown = 1.5f;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private float lostSightTimer = 0f;
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
            agent.speed = patrolSpeed;
            agent.stoppingDistance = 0.2f;
        }

        GoToCurrentPatrolPoint();
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

        bool seesPlayer = CanSeePlayer();

        if (seesPlayer)
        {
            StartChasing();
            return;
        }

        if (isChasing)
        {
            lostSightTimer += Time.deltaTime;

            if (lostSightTimer < loseSightDelay)
            {
                ChasePlayer();
            }
            else
            {
                StopChasingAndReturnToPatrol();
            }

            return;
        }

        Patrol();
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= patrolPointReachDistance)
        {
            currentPatrolIndex++;

            if (currentPatrolIndex >= patrolPoints.Length)
            {
                currentPatrolIndex = 0;
            }

            GoToCurrentPatrolPoint();
        }
    }

    void GoToCurrentPatrolPoint()
    {
        if (agent == null)
        {
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        if (patrolPoints[currentPatrolIndex] == null)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0.2f;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    bool CanSeePlayer()
    {
        Vector3 eyePosition = transform.position + Vector3.up * 1.4f;
        Vector3 playerPosition = player.position + Vector3.up * 1.2f;

        Vector3 directionToPlayer = playerPosition - eyePosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewDistance)
        {
            return false;
        }

        Vector3 flatDirection = new Vector3(directionToPlayer.x, 0f, directionToPlayer.z);

        if (flatDirection.sqrMagnitude < 0.01f)
        {
            return true;
        }

        float angleToPlayer = Vector3.Angle(transform.forward, flatDirection.normalized);

        bool playerIsClose = distanceToPlayer <= closeDetectionDistance;
        bool playerIsInViewAngle = angleToPlayer <= viewAngle * 0.5f;

        if (!playerIsClose && !playerIsInViewAngle)
        {
            return false;
        }

        RaycastHit[] hits = Physics.RaycastAll(
            eyePosition,
            directionToPlayer.normalized,
            distanceToPlayer,
            visionLayers,
            QueryTriggerInteraction.Ignore
        );

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            bool hitPlayer =
                hit.collider.CompareTag("Player") ||
                hit.collider.transform.root.CompareTag("Player");

            if (hitPlayer)
            {
                return true;
            }

            return false;
        }

        return true;
    }

    void StartChasing()
    {
        isChasing = true;
        lostSightTimer = 0f;
        ChasePlayer();
    }

    void ChasePlayer()
    {
        if (agent == null || player == null)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        agent.speed = chaseSpeed;
        agent.stoppingDistance = chaseStopDistance;
        agent.SetDestination(player.position);
    }

    void StopChasingAndReturnToPatrol()
    {
        isChasing = false;
        lostSightTimer = 0f;
        GoToCurrentPatrolPoint();
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