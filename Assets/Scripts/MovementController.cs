using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    [Header("General Movement")]
    [Tooltip("Standard Movement Speed")]
    public float moveSpeed;
    public float midairSpeedMultiplier;
    public float momentumMultiplier;
    public float jumpSpeed;
    //public Transform cameraTransform;
    [Header("Variable Height Jump")]
    public bool enableVariableJump;
    public float midJumpSpeed;
    public float highJumpSpeed;
    public float highJumpTimeWindow;
    [Header("Double Jump")]
    public bool enableDoubleJump;
    public float doubleJumpSpeed;
    public bool jumpAfterDive;
    [Header("Dive")]
    public float diveSpeedMultiplier;
    public float diveDrag;
    public float minDiveSpeed;
    public float endDiveSpeed;
    [Header("Wall Jump")]
    public bool enableWallJump;
    public float wallJumpAngle;
    public float wallJumpSpeed;
    private Vector3 wallJumpNormal;
    [Header("Other")]
    public float slowdownRate;
    public GameObject slamTargetMarkerPrefab;

    private float timeOnGround = 0.0f;
    private bool doubleJumpReady = false;
    private CharacterController cc;
    private Vector3 movement;
    private int jumpLevel = 0;
    private Vector3 diveMovement;
    private Vector3 midairMomentum;
    private float fixedDeltaTime;
    private GameObject slamTargetMarker;

    enum MovementState {
        Standard,
        Midair,
        WallJump,
        Dive,
        ChargeSlam,
        Slam
    }
    private MovementState movementState = MovementState.Standard;

    private Material material;
    void Start()
    {
        material = GetComponent<Renderer>().material;
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();

        this.fixedDeltaTime = Time.fixedDeltaTime;
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
        controlledMovement *= moveSpeed;

        if (cc.isGrounded)
            WhileGrounded();
        else
            WhileNotGrounded();

        switch(movementState){
            case MovementState.Standard:
                if(!cc.isGrounded){
                    SetState(MovementState.Midair);
                } else if(Input.GetButtonDown("Jump")){
                    if(enableVariableJump){
                        VariableJump();
                    } else {
                        Jump();
                    }
                }
                material.SetColor("_Color", Color.red);//
                movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);
                break;
            case MovementState.Midair:
                material.SetColor("_Color", Color.black);//
                controlledMovement *= midairSpeedMultiplier;
                movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);
                movement += midairMomentum;

                if(Input.GetButtonDown("Jump")){
                    if(enableDoubleJump && doubleJumpReady){
                        DoubleJump();
                    }
                }
                if(Input.GetButtonDown("Dive")){
                    Vector3 flatMovement = movement;
                    flatMovement.y = 0;
                    if((flatMovement.sqrMagnitude > (minDiveSpeed * minDiveSpeed))){
                        diveMovement = flatMovement * diveSpeedMultiplier;
                        movement.y = 0;
                        doubleJumpReady = jumpAfterDive;
                        SetState(MovementState.Dive);
                        // TODO: start dive animation
                    }
                }
                if(Input.GetButtonDown("Fire1")){
                    SetState(MovementState.ChargeSlam);
                    if(slamTargetMarker == null){
                        slamTargetMarker = Instantiate(slamTargetMarkerPrefab);
                    }
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(transform.position, Vector3.up * -1.0f, out hit, Mathf.Infinity))
                    {
                        slamTargetMarker.transform.position = hit.point + new Vector3(0.0f, 0.25f, 0.0f);
                    }
                }
                if(cc.isGrounded){
                    SetState(MovementState.Standard);
                }
                break;
            case MovementState.WallJump:
                movement = new Vector3(0.0f, 0.0f, 0.0f);
                material.SetColor("_Color", Color.green);//

                if(cc.isGrounded){
                    SetState(MovementState.Standard);
                } else if(Input.GetButtonDown("Jump")){
                    Vector3 cross = Vector3.Cross(wallJumpNormal, Vector3.up);
                    Quaternion qua = Quaternion.AngleAxis(wallJumpAngle, cross);
                    Vector3 dir = qua * wallJumpNormal;
                    movement = dir * wallJumpSpeed;
                    SetState(MovementState.Midair);
                }
                break;
            case MovementState.Dive:
                if (cc.isGrounded){
                    // Slow dive movement
                    diveMovement *= (1.0f - (diveDrag * Time.deltaTime));
                }
                material.SetColor("_Color", Color.blue);//
                if(diveMovement.sqrMagnitude < (endDiveSpeed * endDiveSpeed)){
                    // TODO: change this from diveMovement to flat cc.velocity
                    SetState(MovementState.Standard);
                }
                movement = new Vector3(diveMovement.x, movement.y, diveMovement.z);
                break;
            case MovementState.ChargeSlam:
                material.SetColor("_Color", Color.green);//
                // TODO: slow down over time
                Time.timeScale = slowdownRate;
                Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
                // TODO: aim slam target
                if(Input.GetButtonUp("Fire1")){
                    // TODO: Store slam info based on input
                    Destroy(slamTargetMarker);
                    SetState(MovementState.Slam);
                }
                movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);
                break;
            case MovementState.Slam:
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
                if(cc.isGrounded)
                    SetState(MovementState.Standard);
                material.SetColor("_Color", Color.white);//
                //movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);
                break;
            default:
                break;
        }
        

        movement += Physics.gravity * Time.deltaTime;
        cc.Move(movement * Time.deltaTime);
    }
    private void SetState(MovementState ms){
        movementState = ms;
        switch(movementState){
            case MovementState.Standard:
                break;
            case MovementState.Midair:
                midairMomentum = movement * momentumMultiplier;
                midairMomentum.y = 0.0f;
                break;
            case MovementState.WallJump:
                break;
            case MovementState.Dive:
                break;
            case MovementState.ChargeSlam:
                break;
            case MovementState.Slam:
                break;
            default:
                break;
        }
    }
    public void WhileGrounded(){
        doubleJumpReady = true;
        if(timeOnGround == 0.0){
            float yVel = cc.velocity.y;
            int emmisionCount = 0;
            if(yVel > -5.0f){
                emmisionCount = 0;
            } else {
                emmisionCount = 10;
            }
            GetComponent<ParticleSystem>().Emit(emmisionCount);
        }
        timeOnGround += Time.deltaTime;
        if(timeOnGround > highJumpTimeWindow){
            jumpLevel = 0;
        }
        movement.y = 0.0f;
    }
    private void WhileNotGrounded(){
        timeOnGround = 0.0f;
    }
    public void Jump(){
        movement.y = jumpSpeed;
    }
    public void VariableJump(){
        float[] jumpSpeeds = {jumpSpeed, midJumpSpeed, highJumpSpeed};
        Vector3[] DORotation = {new Vector3(0,0,0), transform.right * -360, Vector3.up * 720};
        transform.DOLocalRotate(DORotation[jumpLevel], 1.0f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart);
        movement.y = jumpSpeeds[jumpLevel];
        jumpLevel += 1;
        jumpLevel %= jumpSpeeds.Length;
    }
    public void DoubleJump(){
        transform.DOLocalRotate(Vector3.up * 720, 1.0f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart);
        movement.y = doubleJumpSpeed;
        doubleJumpReady = false;
    }
    public void OnControllerColliderHit(ControllerColliderHit hit){
        if(movementState == MovementState.Dive && Vector3.Dot(Vector3.up, hit.normal) < 0.7071){
            // If player hit wall during dive
            // TODO: filter out collisions that are not head on
            // End dive
            // Stun character for slamming head into wall?
            SetState(MovementState.Standard);
        }
        if(movementState == MovementState.Midair && enableWallJump &&
            Vector3.Dot(Camera.main.transform.forward, hit.normal) < -0.0){
            // Hold onto wall
            wallJumpNormal = hit.normal;
            SetState(MovementState.WallJump);
        }
    }
    public void OnTriggerEnter(Collider collider){
        if(collider.gameObject.CompareTag("Bell")){
            // TODO: get bell
            collider.gameObject.GetComponent<BellController>().GetBell();
        }
    }
}
