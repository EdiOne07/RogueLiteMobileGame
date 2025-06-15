using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic; // Added

public class Movement : MonoBehaviour
{
    private Rigidbody2D body;
    private int counter = 0;
    [SerializeField] private float speed;
    private Animator animator;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float jumpPower;
    private float wallJumpCooldown;
    private float horizontalInput;

    [Header("Sound")]
    [SerializeField] private AudioClip jumpSound;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int extraJumpsCounter;

    [Header("Wall Jumps")]
    [SerializeField] private float wallJumpX;
    [SerializeField] private float wallJumpY;

    [Header("Crouching")]
    [SerializeField] private Vector2 crouchingSize;
    [SerializeField] private Vector2 crouchingOffset;
    [SerializeField] private Vector2 originalBoxColliderSize;
    [SerializeField] private Vector2 originalBoxColliderOffset;

    [Header("Wind Form")]
    [SerializeField] private float windFormDuration;
    private bool isInWindForm = false;
    private float windFormTimer;
    private int defaultLayer;
    [SerializeField] private string windFormLayerName = "WindForm";
    private PlayerAbility player;

    private readonly List<Collider2D> ignoredColliders = new();

    public void Start()
    {
        defaultLayer = gameObject.layer;
    }

    public void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GetComponent<PlayerAbility>();
    }

    public void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // Handle Crouching
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        animator.SetBool("Crouched", isCrouching);
        if (isCrouching)
        {
            boxCollider.size = crouchingSize;
            boxCollider.offset = crouchingOffset;
        }
        else
        {
            boxCollider.size = originalBoxColliderSize;
            boxCollider.offset = originalBoxColliderOffset;
        }

        // Wind Form Activation
        if (Input.GetKey(KeyCode.LeftShift) && !isInWindForm && player.canDash)
        {
            ActivateWindForm();
        }

        // Wind Form Timer + Deactivation Logic (MODIFIED)
        if (isInWindForm)
        {
            windFormTimer -= Time.deltaTime;

            if (windFormTimer <= 0)
            {
                RemoveNullColliders(); // Clean up destroyed colliders
                if (!IsInsideIgnoredColliders())
                {
                    DeactivateWindForm();
                }
                else
                {
                    windFormTimer = 0.05f; // Keep checking until clear
                }
            }
        }

        // Flip
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        animator.SetBool("Run", horizontalInput != 0);
        animator.SetBool("Grounded", isGrounded());

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2);
        }

        // Wall Slide
        if (onWall())
        {
            body.gravityScale = 0;
            body.linearVelocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime;
                if (player.canDoubleJump)
                {
                    extraJumps = 1;
                    extraJumpsCounter = extraJumps;
                }
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }
        }
    }

    public void Jump()
    {
        if (coyoteCounter <= 0 && !onWall() && extraJumpsCounter <= 0)
            return;

        SoundManager.instance.PlaySound(jumpSound);

        if (onWall())
        {
            WallJump();
        }
        else
        {
            if (isGrounded() || coyoteCounter > 0)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            }
            else if (extraJumpsCounter > 0)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
                extraJumpsCounter--;
            }
            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
    }

    public Boolean isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    public Boolean onWall()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return hit.collider != null;
    }

    public Boolean canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }

    public void ActivateWindForm()
    {
        isInWindForm = true;
        windFormTimer = windFormDuration;
        gameObject.layer = LayerMask.NameToLayer(windFormLayerName);
        animator.SetBool("WindPassing", true);
    }

    public void DeactivateWindForm()
    {
        isInWindForm = false;
        animator.SetBool("WindPassing", false);
        gameObject.layer = defaultLayer;

        foreach (var col in ignoredColliders)
        {
            if (col != null)
                Physics2D.IgnoreCollision(boxCollider, col, false);
        }

        ignoredColliders.Clear();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInWindForm && IsIgnorableTag(collision.collider.tag))
        {
            Physics2D.IgnoreCollision(boxCollider, collision.collider, true);
            ignoredColliders.Add(collision.collider);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInWindForm && IsIgnorableTag(collision.tag))
        {
            Physics2D.IgnoreCollision(boxCollider, collision, true);
            ignoredColliders.Add(collision);
        }
    }

    private bool IsIgnorableTag(string tag)
    {
        return tag == "Enemy" || tag == "Trap" || tag == "Box" || tag == "Boss";
    }

    
    private bool IsInsideIgnoredColliders()
    {
        foreach (var col in ignoredColliders)
        {
            if (col != null && boxCollider.bounds.Intersects(col.bounds))
                return true;
        }
        return false;
    }

   
    private void RemoveNullColliders()
    {
        ignoredColliders.RemoveAll(c => c == null);
    }
}
