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
    private Vector3 midairMomentum;
    [Header("Variable Height Jump")]
    public bool enableVariableJump;
    public float midJumpSpeed;
    public float highJumpSpeed;
    public float highJumpTimeWindow;
    private int jumpLevel = 0;
    [Header("Double Jump")]
    public bool enableDoubleJump;
    public float doubleJumpSpeed;
    public bool jumpAfterDive;
    private bool doubleJumpReady = false;
    [Header("Dive")]
    public bool diveEnabled;
    public float diveSpeedMultiplier;
    public float diveDrag;
    public float minDiveSpeed;
    public float endDiveSpeed;
    private Vector3 diveMovement;
    [Header("Wall Climb")]
    public bool wallClimbEnabled;
    public float wallJumpAngle;
    public float wallJumpSpeed;
    public float wallClimbDuration;
    public float wallClimbSpeed;
    public float wallSlideSpeed;
    public float wallClimbVaultRayOffset;
    private Vector3 wallClimbNormal;
    private float wallClimbProgress;
    [Header("Vault")]
    // TODO have variable to store normal data when vaulting, currently just use wall climb normal
    public float vaultRayOffset;
    public float vaultRayDistance;
    public float vaultLiftSpeed;
    public float vaultJumpSpeed;
    public float vaultJumpAngle;
    [Header("Slam")]
    public float slowdownRate;
    public GameObject slamTargetMarkerPrefab;
    private GameObject slamTargetMarker;
    private float normalFixedDeltaTime;
    [Header("Other")]

    private float timeOnGround = 0.0f;
    private CharacterController cc;
    private Vector3 movement;

    enum MovementState {
        Standard,
        Midair,
        WallClimb,
        Vault,
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
        this.normalFixedDeltaTime = Time.fixedDeltaTime;
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
                movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);

                if(!cc.isGrounded){
                    SetState(MovementState.Midair);
                } else if(Input.GetButtonDown("Jump")){
                    if(enableVariableJump){
                        VariableJump();
                    } else {
                        Jump();
                    }
                }
                break;
            case MovementState.Midair:
                controlledMovement *= midairSpeedMultiplier;
                movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);
                movement += midairMomentum;

                Vector3 flatMovement = movement;
                flatMovement.y = 0;
                if(diveEnabled && Input.GetButtonDown("Dive") && flatMovement.sqrMagnitude > (minDiveSpeed * minDiveSpeed)){
                    SetState(MovementState.Dive);
                } else if(Input.GetButtonDown("Jump") && enableDoubleJump && doubleJumpReady){
                    DoubleJump();
                } else if(Input.GetButtonDown("Fire1")){
                    SetState(MovementState.ChargeSlam);
                } else if(cc.isGrounded){
                    SetState(MovementState.Standard);
                }
                break;
            case MovementState.WallClimb:

                Vector3 climbMovement;
                if(wallClimbProgress <= wallClimbDuration){
                    // Climb wall
                    wallClimbProgress += Time.deltaTime;
                    climbMovement = new Vector3(0.0f, 1.0f, 0.0f);
                    climbMovement *= wallClimbSpeed;
                } else {
                    // Slide down Wall
                    climbMovement = new Vector3(0.0f, -1.0f, 0.0f);
                    climbMovement *= wallSlideSpeed;
                }
                movement = climbMovement;

                // TODO: consider using layer to filter out many collisions
                Vector3 rayOffset = new Vector3(0.0f, wallClimbVaultRayOffset, 0.0f);
                if (!Physics.Raycast(transform.position + rayOffset, wallClimbNormal * -1.0f, vaultRayDistance)){
                    // if character has reached top of wall, vault over
                    SetState(MovementState.Vault);

                } else if(cc.isGrounded){
                    SetState(MovementState.Standard);
                } else if(Input.GetButtonDown("Jump")){
                    Vector3 cross = Vector3.Cross(wallClimbNormal, Vector3.up);
                    Quaternion qua = Quaternion.AngleAxis(wallJumpAngle, cross);
                    Vector3 dir = qua * wallClimbNormal;
                    movement = dir * wallJumpSpeed;
                    SetState(MovementState.Midair);
                }
                break;
            case MovementState.Vault:
                Vector3 vaultLowerOffset = new Vector3(0.0f, vaultRayOffset, 0.0f);
                if (!Physics.Raycast(transform.position + vaultLowerOffset, wallClimbNormal * -1.0f, vaultRayDistance)){
                    // If character is high enough to vault
                    Vector3 cross = Vector3.Cross(wallClimbNormal, Vector3.up);
                    Quaternion qua = Quaternion.AngleAxis(-vaultJumpAngle, cross);
                    Vector3 dir = qua * (wallClimbNormal * -1.0f);
                    movement = dir * vaultJumpSpeed;
                    SetState(MovementState.Midair);
                } else {
                    Vector3 vaultMovement = new Vector3(0.0f, 1.0f, 0.0f);
                    vaultMovement *= wallClimbSpeed;
                    movement = vaultMovement;
                }
                break;
            case MovementState.Dive:
            
                // Slow dive movement on ground
                if (cc.isGrounded){
                    diveMovement *= (1.0f - (diveDrag * Time.deltaTime));
                }
                movement = new Vector3(diveMovement.x, movement.y, diveMovement.z);

                if(diveMovement.sqrMagnitude < (endDiveSpeed * endDiveSpeed)){
                    // TODO: change this from diveMovement to flat cc.velocity
                    SetState(MovementState.Standard);
                }
                break;
            case MovementState.ChargeSlam:
                // TODO: slow down over time
                Time.timeScale = slowdownRate;
                Time.fixedDeltaTime = this.normalFixedDeltaTime * Time.timeScale;
                movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);

                // TODO: aim slam target

                if(Input.GetButtonUp("Fire1")){
                    SetState(MovementState.Slam);
                }
                break;
            case MovementState.Slam:
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = this.normalFixedDeltaTime * Time.timeScale;
                //movement = new Vector3();
                if(cc.isGrounded)
                    SetState(MovementState.Standard);
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
                material.SetColor("_Color", Color.red);//
                break;
            case MovementState.Midair:
                material.SetColor("_Color", Color.black);//
                midairMomentum = movement * momentumMultiplier;
                midairMomentum.y = 0.0f;
                break;
            case MovementState.WallClimb:
                material.SetColor("_Color", Color.green);//
                wallClimbProgress = 0.0f;
                break;
            case MovementState.Vault:
                material.SetColor("_Color", Color.magenta);//
                break;
            case MovementState.Dive:
                material.SetColor("_Color", Color.blue);//
                Vector3 flatMovement = movement;
                flatMovement.y = 0;
                diveMovement = flatMovement * diveSpeedMultiplier;
                movement.y = 0;
                doubleJumpReady = jumpAfterDive;
                break;
            case MovementState.ChargeSlam:
                material.SetColor("_Color", Color.green);//
                if(slamTargetMarker == null){
                    slamTargetMarker = Instantiate(slamTargetMarkerPrefab);
                }
                RaycastHit hit;
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(transform.position, Vector3.up * -1.0f, out hit, Mathf.Infinity))
                {
                    slamTargetMarker.transform.position = hit.point + new Vector3(0.0f, 0.25f, 0.0f);
                }
                break;
            case MovementState.Slam:
                material.SetColor("_Color", Color.white);//
                // TODO: Store slam info based on input
                if(slamTargetMarker)
                    Destroy(slamTargetMarker);
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
        if(movementState == MovementState.Midair && wallClimbEnabled &&
            Vector3.Dot(Vector3.up, hit.normal) < 0.7071 && Vector3.Dot(Vector3.up, hit.normal) > -0.7071){
            // TODO base if on movement direction
            // Hold onto wall
            wallClimbNormal = hit.normal;
            SetState(MovementState.WallClimb);
        }
    }
    public void OnTriggerEnter(Collider collider){
        if(collider.gameObject.CompareTag("Bell")){
            // TODO: get bell
            collider.gameObject.GetComponent<BellController>().GetBell();
        }
    }
}
