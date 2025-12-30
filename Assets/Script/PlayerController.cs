using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string jumpParam = "Jump";
    [SerializeField] private string dieParam = "Die";

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private float moveAxis;
    private bool isJumping;
    private bool isDead;

    #region Unity
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        if (!groundCheck) groundCheck = transform.Find("GroundCheck");
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (isDead) { UpdateAnimation(); return; }
        ReadMoveInput();
        HandleJump();
        UpdateJumpState();
        UpdateAnimation();
        UpdateFlip();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        HandleMove();
    }
    #endregion

    #region Input
    private void ReadMoveInput()
    {
        moveAxis = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveAxis -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveAxis += 1f;
    }
    #endregion

    #region Move
    private void HandleMove()
    {
        rb.linearVelocity = new Vector2(moveAxis * moveSpeed, rb.linearVelocity.y);
    }
    #endregion

    #region Jump
    private void HandleJump()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        if (!IsGrounded()) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJumping = true;
        if (animator) animator.SetBool(jumpParam, true);
    }

    private void UpdateJumpState()
    {
        if (!isJumping) return;
        if (!IsGrounded()) return;

        isJumping = false;
        if (animator) animator.SetBool(jumpParam, false);
    }
    #endregion

    #region Ground
    private bool IsGrounded()
    {
        if (!groundCheck) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
    }
    #endregion

    #region Animation
    private void UpdateAnimation()
    {
        if (!animator) return;
        animator.SetFloat(speedParam, Mathf.Abs(rb.linearVelocity.x));
    }
    #endregion

    #region Visual
    private void UpdateFlip()
    {
        if (!sr) return;
        if (moveAxis > 0.01f) sr.flipX = false;
        if (moveAxis < -0.01f) sr.flipX = true;
    }
    #endregion

    #region Die
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        moveAxis = 0f;
        isJumping = false;
        if (rb) rb.linearVelocity = Vector2.zero;
        if (animator) { animator.SetBool(jumpParam, false); animator.SetBool(dieParam, true); }
    }

    public void Revive()
    {
        isDead    = false;
        moveAxis  = 0f;
        isJumping = false;

        if (rb) rb.linearVelocity = Vector2.zero;

        if (animator)
        {
            animator.SetBool(jumpParam, false);
            animator.SetBool(dieParam, false);
        }
    }

    #endregion
}