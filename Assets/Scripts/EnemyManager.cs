using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject artifact;
    private Transform cameraTransform;
    public Transform target;
    public float moveSpeed;
    public float enemyHealth;
    public float cameraMoveRadius = 12.2f;

    public LayerMask obstacleMask;               // layers considered as blocking obstacles
    public float maxDetourDistance = 4;          // how far to sample detour points from enemy
    public float avoidSampleAngleStep = 20;      // degrees between detour samples
    public float arrivalThreshold = 0.1f;        // when detour reached, consider arrived

    private bool isDead;
    public bool finalboss = false;
    private bool hasDetour;
    private Vector2 detourTarget;

    private Coroutine knockbackCoroutine;

    private Collider2D cachedCollider;
    private const float knockbackBackoff = 0.02f; // small distance to keep from wall

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        cameraTransform = Camera.main.transform ?? GameObject.FindWithTag("MainCamera")?.transform;
        isDead = false;
        hasDetour = false;

        // cache collider for safe spacing
        cachedCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        // Only attempt movement when within camera radius
        float sqrDistanceToCamera = (transform.position - cameraTransform.position).sqrMagnitude;
        if (sqrDistanceToCamera > cameraMoveRadius * cameraMoveRadius)
            return;

        ObstacleAvoidance();
    }

    private void ObstacleAvoidance()
    {
        Vector2 current = transform.position;
        Vector2 goal = target.position;

        // If direct path is clear, clear detour and head straight to player
        if (!IsObstructed(current, goal))
        {
            hasDetour = false;
            MoveTowards(goal);
            return;
        }

        // If we already have a detour but it's now obstructed or reached, try to refresh it
        if (!hasDetour || IsObstructed(current, detourTarget) || Vector2.Distance(current, detourTarget) <= arrivalThreshold)
        {
            if (FindDetourPoint(current, goal, out Vector2 found))
            {
                detourTarget = found;
                hasDetour = true;
            }
            else
            {
                // No detour found: stop
                hasDetour = false;
                return;
            }
        }

        // Move toward detour target
        MoveTowards(detourTarget);
    }

    private void MoveTowards(Vector2 destination)
    {
        Vector2 newPos = Vector2.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    // Returns true if a straight line between a and b is blocked by an obstacle
    private bool IsObstructed(Vector2 a, Vector2 b)
    {
        Vector2 dir = b - a;
        float dist = dir.magnitude;
        if (dist <= 0.0001f) return false;
        dir.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(a, dir, dist, obstacleMask);
        return hit.collider != null;
    }

    // Samples directions around the line to the goal to find a reachable detour point
    private bool FindDetourPoint(Vector2 from, Vector2 to, out Vector2 detour)
    {
        Vector2 dirToTarget = (to - from).normalized;

        // Start searching by increasing angle from the direct line
        for (float angle = avoidSampleAngleStep; angle <= 180f; angle += avoidSampleAngleStep)
        {
            for (int sign = 1; sign >= -1; sign -= 2)
            {
                float testAngle = angle * sign;
                Vector2 sampleDir = Rotate(dirToTarget, testAngle);
                Vector2 samplePoint = from + sampleDir * maxDetourDistance;

                bool toSampleClear = !Physics2D.Raycast(from, sampleDir, maxDetourDistance, obstacleMask);
                bool sampleToGoalClear = !IsObstructed(samplePoint, to);

                if (toSampleClear && sampleToGoalClear)
                {
                    detour = samplePoint;
                    return true;
                }
            }
        }

        detour = default;
        return false;
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, cameraMoveRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            if (hasDetour)
            {
                Gizmos.DrawSphere(detourTarget, 0.1f);
                Gizmos.DrawLine(transform.position, detourTarget);
            }

            if (target != null)
            {
                Gizmos.color = IsObstructed(transform.position, (Vector2)target.position) ? Color.red : Color.green;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
    
    //knockback stuff
    public void ApplyKnockback(Vector2 direction, float distance, float duration)
    {
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
        }
        knockbackCoroutine = StartCoroutine(KnockbackCoroutine(direction.normalized, distance, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction, float distance, float duration)
    {
        Vector2 start = transform.position;

        // Raycast ahead along the knockback path to see if an obstacle is between start and desired target.
        RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, obstacleMask);

        Vector2 targetPos;
        if (hit.collider != null)
        {
            // stop just before the obstacle
            float backoff = knockbackBackoff;
            // If we have a collider, use its smallest extent to increase safety slightly
            if (cachedCollider != null)
            {
                float minExtent = Mathf.Min(cachedCollider.bounds.extents.x, cachedCollider.bounds.extents.y);
                backoff = Mathf.Max(backoff, minExtent * 0.1f);
            }

            targetPos = hit.point - direction * backoff;
        }
        else
        {
            targetPos = start + direction * distance;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float frac = Mathf.Clamp01(t / duration);
            transform.position = Vector2.Lerp(start, targetPos, frac);
            yield return null;
        }
        transform.position = targetPos;
        knockbackCoroutine = null;
    }

    public void lowerEnemyHealth(int amount)
    {
            enemyHealth -= amount;

            if (enemyHealth <= 0)
            {
            isDead = true;
            gameObject.SetActive(false);
            if (this.gameObject.name == "Boss_Enemy" && isDead) {
                GameObject spawnedArtifact = Instantiate(artifact);
                spawnedArtifact.transform.position = this.transform.position;
                GameObject bossRoomPresence = GameObject.FindGameObjectWithTag("BossRoom");
                if (bossRoomPresence != null) {
                    Destroy(bossRoomPresence);
                }
            }
            }
    }
}
