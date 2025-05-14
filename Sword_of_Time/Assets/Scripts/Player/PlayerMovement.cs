using System;
using Unity.VisualScripting;
using UnityEngine;

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
    public void Start()
    {
        defaultLayer = gameObject.layer;
    }
    public void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    public void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        //Handle Crouching Mechanic
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
        //Wind Passing Mechanic
        bool isUsingAbility = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKey(KeyCode.LeftShift) && !isInWindForm)
        {
            ActivateWindForm();
        }
        if (isInWindForm)
        {
            windFormTimer -= Time.deltaTime;
            if (windFormTimer <= 0)
            {
                DeactivateWindForm();
            }
        }
        //Flip face of character
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        animator.SetBool("Run", horizontalInput != 0);
        animator.SetBool("Grounded", isGrounded());
        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2);
        }
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
                extraJumpsCounter = extraJumps;
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
        {
            return;
        }
        SoundManager.instance.PlaySound(jumpSound);
        if (onWall())
        {
            WallJump();
        }
        else
        {
            if (isGrounded())
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            }
            else
            {
                if (coyoteCounter > 0)
                {
                    body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
                }
                else
                {
                    if (extraJumpsCounter > 0)
                    {
                        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
                        extraJumpsCounter--;
                    }
                }
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
    }
}
