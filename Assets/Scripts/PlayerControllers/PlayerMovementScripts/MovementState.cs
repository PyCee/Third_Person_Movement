using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementState : MonoBehaviour
{
    protected CharacterController cc;
    protected MovementController mc;
    protected Material material;

    protected bool _isActive;
    protected bool _canShoot;
    public bool CanShoot(){return _canShoot;}

    void Start(){
        cc = GetComponent<CharacterController>();
        mc = GetComponent<MovementController>();
        material = GetComponent<Renderer>().material;
        _isActive = false;
        _canShoot = true;
        _Start();
    }
    virtual protected void _Start(){}
    virtual protected void _SwitchTo(){}
    virtual protected void _Update(){}
    virtual protected void _SwitchFrom(){}
    virtual protected void _OnControllerColliderHit(ControllerColliderHit hit){}
    
    abstract public Vector3 GetMovement(Vector3 previousMovement, Vector3 controlledMovement);
    abstract public System.Type GetState();
    public void SwitchTo(){
        _isActive = true;
        _SwitchTo();
    }
    public void Update(){
        if(_isActive){
            _Update();
        }
    }
    public void SwitchFrom(){
        _SwitchFrom();
        _isActive = false;
    }
    public void OnControllerColliderHit(ControllerColliderHit hit){
        if(_isActive){
            _OnControllerColliderHit(hit);
        }
    }
}
