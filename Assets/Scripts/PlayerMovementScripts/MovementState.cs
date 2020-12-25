using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementState : MonoBehaviour
{
    public enum MovementStateName {
        Standard,
        Midair,
        WallClimb,
        Vault,
        AirDash,
        Dive,
        ChargeSlam,
        Slam
    }
    public readonly MovementStateName state;
    protected CharacterController cc;
    protected MovementController mc;
    protected Material material;

    private List<MovementStateName> usedNames = new List<MovementStateName>();

    protected bool _isActive;
    protected bool _canShoot;
    public bool CanShoot(){return _canShoot;}

    public MovementState(MovementStateName initState){
        state = initState;

        if(usedNames.Contains(state)){
            print("ERROR::multiple movement states with name '"+state+"'");
        } else {
            usedNames.Add(state);
        }
    }
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
    abstract public MovementStateName GetState();
    public void SwitchTo(){
        _isActive = true;
        _SwitchTo();
    }
    public void SwitchFrom(){
        _SwitchFrom();
        _isActive = false;
    }
}
