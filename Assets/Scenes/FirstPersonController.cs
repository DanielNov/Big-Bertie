using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;  //does the player have control of the character <Not set outside>
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool shouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool shouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")]
        [SerializeField] private bool canSprint = true;
        [SerializeField] private bool canJump = true;
        [SerializeField] private bool canCrouch = true;  

    [Header("Controls")]
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement Parameters")]
        [SerializeField] private float walkSpeed = 3.0f; //defaut walk speed and gravity
        [SerializeField] private float sprintSpeed = 4.5;
        [SerializeField] private float crouchSpeed = 1.5f;
    
    
    [Header("Look Parameters")]
        [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
        [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
        [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f; //Camera look limit
        [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;
    
    [Header("Jumping Parameters")]
        [SerializeField] private float jumpForce = 8.0f;
        [SerializeField] private float gravity = 30.0f;

    [Header("Counching Parameters")]
        [SerializeField] private float crouchHeight = 0.5f;
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float timeToCrouch = 0.25f;
        [SerializeField] private Vector3 crouchingCentre = new Vector3(0,0.5f,0);
        [SerializeField] private Vector3 standingCentre = new  Vector3(0, 0, 0);
            private bool isCrouching;
            private bool duringCrouchAnimation;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; //Locked Cursor
        Cursor.visible = false;  
    }

    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook(); 

            if(canJump) 
                HandleJump();
            
            if (canCrouch)
                HandleCrouch();

            ApplyFinalMovements();

        }

    }

    private void HandleMovementInput() //Keyboard Controller 
    {
currentInput = new Vector2(
    (isCrouching ? crouchSpeed : (IsSprinting ? sprintSpeed : walkSpeed)) * Input.GetAxis("Vertical"),
    (IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
            //Reset Vecotr Direction for next movement 
        float moveDirectionY  = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY; // moveDirection.y set to cached moveDirectionY
    }

    private void HandleMouseLook()
    {   
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler (rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }
    
    private void HandleJump()
    {
        if (shouldJump)
            moveDirection.y = jumpForce;
    }
   
    private void HandleCrouch()
    {
        if (shouldCrouch)
            StartCoroutine(CrouchStand()); 
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight =  isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCentre = isCrouching ? standingCentre : crouchingCentre;
        Vector3 currentCentre = characterController.center;

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCentre, targetCentre, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
            characterController.height = targetHeight;
            characterController.center = targetCentre;

            isCrouching = !isCrouching;

            duringCrouchAnimation = false;
    }

}