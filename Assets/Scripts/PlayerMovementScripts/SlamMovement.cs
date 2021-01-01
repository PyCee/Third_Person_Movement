using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlamMovement : MovementState
{
    [Header("Slam")]
    [Tooltip("Speed Of Slam"), Min(0.0f)]
    public float slamMoveSpeed;
    public float leapSpeed;
    public float angleFromDown;
    private Vector3 _direction;
    private Vector3 _startPosition;
    private bool _hitEnemy;// Used to avoid situation where state is switched before leap movement can be applied
    private bool _leap;

    override protected void _Start(){
        _canShoot = false;
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.blue);
        _hitEnemy = false;
        _leap = false;
        _startPosition = transform.position;
        if(_direction == Vector3.zero)
            print("ERROR::_direction should not have a magnitude of zero. May not have been set prior to 'SwitchTo'.");
    }
    override protected void _SwitchFrom(){
        _direction = Vector3.zero;
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        controlledMovement = _direction * slamMoveSpeed;
        if(_hitEnemy){
            controlledMovement.y = leapSpeed;
            _leap = true;
        }
        return controlledMovement;
    }
    override public System.Type GetState(){
        if(_leap)
            return typeof(MidairMovement);
        else if(cc.isGrounded && !_hitEnemy)
            return typeof(StandardMovement);
        else
            return typeof(SlamMovement);
    }
    public void SetDirection(Vector3 direction){
        float angle = Vector3.Dot(Vector3.down, direction.normalized);
        if(angle < angleFromDown){
            print("TODO: behavior when slam angle is not allowed");
        }

        _direction = direction;
    }
    override protected void _OnControllerColliderHit(ControllerColliderHit hit){
        // if hit angle is opposite _direction
        // On Slam down, do stuff
        EnemyController ec = hit.gameObject.GetComponent<EnemyController>();
        if(ec != null){
            // print("hit enemy");
            _hitEnemy = true;
        } else {
            // print("hit not enemy");
        }
    }
}
