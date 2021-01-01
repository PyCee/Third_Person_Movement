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

    override protected void _Start(){
        _canShoot = false;
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.yellow);
        _startPosition = transform.position;
        if(_direction == Vector3.zero)
            print("ERROR::_direction should not have a magnitude of zero. May not have been set prior to 'SwitchTo'.");
    }
    override protected void _SwitchFrom(){
        _direction = Vector3.zero;
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        return _direction * dashMoveSpeed;
    }
    override public System.Type GetState(){
        // TODO: if hit wall, return midair
        float sqrDistance = (transform.position - _startPosition).sqrMagnitude;
        if(sqrDistance >= Mathf.Pow(dashMoveDistance, 2))
            return typeof(MidairMovement);
        else if(cc.isGrounded)
            return typeof(StandardMovement);
        else
            return typeof(AirDashMovement);
    }
    public void SetDirection(Vector3 direction){
        _direction = direction;
    }
    override protected void _OnControllerColliderHit(ControllerColliderHit hit){
            // if hit angle is opposite _direction
    }
}
