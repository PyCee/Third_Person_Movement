using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AirDashMovement : MovementState
{
    [Header("Air Dash")]
    [Tooltip("Speed Of Dash"), Min(0.0f)]
    public float dashMoveSpeed;
    [Tooltip("Distance Of Dash"), Min(0.0f)]
    public float dashMoveDistance;
    private Vector3 _direction;
    private Vector3 _startPosition;

    public AirDashMovement()
        : base(MovementStateName.AirDash) {
    }

    override protected void _Start(){
        _canShoot = false;
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.yellow);
        _startPosition = transform.position;
    }
    override protected void _SwitchFrom(){
        _direction = Vector3.zero;
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        if(_direction == Vector3.zero)
            print("ERROR::Airdash should not have a zero movement direction. May not have been set.");
        return _direction * dashMoveSpeed;
    }
    override public MovementStateName GetState(){

        // TODO: if hit wall, change to midair

        float sqrDistance = (transform.position - _startPosition).sqrMagnitude;
        if(sqrDistance >= Mathf.Pow(dashMoveDistance, 2))
            return MovementStateName.Midair;
        else if(cc.isGrounded)
            return MovementStateName.Standard;
        else
            return MovementStateName.AirDash;
    }
    public void SetDirection(Vector3 direction){
        _direction = direction;
    }
    public void OnControllerColliderHit(ControllerColliderHit hit){
        if(_isActive){
            // if hit angle is opposite _direction
        }
    }
}
