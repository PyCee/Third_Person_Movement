using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public abstract class ShotGenerator : MonoBehaviour
{
    [Tooltip("Camera to use with this aiming mode")]
    public CinemachineFreeLook aimCamera;
    abstract public void Shoot(Vector3 origin, Vector3 dir);
    abstract public void BeginAim();
    abstract public void EndAim();

}
