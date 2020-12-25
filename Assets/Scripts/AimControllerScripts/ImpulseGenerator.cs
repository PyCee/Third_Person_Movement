using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseGenerator : ShotGenerator
{
    override public void Shoot(Vector3 origin, Vector3 dir){
        GetComponent<AirDashMovement>().SetDirection(dir);
        GetComponent<MovementController>().SetMovementState(typeof(AirDashMovement));
    }
}
