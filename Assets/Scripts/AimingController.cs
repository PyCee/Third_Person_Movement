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
    [Tooltip("Aiming Cinemachine Camera")]
    public CinemachineFreeLook aimingCamera;
    [Tooltip("Standard Cinemachine Camera")]
    public CinemachineFreeLook standardCamera;
    public Transform shotOrigin;

    private float startingFixedDeltaTime; // Normal physics update delta time

    private bool currentlyAiming;

    private int shotGeneratorIndex;
    private List<ShotGenerator> shotGenerators;

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
        if(Input.GetKeyDown(KeyCode.E)){
            IncrementShotIndex();
            print(shotGenerators[shotGeneratorIndex].GetType());
            // TODO: End aim of last and start aim of new to handle visual indicators
        } else if(Input.GetKeyDown(KeyCode.Q)){
            DecrementShotIndex();
            print(shotGenerators[shotGeneratorIndex].GetType());
        }

        // TODO::Update to new unity input system. Currently use legacy system.
        if(Input.GetButtonDown("Fire1")){
            StartAim();
        } else if (Input.GetButtonUp("Fire1")){
            EndAim();
            Shoot();
        }
        if(currentlyAiming){
            //TODO Update visual element
            //TODO rotate character so aim direction is formward
        }
    }
    public void StartAim(){
        // Slow down time
        ChangeTimeOverTime(slowdownRate);
        // TODO Change camera
        StartAimCamera();
        // TODO Add visual element for aim
        //
        currentlyAiming = true;
    }
    public void EndAim(){
        // TODO remove visual aiming element
        // TODO revert to normal camera
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
    private void StartAimCamera(){
        aimingCamera.m_XAxis.Value = standardCamera.m_XAxis.Value;
        aimingCamera.m_YAxis.Value = standardCamera.m_YAxis.Value;
        aimingCamera.m_Priority = 11;
    }
    private void EndAimCamera(){
        standardCamera.m_XAxis.Value = aimingCamera.m_XAxis.Value;
        standardCamera.m_YAxis.Value = aimingCamera.m_YAxis.Value;
        aimingCamera.m_Priority = 9;
    }
    private void Shoot(){
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (CanShoot() && 
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity)){
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
    public void IncrementShotIndex(){
        shotGeneratorIndex++;
        ApplyShotIndexBounds();
    }
    public void DecrementShotIndex(){
        shotGeneratorIndex--;
        ApplyShotIndexBounds();

    }
    private void ApplyShotIndexBounds(){
        shotGeneratorIndex %= shotGenerators.Count;
    }
}
