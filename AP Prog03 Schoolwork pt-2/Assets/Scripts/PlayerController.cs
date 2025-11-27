using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 playerInput = Vector2.zero;
    [SerializeField] bool lastDirLeft = false;

    [SerializeField] float apexHeight = 5;
    [SerializeField] float apexTime = 0.8f;
    [SerializeField] float gravity;
    [SerializeField] float jumpVel;
    [SerializeField] float terminalFallSpeed = -0.9f;
    [SerializeField] float currentTime = 0;
    [SerializeField] Vector2 lastPos = Vector2.zero;
    [SerializeField] float startingJumpPos = 0;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float coyoteTimer = 0;
    [SerializeField] float coyoteMax = 0.1f;
    float wallJumpRange = 0.25f;

    public enum CharacterState
    {
        Idle, Walking, Jumping, Falling, Dead
    }

    public CharacterState state = CharacterState.Idle;

    public enum FacingDirection
    {
        left, right
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        lastPos = rb.position;
        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        jumpVel = 2 * apexHeight / apexTime;
    }

    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        playerInput.x = 0;

        if (Input.GetKey(KeyCode.A))
        {
            playerInput.x -= moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerInput.x += moveSpeed;
        }

        if (IsGrounded())
        {
            coyoteTimer = 0;
        }
        else
        {
            coyoteTimer += Time.deltaTime;
        }

        if (coyoteTimer <= coyoteMax)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                playerInput.y += jumpVel;
                coyoteTimer = coyoteMax;
            }
        }
        else
        {
            playerInput.y += Mathf.Clamp(gravity, terminalFallSpeed, 10);
        }
        


        IsWalking();
        MovementUpdate(playerInput);
        
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        lastPos = rb.position;
        rb.position = (rb.position + playerInput * Time.deltaTime);
    }

    public bool IsWalking()
    {
        if (rb.position + playerInput != lastPos)
        {
            return true;
        }
        else return false;
    }

    public bool canWallJump()
    {
        float offset = 1;

        if (GetFacingDirection() == FacingDirection.left) {
            offset *= -1;
        }
        return true;
        //RaycastHit2D cast = Physics2D.OverlapBox(rb.position + new Vector2((boxCollider.size.x + wallJumpRange) * offset, 0), new Vector2(wallJumpRange * 2, boxCollider.size.y), 0);
    }

    public bool IsGrounded()
    {
        RaycastHit2D cast = Physics2D.BoxCast(rb.position - new Vector2(0, boxCollider.size.y / 2), new Vector2(sr.bounds.size.x / 1.1f, 0.1f), 0, transform.up, terminalFallSpeed / 2);
        if (cast && playerInput.y <= 0)
        {
            Debug.Log(cast.point);

            if (playerInput.y != 0)
            {
                rb.position -= new Vector2(0, cast.distance);
            }

            playerInput.y = 0;
            return true;
        }
        else return false;
    }

    public FacingDirection GetFacingDirection()
    {
        if (playerInput.x != 0)
        {
            if (playerInput.x > 0)
            {
                lastDirLeft = false;
                return FacingDirection.right;
            }
            else
            {
                lastDirLeft = true;
                return FacingDirection.left;
            }
        }
        else if (lastDirLeft)
        {
            return FacingDirection.left;
        }
        else return FacingDirection.right;
    }
}
