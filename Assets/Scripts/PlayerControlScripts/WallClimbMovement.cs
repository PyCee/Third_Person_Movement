using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WallClimbMovement : MovementState
{
    [Header("Wall Climb")]
    [Tooltip("If Wall Climb Is Enabled")]
    public bool wallClimbEnabled;
    [Tooltip("Duration Of Wall Climb"), Min(0.0f)]
    public float wallClimbDuration;
    [Tooltip("Speed Of Wall Climb"), Min(0.0f)]
    public float wallClimbSpeed;
    [Tooltip("Speed Of Wall Slide After Climb Has Ended"), Min(0.0f)]
    public float wallSlideSpeed;
    [Tooltip("???"), Min(0.0f)]
    public float wallClimbVaultRayOffset;
    private Vector3 wallClimbNormal;
    private float wallClimbProgress;

    [Header("Wall Jump")]
    [Tooltip("If Wall Jump Is Enabled")]
    public bool wallJumpEnabled;
    [Tooltip("Angle Of Wall Jump"), Min(0.0f)]
    public float wallJumpAngle;
    [Tooltip("Speed Of Wall Jump"), Min(0.0f)]
    public float wallJumpSpeed;

    public void SetWallClimbNormal(Vector3 normal){
        wallClimbNormal = normal;
    }
    public Vector3 GetWallClimbNormal(){
        return wallClimbNormal;
    }
    public float GetWallClimbSpeed(){
        return wallClimbSpeed;
    }

    public WallClimbMovement()
        : base(MovementStateName.WallClimb) {
    }

    private VaultMovement vaultMovement;

    void Start(){
        Init();
        vaultMovement = GetComponent<VaultMovement>();
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        Vector3 climbMovement;
        if(wallClimbEnabled && wallClimbProgress <= wallClimbDuration){
            // Climb wall
            wallClimbProgress += Time.deltaTime;
            climbMovement = new Vector3(0.0f, 1.0f, 0.0f);
            climbMovement *= wallClimbSpeed;
        } else {
            // Slide down Wall
            climbMovement = new Vector3(0.0f, -1.0f, 0.0f);
            climbMovement *= wallSlideSpeed;
        }
        controlledMovement = climbMovement;

        if(wallJumpEnabled && Input.GetButtonDown("Jump")){
            Vector3 cross = Vector3.Cross(wallClimbNormal, Vector3.up);
            Quaternion qua = Quaternion.AngleAxis(wallJumpAngle, cross);
            Vector3 dir = qua * wallClimbNormal;
            controlledMovement = dir * wallJumpSpeed;
            // SetMidairMomentum(controlledMovement);
        }
        return controlledMovement;
    }
    override public MovementStateName GetState(){
        // TODO: consider using layer to filter out many collisions
        Vector3 rayOffset = new Vector3(0.0f, wallClimbVaultRayOffset, 0.0f);
        if(wallClimbEnabled && !Physics.Raycast(transform.position + rayOffset, wallClimbNormal * -1.0f, vaultMovement.vaultRayDistance))
            // if character has reached top of wall, vault over
            return MovementStateName.Vault;
        else if(cc.isGrounded)
            // If the character has reached the bottom of the wall
            return MovementStateName.Standard;
        else if(Input.GetButtonDown("Jump"))
            return MovementStateName.Midair;
        else
            return MovementStateName.WallClimb;
    }
    override public void SwitchTo(){
        material.SetColor("_Color", Color.green);
        wallClimbProgress = 0.0f;
    }
    override public void SwitchFrom(){
    }
}
