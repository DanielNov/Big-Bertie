using UnityEngine;
using UnityEngine.Animations;
//Header - aby to vyzeralo pekne v editore 
public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;  //does the player have control of the character <Not set outside>
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f; //defaut walk speed and gravity
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float gravity = 30.0f;
    
    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f; //Camera look limit
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

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
            ApplyFinalMovements();

        }

    }

    private void HandleMovementInput() //Keyboard Controller 
    {
        currentInput = new Vector2 ((IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
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

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

    }



}