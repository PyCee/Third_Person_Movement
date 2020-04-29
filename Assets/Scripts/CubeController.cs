using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CubeController : MonoBehaviour
{
    public float moveSpeed;
    public float lowJumpSpeed;
    public float midJumpSpeed;
    public float highJumpSpeed;
    public bool enableDoubleJump;
    public Transform cameraTransform;
    public float highJumpTimeWindow;
    public float diveDrag;
    public float endDiveSpeed;
    public float jumpControlSpeedMultiplier;
    public float diveSpeedMultiplier;
    public float crouchSpeedMultiplier;
    public float minDiveSpeed;

    private float timeOnGround = 0.0f;
    private bool doubleJump = false;
    private CharacterController cc;
    private Vector3 movement;
    private int jumpLevel = 0;
    private Vector3 diveMovement;

    enum MovementState {
        Standard,
        Jump,
        Crouch,
        Dive
    }
    private MovementState movementState = MovementState.Standard;

    void Start()
    {
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0.0f;
        Vector3 flatRight = cameraTransform.right;
        flatRight.y = 0.0f;
        Vector3 controlledMovement = flatForward.normalized * z + flatRight.normalized * x;
        controlledMovement *= moveSpeed;

        if (cc.isGrounded)
            WhileGrounded();

        if(Input.GetButtonDown("Crouch")){
            Vector3 flatMovement = movement;
            flatMovement.y = 0;
            if((!cc.isGrounded || flatMovement.sqrMagnitude > (minDiveSpeed * minDiveSpeed)) && 
                movementState != MovementState.Dive){
                diveMovement = flatMovement * diveSpeedMultiplier;
                movement.y = 0;
                movementState = MovementState.Dive;
                // TODO: start dive animation
            }
        }
        if (Input.GetButtonDown("Jump")){
            if(cc.isGrounded || (doubleJump && enableDoubleJump)){
                CharacterJump();
            }
        }

        switch(movementState){
            case MovementState.Standard:
                if(Input.GetButton("Crouch")){
                    movementState = MovementState.Crouch;
                    // TODO: start crouch animation
                }
                movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);
                break;
            case MovementState.Jump:
                controlledMovement *= jumpControlSpeedMultiplier;
                movement = new Vector3(controlledMovement.x, movement.y, controlledMovement.z);
                break;
            case MovementState.Dive:
                if (cc.isGrounded){
                    // Slow dive movement
                    diveMovement *= (1.0f - (diveDrag * Time.deltaTime));
                }
                if(diveMovement.sqrMagnitude < (endDiveSpeed * endDiveSpeed)){
                    // TODO: exit dive animation
                    movementState = MovementState.Standard;
                }
                movement = new Vector3(diveMovement.x, movement.y, diveMovement.z);
                break;
            case MovementState.Crouch:
                if(!Input.GetButton("Crouch")){
                    movementState = MovementState.Standard;
                }
                Vector3 crouchMovement = controlledMovement * crouchSpeedMultiplier;
                movement = new Vector3(crouchMovement.x, movement.y, crouchMovement.z);
                break;
            default:
                break;
        }
        

        movement += Physics.gravity * Time.deltaTime;
        cc.Move(movement * Time.deltaTime);
    }
    public void WhileGrounded(){
        doubleJump = true;
        timeOnGround += Time.deltaTime;
        if(timeOnGround > highJumpTimeWindow){
            jumpLevel = 0;
        }
        movement.y = 0.0f;
    }
    public void CharacterJump(){
        float[] jumpSpeed = {lowJumpSpeed, midJumpSpeed, highJumpSpeed};
        // Set jump animation
        movement.y = jumpSpeed[jumpLevel];
        doubleJump = cc.isGrounded;
        timeOnGround = 0.0f;
        jumpLevel++;
        if(jumpLevel > jumpSpeed.Length - 1)
            jumpLevel = 0;
    }
    public void OnControllerColliderHit(ControllerColliderHit hit){
        if(movementState == MovementState.Dive && Vector3.Dot(Vector3.up, hit.normal) < 0.7071){
            // If player hit wall during dive
            // End dive
            movementState = MovementState.Standard;
        }
    }
    public void OnTriggerEnter(Collider collider){
        if(collider.gameObject.CompareTag("Bell")){
            print("Got bell!");
            collider.gameObject.GetComponent<ParticleSystem>().Play();
        }
    }
}
