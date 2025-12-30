using UnityEngine;

public class EnemyPatrolChase : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Move")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [Header("Detect")]
    [SerializeField] private float detectRange = 6f;
    [SerializeField] private float stopDistance = 1.2f;
    [SerializeField] private string playerTag = "Player";

    [Header("Flip")]
    [SerializeField] private bool faceRightByDefault = true;

    [Header("Ground Snap")]
    [SerializeField] private bool useGroundSnap = true;
    [SerializeField] private Transform groundRayOrigin;
    [SerializeField] private float groundRayLength = 3f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundOffsetY = 0.0f;
    [SerializeField] private float snapSpeed = 30f;

    private Rigidbody2D rb;
    private Transform targetPoint;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPoint = pointB;
        if (!groundRayOrigin) groundRayOrigin = transform;
    }

    private void FixedUpdate()
    {
        FindPlayer();
        if (player) Chase();
        else Patrol();
        if (useGroundSnap) SnapToGround();
    }

    private void FindPlayer()
    {
        if (player && Vector2.Distance(transform.position, player.position) <= detectRange) return;
        var go = GameObject.FindGameObjectWithTag(playerTag);
        player = go ? go.transform : null;
        if (player && Vector2.Distance(transform.position, player.position) > detectRange) player = null;
    }

    private void Patrol()
    {
        if (!pointA || !pointB) { rb.linearVelocity = Vector2.zero; return; }

        Vector2 pos    = rb.position;
        Vector2 target = targetPoint.position;

        if (Mathf.Abs(target.x - pos.x) < 0.1f)
            targetPoint = (targetPoint == pointA) ? pointB : pointA;

        float dirX = Mathf.Sign(target.x - pos.x);
        rb.linearVelocity = new Vector2(dirX * patrolSpeed, rb.linearVelocity.y);
        Flip(dirX);
    }


    private void Chase()
    {
        Vector2 pos = rb.position;
        Vector2 target = player.position;

        float dist = Vector2.Distance(pos, target);
        if (dist <= stopDistance) { rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); return; }
        if (dist > detectRange) { player = null; return; }

        Vector2 dir = (target - pos).normalized;
        rb.linearVelocity = new Vector2(dir.x * chaseSpeed, rb.linearVelocity.y);
        Flip(dir.x);
    }

    private void Flip(float x)
    {
        if (Mathf.Abs(x) < 0.01f) return;

        float sx = Mathf.Abs(transform.localScale.x);
        bool goingRight = x > 0f;

        bool shouldBePositive = faceRightByDefault ? !goingRight : goingRight;
        transform.localScale = new Vector3(shouldBePositive ? sx : -sx, transform.localScale.y, transform.localScale.z);
    }

    private void SnapToGround()
    {
        Vector2 origin = groundRayOrigin.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundRayLength, groundMask);
        if (!hit.collider) return;

        float targetY = hit.point.y + groundOffsetY;
        float newY = Mathf.Lerp(transform.position.y, targetY, snapSpeed * Time.fixedDeltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        if (pointA && pointB) { Gizmos.color = Color.cyan; Gizmos.DrawLine(pointA.position, pointB.position); }

        if (groundRayOrigin)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundRayOrigin.position, groundRayOrigin.position + Vector3.down * groundRayLength);
        }
    }
}