using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlamMovement : MovementState
{
    [Header("Slam")]
    [Tooltip("Speed Of Slam"), Min(0.0f)]
    public float slamMoveSpeed;
    private Vector3 _direction;
    private Vector3 _startPosition;

    override protected void _Start(){
        _canShoot = false;
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.grey);
        _startPosition = transform.position;
        if(_direction == Vector3.zero)
            print("ERROR::_direction should not have a magnitude of zero. May not have been set prior to 'SwitchTo'.");
    }
    override protected void _SwitchFrom(){
        _direction = Vector3.zero;
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        return _direction * slamMoveSpeed;
    }
    override public System.Type GetState(){
        // float sqrDistance = (transform.position - _startPosition).sqrMagnitude;
        // if(sqrDistance >= Mathf.Pow(slamMoveDistance, 2))
        //     return typeof(MidairMovement);
        // else 
        if(cc.isGrounded)
            return typeof(StandardMovement);
        else
            return typeof(SlamMovement);
    }
    public void SetDirection(Vector3 direction){
        _direction = direction;
        // if direction isnt down, error
    }
    public void OnControllerColliderHit(ControllerColliderHit hit){
        if(_isActive){
            // if hit angle is opposite _direction
        }
    }
}
