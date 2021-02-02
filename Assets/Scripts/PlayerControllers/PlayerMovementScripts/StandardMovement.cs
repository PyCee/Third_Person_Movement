using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StandardMovement : MovementState
{
    [Header("General Movement")]
    [Tooltip("Standard Movement Speed"), Min(0.0f)]
    public float moveSpeed;
    [Tooltip("Base Jump Velocity"), Min(0.0f)]
    public float jumpSpeed;
    [Tooltip("How Quickly The Player Turns"), Min(0.0f)]
    public float rotationSpeed;
    
    [Header("Variable Height Jump")]
    [Tooltip("If Variable Height Jump Is Enabled")]
    public bool enableVariableJump;
    [Tooltip("Second Jump Velocity"), Min(0.0f)]
    public float secondJumpSpeed;
    [Tooltip("Third Jump Velocity"), Min(0.0f)]
    public float thirdJumpSpeed;
    [Tooltip("Time Window Between Jumps"), Min(0.0f)]
    public float highJumpTimeWindow;

    private int jumpLevel = 0;
    private float timeGrounded;
    private Sequence jumpSequence;

    override protected void _Start(){
        jumpSequence = DOTween.Sequence();
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.red);
        timeGrounded = Time.time;
    }
    override protected void _SwitchFrom(){
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        // TODO: test with cc.velocity in place of vel, might lag and not use last frame info
        controlledMovement *= moveSpeed;
        controlledMovement.y = vel.y;

        if(cc.isGrounded){
            if(Input.GetButtonDown("Jump")){
                controlledMovement.y = GetJumpVelocity();
            } else {
                controlledMovement.y = 0.0f;
            }
        }

        return controlledMovement;
    }
    override public System.Type GetState(){
        if(!cc.isGrounded)
            return typeof(MidairMovement);
        else if(Input.GetButton("Sprint"))
            return typeof(SprintMovement);
        else
            return typeof(StandardMovement);
    }
    private float GetJumpVelocity(){
        float vel = 0.0f;
        if(enableVariableJump){
            if(Time.time - timeGrounded > highJumpTimeWindow){
                jumpLevel = 0;
            }

            float[] jumpSpeeds = {jumpSpeed, secondJumpSpeed, thirdJumpSpeed};
            jumpSequence.Kill();
            // TODO: cool jumps
            // Vector3[] DORotation = {new Vector3(0,0,0), transform.right * -360, Vector3.up * 720};
            // jumpSequence.Append(transform.DOLocalRotate(DORotation[jumpLevel], 1.0f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart));
            vel = jumpSpeeds[jumpLevel];
            jumpLevel += 1;
            jumpLevel %= jumpSpeeds.Length;
        } else {
            vel = jumpSpeed;
        }
        return vel;
    }
}
