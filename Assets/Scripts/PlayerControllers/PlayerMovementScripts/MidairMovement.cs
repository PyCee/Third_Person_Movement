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
    private bool _doubleJumpReady = true;
    private bool _downwards;
    private bool _slam;

    override protected void _Start(){
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.black);
        _doubleJumpReady = true;
        // Must use movement variable to calculate momentum because character controller velocity is a frame behind
        midairMomentum = mc.GetMovement();
        midairMomentum.y = 0.0f;
        midairMomentum *= momentumMultiplier;
        _slam = false;
    }
    override protected void _SwitchFrom(){
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        controlledMovement *= midairMoveSpeed;
        controlledMovement = new Vector3(controlledMovement.x, vel.y, controlledMovement.z);
        controlledMovement += midairMomentum;
        
        if(Input.GetButtonDown("Slam")){
            _slam = true;
        } else if(Input.GetButtonDown("Jump") && enableDoubleJump && _doubleJumpReady){
            controlledMovement.y = GetDoubleJumpSpeed();
            // transform.DOLocalRotate(Vector3.up * 720, 1.0f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart);
            _doubleJumpReady = false;
        }
        return controlledMovement;
    }
    override public System.Type GetState(){
        if(cc.isGrounded && GetComponent<MovementController>().GetMovement().y < 0.0f)
            return typeof(StandardMovement);
        else if(_slam)
            return typeof(SlamMovement);
        else
            return typeof(MidairMovement);
    }
    private float GetDoubleJumpSpeed(){
        return doubleJumpSpeed;
    }
    override protected void _OnControllerColliderHit(ControllerColliderHit hit){
        if(GetComponent<WallClimbMovement>().wallClimbEnabled && 
            Vector3.Dot(Vector3.up, hit.normal) < 0.7071 && Vector3.Dot(Vector3.up, hit.normal) > -0.7071 && 
            Vector3.Dot(mc.GetMovement().normalized, -1.0f * hit.normal) > 0.7071){
            // TODO base the above if on movement direction
            // Hold onto wall
            // GetComponent<StandardMovement>().jumpLevel = 0;
            GetComponent<WallClimbMovement>().SetWallClimbNormal(hit.normal);
            mc.SetMovementState(typeof(WallClimbMovement));
        }
    }
}
