using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(ArrowGenerator))]
[RequireComponent(typeof(ImpulseGenerator))]
public class AimingController : MonoBehaviour
{

    // TODO: different shot types
    [Header("Aiming Parameters")]
    [Tooltip("Rate Of Passing Time While Aiming"), Range(0.0f, 1.0f)]
    public float slowdownRate;
    [Tooltip("Duration Of Time Change"), Min(0.0f)]
    public float timerateChangeDuration;
    [Tooltip("Standard Cinemachine Camera")]
    public CinemachineFreeLook standardCamera;
    public Transform shotOrigin;
    [Tooltip("Layers to exclude from aiming raycast")]
    public LayerMask excludeAimLayers;

    private float startingFixedDeltaTime; // Normal physics update delta time

    private bool currentlyAiming;

    private int shotGeneratorIndex;
    private List<ShotGenerator> shotGenerators;
    private CinemachineFreeLook GetCurrentCamera(){
        return shotGenerators[shotGeneratorIndex].aimCamera;
    }

    void Start()
    {
        startingFixedDeltaTime = Time.fixedDeltaTime;
        currentlyAiming = false;

        shotGenerators = new List<ShotGenerator>();
        foreach(ShotGenerator shotGenerator in GetComponents<ShotGenerator>()){
            shotGenerators.Add(shotGenerator);
        }
        shotGeneratorIndex = 0;
    }

    void Update()
    {

        // TODO::Update to new unity input system. Currently use legacy system.
        if(Input.GetButtonDown("Fire1")){
            StartAim();
        } else if (Input.GetButtonUp("Fire1")){
            EndAim();
            Shoot();
        }
        if(currentlyAiming){
            if(Input.GetKeyDown(KeyCode.E)){
                ChangeAimingType(+1);
                // TODO: visual indicator for shot type
                // TODO: End aim of last and start aim of new to handle visual indicators
            } else if(Input.GetKeyDown(KeyCode.Q)){
                ChangeAimingType(-1);
            }
            //TODO Update visual element
            //TODO rotate character so aim direction is forward
        }
    }
    public void StartAim(){
        // Slow down time
        ChangeTimeOverTime(slowdownRate);
        StartAimCamera();
        // TODO Add visual element for aim
        
        currentlyAiming = true;
    }
    public void EndAim(){
        // TODO remove visual aiming element

        EndAimCamera();
        // Normal time
        ChangeTimeOverTime(1.0f);
        currentlyAiming = false;
    }
    private void ChangeTimeOverTime(float endingTimeRate){
        // TODO: make this happen over time
        //   rn it is instant
        Time.fixedDeltaTime = startingFixedDeltaTime * endingTimeRate;
        Time.timeScale = endingTimeRate;
    }

    // Copy Camera Parameters From One Camera To Another And Set Priorities
    private void SwitchToCamera(CinemachineFreeLook switchToCamera, CinemachineFreeLook switchFromCamera){
        switchToCamera.m_XAxis.Value = switchFromCamera.m_XAxis.Value;
        switchToCamera.m_YAxis.Value = switchFromCamera.m_YAxis.Value;
        switchFromCamera.m_Priority = 9;
        switchToCamera.m_Priority = 11;
    }
    private void StartAimCamera(){
        SwitchToCamera(GetCurrentCamera(), standardCamera);
    }
    private void EndAimCamera(){
        SwitchToCamera(standardCamera, GetCurrentCamera());
    }
    private void ApplyShotIndexBounds(){
    }
    private void ChangeAimingType(int indexChange){
        int lastIndex = shotGeneratorIndex;

        shotGeneratorIndex += indexChange;

        shotGeneratorIndex %= shotGenerators.Count;
        if(shotGeneratorIndex > shotGenerators.Count) shotGeneratorIndex -= shotGenerators.Count;
        else if(shotGeneratorIndex < 0) shotGeneratorIndex += shotGenerators.Count;

        SwitchToCamera(GetCurrentCamera(), shotGenerators[lastIndex].aimCamera);
        print(shotGenerators[shotGeneratorIndex].GetType());
    }
    private void Shoot(){
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (CanShoot() && 
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, ~excludeAimLayers)){
            // If the center of the camera raycast hists an object
            Vector3 offset = hit.point - shotOrigin.position;
            Vector3 dir = offset / offset.magnitude;
            Debug.DrawRay(shotOrigin.position, offset, Color.blue, 2);

            // TODO handle projectile based on selected shot type (arrow, hookshot, airdash, ...)
            shotGenerators[shotGeneratorIndex].Shoot(shotOrigin.position, dir);

        } else {
            // Shoot in direction
        }
    }
    private bool CanShoot(){
        return GetComponent<MovementController>().GetCurrentMovementState().CanShoot();
    }
}
