using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlamGenerator : ShotGenerator
{
    override public void Shoot(Vector3 origin, Vector3 dir){
        GetComponent<SlamMovement>().SetDirection(dir);
        GetComponent<MovementController>().SetMovementState(typeof(SlamMovement));
    }
    override public void BeginAim(){}
    override public void EndAim(){}
}
