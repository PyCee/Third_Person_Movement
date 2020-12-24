using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseGenerator : ShotGenerator
{
    public float forceSpeed;
    public float duration;
    public bool useGravity;
    
    override public void Shoot(Vector3 origin, Vector3 dir){
        Vector3 impulse = dir * forceSpeed;
        // TODO: make work with player that has CharacterController, not Rigidbody
        // GetComponent<Rigidbody>().AddForce(impulse);
    }
}
