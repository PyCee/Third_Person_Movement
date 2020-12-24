using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShotGenerator : MonoBehaviour
{
    abstract public void Shoot(Vector3 origin, Vector3 dir);
}
