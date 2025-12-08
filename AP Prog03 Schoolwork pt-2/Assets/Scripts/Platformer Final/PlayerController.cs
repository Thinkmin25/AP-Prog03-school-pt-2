using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 7f;
    public Rigidbody2D rb;
    public Vector2 playerInput = Vector2.zero;
    public bool lastDirLeft = false;
    int directionCoefficient = 1;

    public float apexHeight = 11f;
    public float apexTime = 1.0f;
    public float gravity;
    public float jumpVel;
    public float terminalFallSpeed = -0.7f;
    public float currentTime = 0;
    public Vector2 lastPos = Vector2.zero;
    public float startingJumpPos = 0;
    public BoxCollider2D boxCollider;
    public float coyoteTimer = 0;
    public float coyoteMax = 0.05f;

    public bool climbing = false;
    public bool canClimb = true;
    public float climbRange = 0.8f;
    public float climbSpeed = 9f;
    public float climbJump = 18f;
    public float climbTimer = 0;
    public float climbMax = 1.5f;
    public float climbCD = 1f;

    public bool dashing = false;
    public Vector2 dashTarget = Vector2.zero;
    public float dashTimer = 0;
    public float dashTimeMax = 0.25f;
    public float dashSpeed = 25f;
    public float postDashTimer = 0;
    public float postDashBonusSpeed = 1f;

    public bool grounded = false;

    public GameObject chaosSpherePrefab;
    public GameObject chaosSphereRef;

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
        lastPos = rb.position;
    }

    void Update()
    {
        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        jumpVel = 2 * apexHeight / apexTime;
        if (climbCD > 0)
        {
            climbCD -= Time.deltaTime;
        }

        // Movement state of the player
        if (dashing)
        {
            Dash();
        }
        else if (climbing)
        {
            WallClimb();
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
        // Brings th eplayer back up if they jump off of the screen
        if (rb.position.y < -10)
        {
            rb.position += new Vector2(0, 20);
        }

        // Spawns the Chaos Sphere when right click is pressed
        if (Input.GetMouseButtonDown(1))
        {
            if (chaosSpherePrefab != null)
            {
                if (chaosSphereRef != null)
                {
                    Destroy(chaosSphereRef);
                }

                chaosSphereRef = Instantiate(chaosSpherePrefab, rb.position + new Vector2(boxCollider.size.x, 0), Quaternion.identity, transform);
                chaosSphereRef.GetComponent<ChaosSphere>().startingDirection = directionCoefficient;
                Physics2D.IgnoreCollision(boxCollider, chaosSphereRef.GetComponent<CircleCollider2D>(), true);
            }
        }

        // Determines if hte player can dash towards the Chaos Sphere, taking distance and diagonal positioning into account
        if (chaosSphereRef)
        {
            if (Vector2.Distance(rb.position, chaosSphereRef.transform.position) < 10)
            {
                if ((chaosSphereRef.transform.position.x) * directionCoefficient > (rb.position.x + boxCollider.offset.x * directionCoefficient + (boxCollider.size.x / 2)) * directionCoefficient)
                {
                    if (chaosSphereRef.transform.position.y > rb.position.y + boxCollider.offset.y + (boxCollider.size.y / 2) || chaosSphereRef.transform.position.y < rb.position.y + boxCollider.offset.y - (boxCollider.size.y / 2))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            dashing = true;
                            dashTimer = dashTimeMax;
                            dashTarget = ((Vector2)chaosSphereRef.transform.position - rb.position).normalized;
                        }
                    }
                    
                }
            }
        }

        // Gives the player a speed boost after a dash
        if (postDashTimer > 0)
        {
            postDashTimer -= Time.deltaTime;
            postDashBonusSpeed = 2;
        }
        else postDashBonusSpeed = 0;

        // Basic left and right movement
        if (Input.GetKey(KeyCode.A))
        {
            playerInput.x -= moveSpeed + postDashBonusSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerInput.x += moveSpeed + postDashBonusSpeed;
        }

        // Grounded behaviour
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

        // Coyote time behaviour
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
            // Wall climb check
            if (ClimbCheck())
            {
                climbing = true;
            }

            // Gravity
            playerInput.y += Mathf.Clamp(gravity, terminalFallSpeed, 10);
        }
    }

    private void WallClimb()
    {
        // Code to let the player dismount the wall early
        if (lastDirLeft)
        {
            if (Input.GetKey(KeyCode.D))
            {
                climbTimer = climbMax;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                climbCD = 0.5f;
                playerInput = new Vector2(moveSpeed, jumpVel);
                coyoteTimer = coyoteMax;
                climbing = false;
            }
        }
        // Has different criteria and effects when done facing right
        else if (!lastDirLeft)
        {
            if (Input.GetKey(KeyCode.A))
            {
                climbTimer = climbMax;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                climbCD = 0.5f;
                playerInput = new Vector2(-moveSpeed, jumpVel);
                coyoteTimer = coyoteMax;
                climbing = false;
            }
        }

        // Automatically dimounts players from the wall after enough time has elapsed
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

        // Player stops climbing if they've hit the top of the wall
        if (ClimbCheck() == false)
        {
            climbCD = 0.3f;
            playerInput.y = climbJump;
            coyoteTimer = coyoteMax;
            climbing = false;
        }
    }

    private void Dash()
    {
        // Locks the player into dahsing for a short amount of time
        dashTimer -= Time.deltaTime;
        if (dashTimer < 0)
        {
            postDashTimer = 2;
            dashing = false;
        }

        // propels them towards a target around the Chaos Sphere
        playerInput = dashTarget * dashSpeed;
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

        // BoxCasts infront of the player's legs to detect if they're able to start wall climbing
        RaycastHit2D cast = Physics2D.BoxCast(rb.position + new Vector2(boxCollider.offset.x * directionCoefficient, boxCollider.offset.y + -boxCollider.size.y / 4), new Vector2(0.1f, 2 * boxCollider.size.y / 5), 0, transform.right * directionCoefficient, climbRange);
        if (cast)
        {
            return true;
        }
        else return false;
    }

    public bool IsGrounded()
    {
        // BoxCasts under the player to stick them to the floor if they're about to fall onto it
        RaycastHit2D cast = Physics2D.BoxCast(rb.position + boxCollider.offset - new Vector2(0, boxCollider.size.y / 2), new Vector2(boxCollider.size.x / 1.5f, 0.1f), 0, transform.up, terminalFallSpeed / 2);
        if (cast && playerInput.y <= 0)
        {
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
                directionCoefficient = 1;
                return FacingDirection.right;
            }
            else
            {
                lastDirLeft = true;
                directionCoefficient = -1;
                return FacingDirection.left;
            }
        }
        // The last direction the player was facing; useful when they stop moving and have no velocity to check
        else if (lastDirLeft)
        {
            return FacingDirection.left;
        }
        else return FacingDirection.right;
    }
}
