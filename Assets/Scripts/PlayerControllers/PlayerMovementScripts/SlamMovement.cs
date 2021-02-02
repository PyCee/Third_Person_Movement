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
    [Tooltip("Angular Range Of Slam"), Range(0.0f, 90.0f)]
    public float degreeVariance;
    private Vector3 _direction;
    private Vector3 _startPosition;
    private bool _hitEnemy;// Used to avoid situation where state is switched before leap movement can be applied
    private bool _leap;

    // TODO: lock on if aimed point is near enemy

    override protected void _Start(){
        _canShoot = false;
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.blue);
        _hitEnemy = false;
        _leap = false;
        _startPosition = transform.position;
        _direction = GetDirection();
    }
    override protected void _SwitchFrom(){
        _direction = Vector3.zero;
    }
    
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        controlledMovement = _direction * slamMoveSpeed;
        if(_hitEnemy){
            // controlledMovement.y = leapSpeed;
            controlledMovement = new Vector3(0.0f, leapSpeed, 0.0f);
            // TODO: make after_hit context dependent
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
    private float GetAngleVariance(){
        return Mathf.Deg2Rad * degreeVariance;
    }
    private Vector3 GetDirection(){
        // TODO: make default inherit momentum
        Vector3 slamDirection = new Vector3(0.0f, -1.0f, 0.0f);

        // Get all enemies with a 'SlamContext' component
        SlamContext[] contexts = FindObjectsOfType<SlamContext>();

        // Find all that are under a certain angle from 
        foreach(SlamContext context in contexts){
            Vector3 dir = (context.transform.position - transform.position).normalized;
            float angle = Vector3.Dot(Vector3.down, dir);
            if(angle >= (1.0f - GetAngleVariance())){
                // If this object is at a valid angle
                // TODO: Attempt to determine player intent to determine target
                slamDirection = dir;
            }
        }
        return slamDirection;
    }
}
