﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(StandardMovement))]
[RequireComponent(typeof(SprintMovement))]
[RequireComponent(typeof(MidairMovement))]
[RequireComponent(typeof(WallClimbMovement))]
[RequireComponent(typeof(VaultMovement))]
[RequireComponent(typeof(AirDashMovement))]
[RequireComponent(typeof(SlamMovement))]
public class MovementController : MonoBehaviour
{
    private CharacterController cc;

    private Vector3 movement;
    public Vector3 GetMovement(){return movement;}
    
    private Material material;

    private List<MovementState> movementStates;
    private MovementState currMovementState;
    public MovementState GetCurrentMovementState(){return currMovementState;}
    public void SetMovementState(System.Type T){
        currMovementState.SwitchFrom();
        currMovementState = null;
        for(int i = 0; i < movementStates.Count; i++){
            if(movementStates[i].GetType() == T){
                currMovementState = movementStates[i];
                break;
            }
        }
        if(currMovementState == null){
            print("ERROR::No used movement state with type '" + T + "'");
            return;
        } else
            currMovementState.SwitchTo();
    }

    void Start()
    {
        material = GetComponent<Renderer>().material;
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();

        currMovementState = GetComponent<StandardMovement>();
        movementStates = new List<MovementState>();
        foreach(MovementState movementState in GetComponents<MovementState>()){
            movementStates.Add(movementState);
        }
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 flatForward = Camera.main.transform.forward;
        flatForward.y = 0.0f;
        Vector3 flatRight = Camera.main.transform.right;
        flatRight.y = 0.0f;
        Vector3 controlledMovement = flatForward.normalized * z + flatRight.normalized * x;

        System.Type t = currMovementState.GetState();
        if(t != currMovementState.GetType()){
            SetMovementState(t);
        }
        // TODO: remove first parameter to GetMovement and reference cc.velocity in the function
        // movement = currMovementState.GetMovement(movement, controlledMovement);
        movement = currMovementState.GetMovement(cc.velocity, controlledMovement);

        movement += Physics.gravity * Time.deltaTime;
        Vector3 frameMovement = movement * Time.deltaTime;

        cc.Move(frameMovement);

        // TODO: handle turning in movement states
        TurnTowards(controlledMovement);
    }

    private void TurnTowards(Vector3 dir){
        if (dir.sqrMagnitude != 0.0f){
            transform.rotation = Quaternion.LookRotation(dir.normalized);
            // transform.DOLocalRotate(new Vector3(0.0f, 360.0f, 0.0f), 3.0f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart);
        }
    }
    public void OnTriggerEnter(Collider collider){
        if(collider.gameObject.CompareTag("Bell")){
            // TODO: get bell
            collider.gameObject.GetComponent<BellController>().GetBell();
        }
    }
    
}
/*
[Header("Dive")]
public bool diveEnabled;
public float diveSpeedMultiplier;
public float diveDrag;
public float minDiveSpeed;
public float endDiveSpeed;
private Vector3 diveMovement;
[Header("Slam")]
public float slowdownRate;
public GameObject slamTargetMarkerPrefab;
private GameObject slamTargetMarker;
private float normalFixedDeltaTime; // Normal physics update delta time
*/
// private Vector3 DiveMovement(Vector3 controlledMovement){
//     // Slow dive movement on ground
//     if (cc.isGrounded){
//         diveMovement *= (1.0f - (diveDrag * Time.deltaTime));
//     }
//     movement = new Vector3(diveMovement.x, movement.y, diveMovement.z);
//     if(diveMovement.sqrMagnitude < (endDiveSpeed * endDiveSpeed)){
//         // TODO: change this from diveMovement to flat cc.velocity
//         SetState(MovementStateE.Standard);
//     }
//     return controlledMovement;
// }
// public void OnControllerColliderHit(ControllerColliderHit hit){
    // if(movementStateE == MovementStateE.Dive && Vector3.Dot(Vector3.up, hit.normal) < 0.7071){
    //     // If player hit wall during dive
    //     // TODO: filter out collisions that are not head on
    //     // End dive
    //     // Stun character for slamming head into wall?
    //     SetState(MovementStateE.Standard);
    // }
// }