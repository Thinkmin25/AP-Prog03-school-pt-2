using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerController playerController;

    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");

    void Update()
    {
        animator.SetBool(isWalkingHash, playerController.IsWalking());
        animator.SetBool(isGroundedHash, playerController.IsGrounded());

        switch (playerController.GetFacingDirection())
        {
            case PlayerController.FacingDirection.left:
                bodyRenderer.flipX = true;
                break;
            case PlayerController.FacingDirection.right:
                bodyRenderer.flipX = false;
                break;
        }

        switch (playerController.state)
        {
            case PlayerController.CharacterState.Idle:
                break;
            case PlayerController.CharacterState.Walking:
                break;
            case PlayerController.CharacterState.Jumping:
                break;
            case PlayerController.CharacterState.Falling:
                break;
            case PlayerController.CharacterState.Dead:
                //animator.Play()
                break;
        }
    }
}
