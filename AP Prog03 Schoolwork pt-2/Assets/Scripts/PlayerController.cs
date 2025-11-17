using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 playerInput = Vector2.zero;
    [SerializeField] bool lastDirLeft = false;

    [SerializeField] float apexHeight = 3;
    [SerializeField] float apexTime = 0.8f;
    [SerializeField] float gravity;
    [SerializeField] float jumpVel;
    [SerializeField] float terminalFallSpeed = -0.2f;
    [SerializeField] float currentTime = 0;
    [SerializeField] Vector2 lastPos = Vector2.zero;
    [SerializeField] float startingJumpPos = 0;
    [SerializeField] bool jumping = false;
    [SerializeField] BoxCollider2D boxCollider;

    public enum FacingDirection
    {
        left, right
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        lastPos = rb.position;
        gravity = -2 * apexHeight / Mathf.Pow(apexTime, 2);
        jumpVel = 4 * apexHeight / apexTime;
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
            if (Input.GetKey(KeyCode.Space))
            {
                playerInput.y += jumpVel;
                jumping = true;
            }
            else
            {
                jumping = false;
            }
        }
        else
        {
            playerInput.y += Mathf.Clamp(gravity, terminalFallSpeed, 10);
        }
        
        IsWalking();
        GetFacingDirection();
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
    public bool IsGrounded()
    {
        RaycastHit2D cast = Physics2D.Raycast(rb.position, -transform.up, playerInput.y - gravity);
        if (cast && playerInput.y <= 0)
        {
            Debug.Log(cast.distance + " " + cast.rigidbody);

            if (playerInput.y != 0)
            {
                rb.position -= new Vector2(0, cast.distance - boxCollider.size.y / 2);
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
