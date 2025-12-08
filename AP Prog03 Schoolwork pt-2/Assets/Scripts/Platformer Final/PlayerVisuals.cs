using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerController playerController;

    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int isClimbingHash = Animator.StringToHash("IsClimbing");
    private readonly int climbJumpHash = Animator.StringToHash("ClimbJump");
    private readonly int climbTopHash = Animator.StringToHash("ClimbTop");
    private readonly int climbDismountHash = Animator.StringToHash("ClimbDismount");
    private readonly int upDashHash = Animator.StringToHash("UpDash");
    private readonly int downDashHash = Animator.StringToHash("DownDash");

    void Update()
    {
        animator.SetBool(isWalkingHash, playerController.IsWalking());
        animator.SetBool(isGroundedHash, playerController.IsGrounded());
        animator.SetBool(isClimbingHash, playerController.climbing == true && playerController.climbCD <= 0.3);
        animator.SetBool(climbJumpHash, playerController.climbCD > 0.3f && playerController.canClimb == true);
        animator.SetBool(climbTopHash, playerController.climbCD <= 0.3f && playerController.canClimb == true);
        animator.SetBool(climbDismountHash, playerController.climbTimer >= playerController.climbMax && playerController.canClimb == false);
        animator.SetBool(upDashHash, playerController.dashing && playerController.playerInput.y > 0);
        animator.SetBool(downDashHash, playerController.dashing && playerController.playerInput.y <= 0);

        switch (playerController.GetFacingDirection())
        {
            case PlayerController.FacingDirection.left:
                bodyRenderer.flipX = true;
                break;
            case PlayerController.FacingDirection.right:
                bodyRenderer.flipX = false;
                break;
        }
    }
}
