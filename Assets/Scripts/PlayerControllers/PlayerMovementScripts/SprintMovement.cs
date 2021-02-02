using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SprintMovement : MovementState
{
    [Tooltip("Movement Speed"), Min(0.0f)]
    public float moveSpeed;
    [Tooltip("Base Jump Velocity"), Min(0.0f)]
    public float jumpSpeed;
    [Tooltip("How Quickly The Player Turns"), Min(0.0f)]
    public float rotationSpeed;

    override protected void _Start(){
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.gray);
        // lower turn speed
    }
    override protected void _SwitchFrom(){
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
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
        else if(!Input.GetButton("Sprint"))
            return typeof(StandardMovement);
        else
            return typeof(SprintMovement);
    }
    private float GetJumpVelocity(){
        float vel = jumpSpeed;
        return vel;
    }
}
