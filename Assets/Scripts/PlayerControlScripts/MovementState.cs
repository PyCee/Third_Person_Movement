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
        Dive,
        ChargeSlam,
        Slam
    }
    public readonly MovementStateName state;
    protected CharacterController cc;
    protected MovementController mc;
    protected Material material;

    private List<MovementStateName> usedNames = new List<MovementStateName>();

    public MovementState(MovementStateName initState){
        state = initState;

        if(usedNames.Contains(state)){
            print("ERROR::multiple movement states with name '"+state+"'");
        } else {
            usedNames.Add(state);
        }
    }
    
    protected void Init(){
        cc = GetComponent<CharacterController>();
        mc = GetComponent<MovementController>();
        material = GetComponent<Renderer>().material;
    }
    
    abstract public Vector3 GetMovement(Vector3 previousMovement, Vector3 controlledMovement);
    abstract public MovementStateName GetState();
    abstract public void SwitchTo();
    abstract public void SwitchFrom();
}
