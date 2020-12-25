using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VaultMovement : MovementState
{
    [Header("Vault")]
    // TODO have variable to store normal data when vaulting, currently just use wall climb normal (months later, idk what this means)
    [Tooltip("???"), Min(0.0f)]
    public float vaultRayOffset;
    [Tooltip("Distance Of Vault Ray"), Min(0.0f)]
    public float vaultRayDistance;
    [Tooltip("Speed Of Vault Climb"), Min(0.0f)]
    public float vaultLiftSpeed;
    [Tooltip("Speed Of Vault"), Min(0.0f)]
    public float vaultJumpSpeed;
    [Tooltip("Angle Of Vault"), Min(0.0f)]
    public float vaultJumpAngle;

    private WallClimbMovement wallClimbMovement;

    override protected void _Start(){
        wallClimbMovement = GetComponent<WallClimbMovement>();
    }
    override protected void _SwitchTo(){
        material.SetColor("_Color", Color.magenta);
        vaultReady = false;
    }
    override protected void _SwitchFrom(){
    }
    
    private bool vaultReady;
    override public Vector3 GetMovement(Vector3 vel, Vector3 controlledMovement){
        Vector3 vaultLowerOffset = new Vector3(0.0f, vaultRayOffset, 0.0f);
        if (!Physics.Raycast(transform.position + vaultLowerOffset, wallClimbMovement.GetWallClimbNormal() * -1.0f, vaultRayDistance)){
            // If character is high enough to vault
            Vector3 cross = Vector3.Cross(wallClimbMovement.GetWallClimbNormal(), Vector3.up);
            Quaternion qua = Quaternion.AngleAxis(-vaultJumpAngle, cross);
            Vector3 dir = qua * (wallClimbMovement.GetWallClimbNormal() * -1.0f);
            controlledMovement = dir * vaultJumpSpeed;
            vaultReady = true;
        } else {
            Vector3 vaultMovement = new Vector3(0.0f, 1.0f, 0.0f);
            vaultMovement *= wallClimbMovement.GetWallClimbSpeed();
            controlledMovement = vaultMovement;
        }
        return controlledMovement;
    }
    override public System.Type GetState(){
        if(vaultReady)
            return typeof(MidairMovement);
        else
            return typeof(VaultMovement);
    }
}
