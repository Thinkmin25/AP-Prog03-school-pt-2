using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 playerInput = Vector2.zero;
    [SerializeField] bool lastDirLeft = false;

    public float apexHeight = 11f;
    public float apexTime = 1.0f;
    public float gravity;
    public float jumpVel;
    public float terminalFallSpeed = -0.7f;
    public float currentTime = 0;
    [SerializeField] Vector2 lastPos = Vector2.zero;
    [SerializeField] float startingJumpPos = 0;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] SpriteRenderer sr;
    public float coyoteTimer = 0;
    public float coyoteMax = 0.05f;

    public bool climbing = false;
    public bool canClimb = true;
    public float climbRange = 0.1f;
    public float climbSpeed = 4f;
    public float climbJump = 8f;
    public float climbTimer = 0;
    public float climbMax = 1.5f;
    public float climbCD = 1f;

    public bool grounded = false;

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
        
    }

    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        jumpVel = 2 * apexHeight / apexTime;
        if (climbCD > 0)
        {
            climbCD -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (climbRange == 0)
            {
                climbRange = 0.1f;
            }
            else climbRange = 0;
        }

        if (climbing)
        {
            if (lastDirLeft)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    climbTimer = climbMax;
                }
                else if (Input.GetKey(KeyCode.Space))
                {
                    climbCD = 0.5f;
                    playerInput = new Vector2(moveSpeed * 10, jumpVel);
                    coyoteTimer = coyoteMax;
                    climbing = false;
                }
            }
            else if (!lastDirLeft)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    climbTimer = climbMax;
                }
                else if (Input.GetKey(KeyCode.Space))
                {
                    climbCD = 0.5f;
                    playerInput = new Vector2(-moveSpeed * 10, jumpVel); //make the move speed a velocity thing
                    coyoteTimer = coyoteMax;
                    climbing = false;
                }
            }

            if (climbTimer < climbMax)
            {
                playerInput.y = climbSpeed;
                climbTimer += Time.deltaTime;
            }
            else
            {
                climbCD = 0.3f;
                canClimb = false;
                climbing = false;
            }

            if (ClimbCheck() == false)
            {
                climbCD = 0.3f;
                playerInput.y = climbJump;
                coyoteTimer = coyoteMax;
                climbing = false;
            }
        }
        else
        {
            MovementUpdate();
        }
    }

    private void FixedUpdate()
    {
        IsWalking();
        lastPos = rb.position;
        rb.position = (rb.position + playerInput * Time.fixedDeltaTime);
        playerInput.x = 0;
    }

    private void MovementUpdate()
    {
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
            canClimb = true;
            grounded = true;
            coyoteTimer = 0;
            climbTimer = 0;
        }
        else
        {
            grounded = false;
            coyoteTimer += Time.deltaTime;
        }

        if (coyoteTimer <= coyoteMax)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                playerInput.y += jumpVel;
                coyoteTimer = coyoteMax;
                climbCD = apexTime / 5;
            }
        }
        else
        {
            if (ClimbCheck())
            {
                climbing = true;
            }
            playerInput.y += Mathf.Clamp(gravity, terminalFallSpeed, 10);
        }
    }

    public bool IsWalking()
    {
        if (rb.position + playerInput != lastPos)
        {
            return true;
        }
        else return false;
    }

    public bool ClimbCheck()
    {
        if (climbCD > 0 || !canClimb)
        {
            return false;
        }

        float offset = 1;
        if (GetFacingDirection() == FacingDirection.left) {
            offset *= -1;
        }

        RaycastHit2D cast = Physics2D.BoxCast(rb.position + boxCollider.offset + new Vector2((boxCollider.size.x / 2 + climbRange) * offset, -boxCollider.size.y / 4), new Vector2(climbRange * 2, 2 * boxCollider.size.y / 5), 0, transform.up, 0);
        if (cast)
        {
            
            Debug.Log("wahjumpin");
            return true;
        }
        else return false;
    }

    public bool IsGrounded()
    {
        float offset = 1;
        if (GetFacingDirection() == FacingDirection.left)
        {
            offset *= -1;
        }
        RaycastHit2D cast = Physics2D.BoxCast(rb.position + boxCollider.offset - new Vector2(0, boxCollider.size.y / 2), new Vector2(boxCollider.size.x / 1.5f, 0.1f), 0, transform.up, terminalFallSpeed / 2);
        if (cast && playerInput.y <= 0)
        {
            //Debug.Log(cast.point);

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
