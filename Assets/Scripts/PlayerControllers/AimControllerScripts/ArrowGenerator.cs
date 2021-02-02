using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : ShotGenerator
{
    public float arrowSpeed;
    public GameObject arrowPrefab;
    
    override public void Shoot(Vector3 origin, Vector3 dir){
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.position = origin;
        arrow.transform.rotation = Quaternion.LookRotation(dir) * arrow.transform.rotation;
        Vector3 arrowImpulse = dir * arrowSpeed;
        arrow.GetComponent<Rigidbody>().AddForce(arrowImpulse);
    }
    override public void BeginAim(){}
    override public void EndAim(){}
}
