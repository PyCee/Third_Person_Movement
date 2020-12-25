using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MidairMovement : MovementState
{
    [Header("Midair")]
    [Tooltip("Movement speed in midair"), Min(0.0f)]
    public float midairMoveSpeed;
    [Tooltip("Percentage Of Ground Speed Carried Over To Midair Movement"), Range(0.0f, 1.0f)]
    public float momentumMultiplier;
    private Vector3 midairMomentum;

    [Header("Double Jump")]
    [Tooltip("If Double Jump Is Enabled")]
    public bool enableDoubleJump;
    [Tooltip("Double Jump Velocity"), Min(0.0f)]
    public float doubleJumpSpeed;
    private bool doubleJumpReady = true;

    public MidairMovement()
        : base(MovementStateName.Midair) {
    }

    override protected void _Start(){
        
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.black);
        doubleJumpReady = true;

        // Must use movement variable to calculate momentum because character controller velocity is a frame behind
        midairMomentum = mc.GetMovement();
        midairMomentum.y = 0.0f;
        midairMomentum *= momentumMultiplier;
    }
    override protected void _SwitchFrom(){
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        controlledMovement *= midairMoveSpeed;
        controlledMovement = new Vector3(controlledMovement.x, vel.y, controlledMovement.z);
        controlledMovement += midairMomentum;

        Vector3 flatMovement = vel;
        flatMovement.y = 0;
        // TODO for when I do dive again
        // if(diveEnabled && Input.GetButtonDown("Dive") && flatMovement.sqrMagnitude > (minDiveSpeed * minDiveSpeed)){
        //     SetState(MovementStateE.Dive);
        // } else 
        if(Input.GetButtonDown("Jump") && enableDoubleJump && doubleJumpReady){
            controlledMovement.y = GetDoubleJumpSpeed();
            // transform.DOLocalRotate(Vector3.up * 720, 1.0f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart);
            doubleJumpReady = false;
        }
        return controlledMovement;
    }
    override public MovementStateName GetState(){
        // if(Input.GetButtonDown("Fire1"))
        //     return MovementStateName.ChargeSlam;
        // else 
        if(cc.isGrounded)
            return MovementStateName.Standard;
        else
            return MovementStateName.Midair;
    }
    public float GetDoubleJumpSpeed(){
        return doubleJumpSpeed;
    }
    public void OnControllerColliderHit(ControllerColliderHit hit){
        if(_isActive && GetComponent<WallClimbMovement>().wallClimbEnabled && 
            Vector3.Dot(Vector3.up, hit.normal) < 0.7071 && Vector3.Dot(Vector3.up, hit.normal) > -0.7071 && 
            Vector3.Dot(mc.GetMovement().normalized, -1.0f * hit.normal) > 0.7071){
            // TODO base the above if on movement direction
            // Hold onto wall
            // GetComponent<StandardMovement>().jumpLevel = 0;
            mc.SetMovementState(MovementState.MovementStateName.WallClimb);
            GetComponent<WallClimbMovement>().SetWallClimbNormal(hit.normal);
            // SetState(MovementStateE.WallClimb);
        }
    }
}
