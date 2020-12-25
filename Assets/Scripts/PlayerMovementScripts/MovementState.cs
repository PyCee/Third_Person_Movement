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
    abstract protected void _Start();
    abstract protected void _SwitchTo();
    abstract protected void _SwitchFrom();
    
    abstract public Vector3 GetMovement(Vector3 previousMovement, Vector3 controlledMovement);
    abstract public System.Type GetState();
    public void SwitchTo(){
        _isActive = true;
        _SwitchTo();
    }
    public void SwitchFrom(){
        _SwitchFrom();
        _isActive = false;
    }
}
